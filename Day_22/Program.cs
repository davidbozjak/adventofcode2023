using System.Text.RegularExpressions;

var bricks = new InputProvider<SandBrick?>("Input.txt", GetSandBrick).Where(w => w != null).Cast<SandBrick>().ToList();

Dictionary<(int, int), SortedList<int, SandBrick>> collapsedGrid = new();
var toCollapse = bricks.OrderBy(w => w.MinZ).ToList();
var collapsedBricks = new List<SandBrick>();

while (toCollapse.Count > 0)
{
    var brick = toCollapse[0];
    toCollapse.RemoveAt(0);

    int newZ = -1;
    List<SandBrick> bricksJustBelow = new();

    foreach (var point in brick.Footprint)
    {
        if (collapsedGrid.ContainsKey(point))
        {
            var stackBelow = collapsedGrid[point];
            var brickBelow = stackBelow.Last().Value;
            var z = brickBelow.MaxZ;
            if (z > newZ)
            {
                newZ = z;
                bricksJustBelow = [brickBelow];
            }
            else if (z == newZ)
            {
                bricksJustBelow.Add(brickBelow);
            }
        }
    }

    var movedBrick = brick.GetCopyAtZ(newZ + 1);
    collapsedBricks.Add(movedBrick);

    foreach (var brickBelow in bricksJustBelow)
    {
        if (!collapsedBricks.Contains(brickBelow))
            throw new Exception();

        movedBrick.AddSupportingBrick(brickBelow);
        brickBelow.AddSupportedBrick(movedBrick);
    }

    foreach (var voxel in movedBrick.Voxels)
    {
        if (!collapsedGrid.ContainsKey((voxel.x, voxel.y)))
        {
            collapsedGrid[(voxel.x, voxel.y)] = new SortedList<int, SandBrick>();
        }

        collapsedGrid[(voxel.x, voxel.y)].Add(voxel.z, movedBrick);
    }
}

if (collapsedBricks.Count != bricks.Count)
    throw new Exception();

if (collapsedBricks.Any(w => w.MinZ > 0 && w.DirectlySupportingBricks.Count() == 0))
    throw new Exception();

Console.WriteLine($"Part 1: {collapsedBricks.Count(w => !w.IsLoadBearing)}");

var loadBearingBricks = collapsedBricks.Where(w => w.IsLoadBearing).ToList();
int countChainExplosion = 0;

foreach (var loadBearingBrick in loadBearingBricks)
{
    HashSet<SandBrick> explodedBricks = [loadBearingBrick];

    bool foundAnyNew;
    do
    {
        foundAnyNew = false;
        foreach (var carriedBrick in collapsedBricks)
        {
            if (carriedBrick == loadBearingBrick)
                continue;

            if (carriedBrick.DirectlySupportingBricks.Any() && carriedBrick.DirectlySupportingBricks.All(explodedBricks.Contains))
            {
                if (!explodedBricks.Contains(carriedBrick))
                {
                    foundAnyNew = true;
                    explodedBricks.Add(carriedBrick);
                }
            }
        }
    } while (foundAnyNew);

    countChainExplosion += explodedBricks.Count - 1;
}

Console.WriteLine($"Part 2: {countChainExplosion}");

static bool GetSandBrick(string? input, out SandBrick? value)
{
    value = null;

    if (input == null) return false;

    Regex numRegex = new(@"-?\d+");

    var numbers = numRegex.Matches(input).Select(w => int.Parse(w.Value)).ToArray();

    value = new SandBrick(numbers[0], numbers[1], numbers[2], numbers[3], numbers[4], numbers[5]);

    return true;
}

class SandBrick
{
    private readonly HashSet<SandBrick> directlySupportingBricks = new();
    private readonly HashSet<SandBrick> directlySupportedBricks = new();
    private readonly Cached<List<(int x, int y, int z)>> cachedVoxels;
    private readonly Cached<HashSet<(int x, int y)>> cachedFootprint;
    private readonly Cached<bool> cachedIsLoadBearing;
    private readonly int X1, Y1, Z1, X2, Y2, Z2;

    public IEnumerable<(int x, int y, int z)> Voxels => this.cachedVoxels.Value;

    public IEnumerable<(int x, int y)> Footprint => this.cachedFootprint.Value;

    public IEnumerable<SandBrick> DirectlySupportedBricks => this.directlySupportedBricks.ToList().AsReadOnly();

    public IEnumerable<SandBrick> DirectlySupportingBricks => this.directlySupportingBricks.ToList().AsReadOnly();

    public int MaxZ => this.Voxels.Select(w => w.z).Max();
    public int MinZ => this.Voxels.Select(w => w.z).Min();

    public bool IsLoadBearing => this.cachedIsLoadBearing.Value;

    public SandBrick(int X1, int Y1, int Z1, int X2, int Y2, int Z2)
    {
        this.X1 = X1; 
        this.Y1 = Y1; 
        this.Z1 = Z1;
        this.X2 = X2;
        this.Y2 = Y2;
        this.Z2 = Z2;
        this.cachedVoxels = new Cached<List<(int x, int y, int z)>>(() => GetVoxels().ToList());
        this.cachedFootprint = new Cached<HashSet<(int x, int y)>>(() => this.Voxels.Select(w => (w.x, w.y)).ToHashSet());
        this.cachedIsLoadBearing = new Cached<bool>(this.GetIsLoadBearing);
    }

    public SandBrick GetCopyAtZ(int Z)
    {
        return new SandBrick(X1, Y1, Z, X2, Y2, Z + (Z2 - Z1));
    }

    public void AddSupportedBrick(SandBrick sandBrick)
    {
        this.directlySupportedBricks.Add(sandBrick);
    }

    public void AddSupportingBrick(SandBrick sandBrick)
    {
        this.directlySupportingBricks.Add(sandBrick);
    }

    private bool GetIsLoadBearing()
    {
        foreach (var brickIAmSupporting in this.directlySupportedBricks)
        {
            if (brickIAmSupporting.directlySupportingBricks.Count == 1)
            {
                return true;
            }
        }

        return false;
    }

    private IEnumerable<(int x, int y, int z)> GetVoxels()
    {
        if ((X1 == X2) && (Y1 == Y2))
        {
            var diff = Z2 - Z1;
            return EnumerateLine(diff > 0 ? diff : -diff, (X1, Y1, Z1), w => (w.x, w.y, w.z + (diff > 0 ? 1 : -1)));
        }
        else if ((X1 == X2) && (Z1 == Z2))
        {
            var diff = Y2 - Y1;

            return EnumerateLine(diff > 0 ? diff : -diff, (X1, Y1, Z1), w => (w.x, w.y + (diff > 0 ? 1 : -1), w.z));
        }
        else
        {
            var diff = X2 - X1;

            return EnumerateLine(diff > 0 ? diff : -diff, (X1, Y1, Z1), w => (w.x + (diff > 0 ? 1 : -1), w.y, w.z));
        }
    }

    private IEnumerable<(int x, int y, int z)> EnumerateLine(int steps, (int x, int y, int z) point, Func<(int x, int y, int z), (int x, int y, int z)> iterator)
    {
        for (int i = 0; i <= steps; i++)
        {
            yield return point;
            point = iterator(point);
        }
    }
}