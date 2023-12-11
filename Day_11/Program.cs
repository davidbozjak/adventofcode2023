var input = new StringInputProvider("Input.txt").ToList();

int expansionFactor = 1000000 - 1;

var allGalaxies = new List<Galaxy>();

for (int y = 0; y < input.Count; y++)
{
    for (int x = 0; x < input[0].Length; x++)
    {
        if (input[y][x] == '#')
        {
            allGalaxies.Add(new Galaxy(allGalaxies.Count, x, y));
        }
    }
}

for (int y = 0; y < input.Count; y++)
{
    var row = input[y];

    bool contansAny = row.IndexOf("#") > -1;

    if (!contansAny)
    {
        var toModify = allGalaxies.Where(w => w.OriginalY > y).ToList();

        foreach (var galaxy in toModify)
        {
            galaxy.OffsetY(expansionFactor);
        }
    }
}

for (int x = 0; x < input[0].Length; x++)
{
    var row = new string(input.Select(w => w[x]).ToArray());

    bool contansAny = row.IndexOf("#") > -1;

    if (!contansAny)
    {
        var toModify = allGalaxies.Where(w => w.OriginalX > x).ToList();

        foreach (var galaxy in toModify)
        {
            galaxy.OffsetX(expansionFactor);
        }
    }
}

var distances = new List<long>();

for (int i = 0; i < allGalaxies.Count; i++)
{
    var g1 = allGalaxies[i];
    for (int j = i + 1; j < allGalaxies.Count; j++)
    {
        var g2 = allGalaxies[j];
        var distance = GetDistance(g1.X, g1.Y, g2.X, g2.Y);
        distances.Add(distance);
    }
}

Console.WriteLine($"Distances: {distances.Sum()}");

long GetDistance(long x1, long y1, long x2, long y2)
{
    return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
}

class Galaxy
{
    public int Id { get; }

    public long OriginalX { get; }

    public long OriginalY { get; }

    public long X { get; private set; }

    public long Y { get; private set; }

    public Galaxy(int id, int x, int y)
    {
        this.Id = id;
        this.OriginalX = this.X = x;
        this.OriginalY = this.Y = y;
    }

    public void OffsetX(int offsetX)
    {
        this.X += offsetX;
    }

    public void OffsetY(int offsetY)
    {
        this.Y += offsetY;
    }
}