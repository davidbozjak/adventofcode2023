using System.Drawing;

var inputStrings = new StringInputProvider("Input.txt").ToList();
var world = new TiltableWorld(inputStrings, false, GetTile);
var printer = new WorldPrinter();

world.TiltNorth();

Console.WriteLine($"Part 1: {GetTotalLoadOnNorthBeams()}");

world = new TiltableWorld(inputStrings, false, GetTile);
var cycleDict = new Dictionary<string, int>
{
    { printer.PrintToString(world), 0 }
};

int cyclesToDo = 1000000000;
bool hasFoundCycle = false;

for (int cycle = 0; cycle < cyclesToDo; cycle++)
{
    world.TiltNorth();
    world.TiltWest();
    world.TiltSouth();
    world.TiltEast();

    var state = printer.PrintToString(world);

    if (!hasFoundCycle && cycleDict.ContainsKey(state))
    {
        hasFoundCycle = true;

        int cycleLength = cycle - cycleDict[state];
        int repeatsRemaining = (cyclesToDo - cycle) / cycleLength;
        cycle += repeatsRemaining * cycleLength;
    }
    else
    {
        cycleDict[state] = cycle;
    }
}

Console.WriteLine($"Part 2: {GetTotalLoadOnNorthBeams()}");

int GetTotalLoadOnNorthBeams()
{
    int maxY = world.MaxY + 1;
    var weightOnNorth = world.WorldObjects
        .Where(w => w is RockTile)
        .Cast<RockTile>()
        .Where(w => w.IsRoundRock)
        .Select(w => maxY - w.Position.Y)
        .Sum();

    return weightOnNorth;
}

Tile GetTile(int x, int y, char c, Func<Tile, IEnumerable<Tile>> func)
{
    if (c == '.')
    {
        return new Tile(x, y, true, func);
    }
    else
    {
        return new RockTile(x, y, c, func);
    }
}

class TiltableWorld : TileWorld
{
    public TiltableWorld(IEnumerable<string> map, bool allowDiagnoalNeighbours, Func<int, int, char, Func<Tile, IEnumerable<Tile>>, Tile> tileCreatingFunc, Func<Tile, Tile, bool> isValidNeighbourFunc = null) 
        : base(map, allowDiagnoalNeighbours, tileCreatingFunc, isValidNeighbourFunc)
    {
    }

    public void TiltNorth()
    {
        Tilt(w => w.Y > 0, w => GetTileAtOrNull(w.X, w.Y - 1), w => w.Position.Y * 1000 + w.Position.X);
    }

    public void TiltSouth()
    {
        Tilt(w => w.Y < this.MaxY, w => GetTileAtOrNull(w.X, w.Y + 1), w => -w.Position.Y * 1000 + w.Position.X);
    }

    public void TiltWest()
    {
        Tilt(w => w.X > 0, w => GetTileAtOrNull(w.X - 1, w.Y), w => w.Position.X * 1000 + w.Position.Y);
    }

    public void TiltEast()
    {
        Tilt(w => w.X < this.MaxX, w => GetTileAtOrNull(w.X + 1, w.Y), w => -w.Position.X * 1000 + w.Position.Y);
    }

    public void Tilt(Func<Point, bool> pointCondition, Func<Point, Tile?> getNextTileFunc, Func<Tile, int> getOrderFunc)
    {
        var tilesToMove = this.allTiles.Values
            .Where(w => w is RockTile)
            .Cast<RockTile>()
            .Where(w => w.IsRoundRock)
            .OrderBy(getOrderFunc)
            .ToList();

        foreach (var tile in tilesToMove)
        {
            while (pointCondition(tile.Position))
            {
                var currentTile = getNextTileFunc(tile.Position);

                if (currentTile == null)
                {
                    break;
                }

                if (currentTile is RockTile)
                {
                    break;
                }

                SwapTiles(tile, currentTile);
            }
        }
    }

    private void SwapTiles(Tile tile1, Tile tile2)
    {
        var pos1 = tile1.Position;
        var pos2 = tile2.Position;

        this.allTiles.Remove(tile1.Position);
        this.allTiles.Remove(tile2.Position);
        
        tile1.Move(pos2.X, pos2.Y);
        tile2.Move(pos1.X, pos1.Y);

        this.allTiles.Add(tile1.Position, tile1);
        this.allTiles.Add(tile2.Position, tile2);
    }
}

class RockTile : Tile
{
    public bool IsRoundRock { get; }

    public override char CharRepresentation => IsRoundRock ? 'O' : '#';

    public RockTile(int x, int y, char c, Func<Tile, IEnumerable<Tile>> fillTraversibleNeighboursFunc) 
        : base(x, y, true, fillTraversibleNeighboursFunc)
    {
        this.IsRoundRock = c == 'O';
    }
}