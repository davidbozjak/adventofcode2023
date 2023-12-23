using System.Drawing;

var inputStrings = new StringInputProvider("Input.txt").ToList();
var world = new TileWorld(inputStrings, false, GetTile, IsDownwardsOnSlope);

List<TunnelTile> tunnels = new List<TunnelTile>();
(var start, var end) = PrepareTunnels(world);

var path = FindLongestTunnelPathWithoutSameTile(start, end);

Console.WriteLine($"Part 1: {GetDistanceForPath(path)}");

world = new TileWorld(inputStrings, false, GetTile);

(start, end) = PrepareTunnels(world);

path = FindLongestTunnelPathWithoutSameTile(start, end);

Console.WriteLine($"Part 2: {GetDistanceForPath(path)}");

static int GetDistanceForPath(List<TunnelTile> path)
{
    int distance = 0;
    for (int i = 0; i < path.Count - 1; i++)
    {
        var from = path[i];
        var to = path[i + 1];
        distance += from.GetDistanceTo(to.Position);
    }
    return distance;
}

(TunnelTile start, TunnelTile end) PrepareTunnels(TileWorld world)
{
    var start = world.WorldObjects.Cast<Tile>()
    .Where(w => w.CharRepresentation == '.')
    .Where(w => w.Position.Y == 0)
    .First();

    var end = world.WorldObjects.Cast<Tile>()
        .Where(w => w.CharRepresentation == '.')
        .Where(w => w.Position.Y == world.MaxY)
        .First();

    var crossways = world.WorldObjects.Cast<Tile>()
        .Where(w => w.IsTraversable)
        .Where(w => w.TraversibleNeighbours.Count() > 2)
        .ToList();

    tunnels.Clear();
    tunnels.AddRange(crossways
        .Select(w => new TunnelTile(w.Position.X, w.Position.Y, GetConnectedTunnels)));

    var tunnelStart = new TunnelTile(start.Position.X, start.Position.Y, GetConnectedTunnels);
    tunnels.Add(tunnelStart);
    var tunnelEnd = new TunnelTile(end.Position.X, end.Position.Y, GetConnectedTunnels);
    tunnels.Add(tunnelEnd);

    return (tunnelStart, tunnelEnd);
}

IEnumerable<TunnelTile> GetConnectedTunnels(Tile start)
{
    if (!(start is TunnelTile tunnelTile))
        throw new Exception();

    var realWorldTile = world.GetTileAt(tunnelTile.Position);

    List<(TunnelTile, int)> neighbours = new();

    foreach (var n in realWorldTile.TraversibleNeighbours)
    {
        //simply follow the path until you get to another 
        Tile prev = realWorldTile;
        var current = n;
        int distance = 1;
        while (current.TraversibleNeighbours.Count() <= 2)
        {
            var next = current.TraversibleNeighbours.FirstOrDefault(w => w != prev);

            if (next == null)
            {
                break;
            }
            
            prev = current;
            current = next;
            distance++;
        }

        var tunnelEnd = tunnels.FirstOrDefault(w => w.Position == current.Position);
        if (tunnelEnd != null)
        {
            neighbours.Add((tunnelEnd, distance));
            tunnelTile.AddDistanceTo(current.Position, distance);
        }
    }

    return neighbours.Select(w => w.Item1);
}

List<TunnelTile> FindLongestTunnelPathWithoutSameTile(TunnelTile start, TunnelTile goal)
{
    PriorityQueue<TunnelState, int> queue = new();
    var startState = new TunnelState(start, new List<TunnelTile>(), new HashSet<TunnelTile>());
    queue.Enqueue(startState, 0);

    var longestPath = new List<TunnelTile>();
    int longestPathLength = -1;

    while (queue.Count > 0)
    {
        var current = queue.Dequeue();

        if (current.Tile == goal)
        {
            current.PathToHere.Add(current.Tile);
            var length = GetDistanceForPath(current.PathToHere);

            if (length > longestPathLength)
            {
                Console.WriteLine($"{DateTime.Now.TimeOfDay}: New MAX path found: {length} in {current.VisitedTiles.Count} TunnelNodes");
                longestPath = current.PathToHere;
                longestPathLength = length;
            }

            continue;
        }

        foreach (var next in current.Tile.TraversibleNeighbours.Cast<TunnelTile>())
        {
            if (current.VisitedTiles.Contains(next))
                continue;

            var newPath = current.PathToHere.ToList();
            newPath.Add(current.Tile);

            queue.Enqueue(new TunnelState(next, newPath, newPath.ToHashSet()), -newPath.Count);
        }
    }

    return longestPath;
}

bool IsDownwardsOnSlope(Tile t1, Tile t2)
{
    if (!t1.Position.IsNeighbour(t2.Position))
        throw new Exception();

    if (t1.Position == t2.Position)
        throw new Exception();

    if (t1 is SlopeTile slopeTile)
    {
        if (slopeTile.CharRepresentation == '>')
        {
            if (t2.Position == slopeTile.Position.Right())
                return true;
            else
                return false;
        }
        else if (slopeTile.CharRepresentation == '<')
        {
            if (t2.Position == slopeTile.Position.Left())
                return true;
            else
                return false;
        }
        else if (slopeTile.CharRepresentation == 'v')
        {
            if (t2.Position == slopeTile.Position.Down())
                return true;
            else
                return false;
        }
        else if (slopeTile.CharRepresentation == '^')
        {
            if (t2.Position == slopeTile.Position.Up())
                return true;
            else
                return false;
        }
    }

    return true;
}

Tile GetTile(int x, int y, char c, Func<Tile, IEnumerable<Tile>> func)
{
    if (c != '.' && c != '#')
    {
        return new SlopeTile(x, y, c, func);
    }

    return new Tile(x, y, c != '#', func);
}

record TunnelState(TunnelTile Tile, List<TunnelTile> PathToHere, HashSet<TunnelTile> VisitedTiles);

class SlopeTile : Tile
{
    private readonly char c;

    public override char CharRepresentation => this.c;

    public SlopeTile(int x, int y, char c, Func<Tile, IEnumerable<Tile>> fillTraversibleNeighboursFunc) 
        : base(x, y, true, fillTraversibleNeighboursFunc)
    {
        this.c = c;
    }
}

class TunnelTile : Tile
{
    private readonly Dictionary<Point, int> distancesToNeighbours = new();

    public TunnelTile(int x, int y, Func<Tile, IEnumerable<Tile>> fillTraversibleNeighboursFunc) 
        : base(x, y, true, fillTraversibleNeighboursFunc)
    {
    }

    public void AddDistanceTo(Point point, int distance)
    {
        this.distancesToNeighbours.Add(point, distance);
    }

    public int GetDistanceTo(Point point)
    {
        if (!this.TraversibleNeighbours.Any(w => w.Position == point))
            throw new Exception();

        return distancesToNeighbours[point];
    }
}