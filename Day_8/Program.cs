var instructions = new StringInputProvider("Instructions.txt").First().ToCharArray();
var tiles = new InputProvider<Tile?>("Map.txt", GetTile).Where(w => w != null).Cast<Tile>().ToList();

var tileDict = tiles.ToDictionary(w => w.Name);

Console.WriteLine($"Part 1: { GetNumberOfSteps(tileDict["AAA"], new[] { tileDict["ZZZ"] })}");

var allStarts = tiles.Where(w => w.Name.EndsWith("A")).ToList();
var allEnds = tiles.Where(w => w.Name.EndsWith("Z")).ToList();

var numberOfSteps = new List<long>();

foreach (var start in allStarts)
{
    numberOfSteps.Add(GetNumberOfSteps(start, allEnds));
}

Console.WriteLine($"Part 2: {MathUtils.LeastCommonMultiple(numberOfSteps)}");


long GetNumberOfSteps(Tile start, IEnumerable<Tile> endtiles)
{
    long stepCount = 0;
    var current = start;
    var provider = new CyclicalElementProvider<char>(instructions.Select(w => new Func<char>(() => w)));
    for (; !endtiles.Contains(current); stepCount++, provider.MoveNext())
    {
        current = provider.Current switch
        {
            'R' => tileDict[current.Right],
            'L' => tileDict[current.Left],
            _ => throw new Exception()
        };
    }

    return stepCount;
}

static bool GetTile(string? input, out Tile? value)
{
    value = null;

    if (input == null) return false;

    var parts = input.Split(new[] { ",", " ", "(", ")", "=" }, StringSplitOptions.RemoveEmptyEntries);
    
    value = new Tile(parts[0], parts[1], parts[2]);

    return true;
}

class Tile(string name, string left, string right)
{
    public string Name => name;

    public string Left => left;

    public string Right => right;
}