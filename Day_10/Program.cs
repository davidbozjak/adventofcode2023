var inputStrings = new StringInputProvider("Input.txt");
var world = new TileWorld(inputStrings, false, GetTile, AreConnectedNeighbours);

var startTile = world.WorldObjects.Cast<PipedTile>().First(w => w.CharRepresentation == 'S');
startTile.DistanceToStart = 0;
startTile.Status = PipedTile.LoopStatus.OnTheLoop;
startTile.OverrideTileChar(ReverseCharLookup(startTile));

var tilesToCheck = new Stack<PipedTile>() { };
tilesToCheck.Push(startTile);

var checkedTiles = new HashSet<PipedTile>() { startTile };

while (tilesToCheck.Count > 0)
{
    var current = tilesToCheck.Pop();
    checkedTiles.Add(current);

    var distance = current.DistanceToStart + 1;

    foreach (PipedTile n in current.TraversibleNeighbours.Cast<PipedTile>())
    {
        n.Status = PipedTile.LoopStatus.OnTheLoop;

        if (distance < n.DistanceToStart)
        {
            n.DistanceToStart = distance;
            tilesToCheck.Push(n);
        }
    }
}

for (int y = world.MinY; y <= world.MaxY; y++)
{
    int depth = 0;
    char lastRelavantChar = '.';

    for (int x = world.MinX; x <= world.MaxX; x++)
    {
        var tile = world.GetTileAtOrNull(x, y) as PipedTile;

        if (tile == null)
            continue;

        if (tile.Status != PipedTile.LoopStatus.OnTheLoop)
        {
            tile.Status = (depth % 2) == 1 ? PipedTile.LoopStatus.Inside : PipedTile.LoopStatus.Outside;
        }
        else
        {
            if (tile.CharRepresentation == '-')
            {

            }
            else if (tile.CharRepresentation == '|')
            {
                depth++;
            }
            else if (tile.CharRepresentation == 'F')
            {
                lastRelavantChar = 'F';
            }
            else if (tile.CharRepresentation == 'L')
            {
                lastRelavantChar = 'L';
            }
            else if (tile.CharRepresentation == '7')
            {
                if (lastRelavantChar == 'L')
                {
                    depth++;
                }
            }
            else if (tile.CharRepresentation == 'J')
            {
                if (lastRelavantChar == 'F')
                {
                    depth++;
                }
            }
            else throw new Exception();
        }
    }
}

var insideTiles = world.WorldObjects.Cast<PipedTile>().Where(w => w.Status == PipedTile.LoopStatus.Inside).ToList();

var paintedWorld = new WorldWithMarkings<PipedTile>(world, insideTiles.Select(w => (w, w.Status switch
{
    PipedTile.LoopStatus.OnTheLoop => w.CharRepresentation,
    PipedTile.LoopStatus.Outside => '.',
    PipedTile.LoopStatus.Inside => '*',
    _ => throw new Exception()
})));

var printer = new WorldPrinter();
printer.Print(paintedWorld);


Console.WriteLine(); Console.WriteLine();
Console.WriteLine($"Part 2: {checkedTiles.Max(w => w.DistanceToStart)}");
Console.WriteLine($"Part 2: {insideTiles.Count}");

Tile GetTile(int x, int y, char c, Func<Tile, IEnumerable<Tile>> func)
{
    return new PipedTile(x, y, c, func);
}

static bool AreConnectedNeighbours(Tile tile, Tile tile2)
{
    if (tile.CharRepresentation == '|' && (tile2.Position == tile.Position.Up() || tile2.Position == tile.Position.Down()))
        return true;
    else if (tile.CharRepresentation == '-' && (tile2.Position == tile.Position.Left() || tile2.Position == tile.Position.Right()))
        return true;
    else if (tile.CharRepresentation == 'L' && (tile2.Position == tile.Position.Up() || tile2.Position == tile.Position.Right()))
        return true;
    else if (tile.CharRepresentation == 'J' && (tile2.Position == tile.Position.Up() || tile2.Position == tile.Position.Left()))
        return true;
    else if (tile.CharRepresentation == '7' && (tile2.Position == tile.Position.Down() || tile2.Position == tile.Position.Left()))
        return true;
    else if (tile.CharRepresentation == 'F' && (tile2.Position == tile.Position.Down() || tile2.Position == tile.Position.Right()))
        return true;
    else if (tile.CharRepresentation == 'S')
    {
        return tile2.TraversibleNeighbours.Contains(tile);
    }
    else return false;
}

static char ReverseCharLookup(Tile tile)
{
    var up = tile.Position.Up();
    var left = tile.Position.Left();
    var right = tile.Position.Right();
    var down = tile.Position.Down();

    var n_pos = tile.TraversibleNeighbours.Select(w => w.Position).ToList();

    if (n_pos.Count != 2)
        throw new Exception();

    if (n_pos.Contains(up) && n_pos.Contains(down))
    {
        return '|';
    }
    else if (n_pos.Contains(left) && n_pos.Contains(right))
    {
        return '-';
    }
    else if (n_pos.Contains(up) && n_pos.Contains(right))
    {
        return 'L';
    }
    else if (n_pos.Contains(up) && n_pos.Contains(left))
    {
        return 'J';
    }
    else if (n_pos.Contains(down) && n_pos.Contains(left))
    {
        return '7';
    }
    else if (n_pos.Contains(down) && n_pos.Contains(right))
    {
        return 'F';
    }
    else throw new Exception();
}

class PipedTile : Tile
{
    public enum LoopStatus { OnTheLoop, Inside, Outside };

    private char tileChar;
    
    public override char CharRepresentation => tileChar;


    public PipedTile(int x, int y, char c, Func<Tile, IEnumerable<Tile>> fillTraversibleNeighboursFunc) 
        : base(x, y, c != '.', fillTraversibleNeighboursFunc)
    {
        this.tileChar = c;
    }

    public int DistanceToStart { get; set; } = int.MaxValue;

    public LoopStatus Status { get; set; } = LoopStatus.Inside;

    public void OverrideTileChar(char newChar)
    {
        this.tileChar = newChar;
    }
}