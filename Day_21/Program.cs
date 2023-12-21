using System.Drawing;

var inputStrings = new StringInputProvider("Input.txt").ToList();

int startX = -1;
int startY = -1;

var world = new TileWorld(inputStrings, false, GetTile);

Console.WriteLine($"Part 1: {GetAllVisitedTilesForExactlySteps(64)}");

int totalStepTarget = 26501365;
int gridLength = world.MaxX + 1;
int repeats = totalStepTarget / gridLength;
int diff = totalStepTarget % gridLength;

var results = new List<int>();

// we need 3 points to get the extrapolation formula, and get the fourth to verify our assumption (the fourth takes a very long time, only do validation once)
for(int i = 0; i < 4; i++)
{
    results.Add(GetAllVisitedTilesForExactlySteps(diff + i * gridLength));
}

var calculatedValue = results.Last();
var extrapolatedValue = GetExtrapolatedValue(results.Count - 1, results.Take(3).ToArray());

if (calculatedValue != extrapolatedValue)
    throw new Exception("Predicted value not matching");

Console.WriteLine($"Part 2: {GetExtrapolatedValue(repeats, results.ToArray())}");

int GetAllVisitedTilesForExactlySteps(int stepTarget)
{
    var current = world.GetTileAt(startX, startY);

    PriorityQueue<(Point, int), int> toVisit = new();
    toVisit.Enqueue((current.Position, 0), 0);

    HashSet<Point> exactlyStepTarget = new();

    HashSet<(Point, int)> memCache = new();

    while (toVisit.Count > 0)
    {
        (Point currentPos, int stepsToHere) = toVisit.Dequeue();

        memCache.Add((currentPos, stepsToHere));

        if (stepsToHere == stepTarget)
        {
            exactlyStepTarget.Add(currentPos);
            continue;
        }
        else if (stepsToHere > stepTarget)
        {
            continue;
        }

        Point[] surroundingPoints = [currentPos.Up(), currentPos.Down(), currentPos.Left(), currentPos.Right()];

        foreach (var p in surroundingPoints)
        {
            var inMapX = p.X % (world.MaxX + 1);
            var inMapY = p.Y % (world.MaxY + 1);

            if (inMapX < 0)
                inMapX += world.MaxX + 1;
            if (inMapY < 0)
                inMapY += world.MaxY + 1;

            current = world.GetTileAt(inMapX, inMapY);

            if (!current.IsTraversable)
                continue;

            if (memCache.Contains((p, stepsToHere + 1)))
                continue;

            for (int step = stepsToHere + 1; step <= stepTarget; step += 2)
            {
                toVisit.Enqueue((p, step), -step);
            }
        }
    }

    return exactlyStepTarget.Count;
}

long GetExtrapolatedValue(int repeats, int[] calculatedValues)
{
    // Credit to kind redditor who shared the formula for determining the quadratic equation
    var c = calculatedValues[0];
    var aPlusB = calculatedValues[1] - c;
    var fourAPlusTwoB = calculatedValues[2] - c;
    var twoA = fourAPlusTwoB - (2 * aPlusB);
    var a = twoA / 2;
    var b = aPlusB - a;

    return Func(repeats);

    long Func(long n)
    {
        return a * (n * n) + b * n + c;
    }
}

Tile GetTile(int x, int y, char c, Func<Tile, IEnumerable<Tile>> func)
{
    if (c == 'S')
    {
        startX = x;
        startY = y;
        c = '.';
    }

    return new Tile(x, y, c != '#', func);
}
