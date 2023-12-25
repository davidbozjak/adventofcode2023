var input = new StringInputProvider("Input.txt").ToList();

var componentsFactory = new UniqueFactory<string, Component>(name => new Component(name, new HashSet<Component>()));
var linksFactory = new UniqueFactory<(string, string), Link>(pair =>
    new Link(componentsFactory.GetOrCreateInstance(pair.Item1), componentsFactory.GetOrCreateInstance(pair.Item2)));

foreach (var line in input)
{
    var parts = line.Split([" ", ":"], StringSplitOptions.RemoveEmptyEntries);

    var component = componentsFactory.GetOrCreateInstance(parts[0]);

    foreach (var part in parts.Skip(1))
    {
        var connectedComponent = componentsFactory.GetOrCreateInstance(part);

        AddLink(GetLink(component, connectedComponent));
    }
}

var orderedComponents = componentsFactory.AllCreatedInstances.OrderBy(w => w.ConnectedComponents.Count).ToList();

Console.WriteLine($"{linksFactory.AllCreatedInstances.Count} links found");

Dictionary<Link, int> linkFrequency = new();
var random = new Random();

//monte-carlo determine frequency
for (int i = 0; i < 10000; i++)
{
    var first = random.Next(orderedComponents.Count);
    var second = random.Next(orderedComponents.Count);
    if (first == second) continue;

    var from = orderedComponents[first];
    var to = orderedComponents[second];

    var visited = new HashSet<Component>();

    var path = AStarPathfinder.FindPath(from, to, w => 0, w => w.ConnectedComponents);
    
    //var path = Find(from, to, new HashSet<Component>(), new List<Link>());
    if (path == null)
        throw new Exception("We are not removing any links here so every node should be reachable");

    for (int j = 0; j < path.Count - 1; j++)
    {
        var link = GetLink(path[j], path[j + 1]);
        if (!linkFrequency.ContainsKey(link))
        {
            linkFrequency[link] = 0;
        }
        linkFrequency[link]++;
    }
}

var mostProminentLinks = linkFrequency.OrderByDescending(w => w.Value).Select(w => w.Key).ToList();

bool found = false;
for (int i = 2; i < mostProminentLinks.Count && !found; i++)
{
    var link1 = mostProminentLinks[i - 2];
    var link2 = mostProminentLinks[i - 1];
    var link3 = mostProminentLinks[i];

    RemoveLink(link1);
    RemoveLink(link2);
    RemoveLink(link3);

    var group1 = new HashSet<Component>();

    if (!AreConnected(link3.Component1, link3.Component2, group1))
    {
        FillAllReachable(link3.Component1, group1);

        if (group1.Count != componentsFactory.AllCreatedInstances.Count)
        {
            found = true;
            var notIncludedCount = componentsFactory.AllCreatedInstances.Where(w => !group1.Contains(w)).Count();
            Console.WriteLine($"Found!: Group 1 size: {group1.Count} Group 2 size: {notIncludedCount} Result: {group1.Count * notIncludedCount}");
        }
    }

    AddLink(link1);
    AddLink(link2);
    AddLink(link3);
}

if (!found)
{
    Console.WriteLine($"Not expected, no groups found");
}

Console.ReadKey();

Link GetLink(Component component1 , Component component2)
{
    List<Component> components = [component1, component2];
    components = components.OrderBy(w => w.Name).ToList();

    return linksFactory.GetOrCreateInstance((components[0].Name, components[1].Name));
}

void RemoveLink(Link link)
{
    link.Component1.ConnectedComponents.Remove(link.Component2);
    link.Component2.ConnectedComponents.Remove(link.Component1);
}

void AddLink(Link link)
{
    link.Component1.ConnectedComponents.Add(link.Component2);
    link.Component2.ConnectedComponents.Add(link.Component1);
}

static void FillAllReachable(Component component, HashSet<Component> reachableComponents)
{
    if (reachableComponents.Contains(component))
        return;

    reachableComponents.Add(component);

    foreach (var connectedComponent in component.ConnectedComponents)
    {
        FillAllReachable(connectedComponent, reachableComponents);
    }
}

static bool AreConnected(Component component, Component soughtComponent, HashSet<Component> visited)
{
    if (visited.Contains(component))
        return false;

    visited.Add(component);

    if (component.ConnectedComponents.Contains(soughtComponent))
        return true;

    foreach (var connectedComponent in component.ConnectedComponents)
    {
        if (AreConnected(connectedComponent, soughtComponent, visited)) 
            return true;
    }

    return false;
}

record Component(string Name, HashSet<Component> ConnectedComponents) : INode
{
    public int Cost => 1;
}

record Link(Component Component1, Component Component2);