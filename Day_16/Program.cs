var inputStrings = new StringInputProvider("Input.txt").ToList();

Console.WriteLine($"Part 1: {Iluminate(0, 0, Direction.East, inputStrings).Count}");

int maxY = inputStrings.Count - 1;
int maxX = inputStrings[0].Length - 1;

int max = int.MinValue;

for (int y = 0; y < maxY; y++)
{
    var left = Iluminate(0, y, Direction.East, inputStrings);

    if (left.Count > max)
    {
        max = left.Count;
    }

    var right = Iluminate(maxX, y, Direction.West, inputStrings);

    if (right.Count > max)
    {
        max = right.Count;
    }
}

for (int x = 0; x < maxX; x++)
{
    var up = Iluminate(x, 0, Direction.South, inputStrings);

    if (up.Count > max)
    {
        max = up.Count;
    }

    var down = Iluminate(x, maxY, Direction.North, inputStrings);

    if (down.Count > max)
    {
        max = down.Count;
    }
}

Console.WriteLine($"Part 2: {max}");

List<EnergizableTile> Iluminate(int x, int y, Direction direction, IEnumerable<string> inputStrings)
{
    var world = new TileWorld(inputStrings, false, GetTile);
    var printer = new WorldPrinter();

    var beams = new Stack<LightBeam>();
    beams.Push(new LightBeam(world.GetTileAt(x, y), direction));

    HashSet<LightBeam> visited = new();

    while (beams.Count > 0)
    {
        var beam = beams.Pop();

        if (visited.Contains(beam))
        {
            continue;
        }

        visited.Add(beam);

        var tile = world.GetTileAt(beam.Current.Position) as EnergizableTile;

        if (tile == null)
            throw new Exception();

        foreach (var newBeam in tile.Flash(beam, world))
        {
            beams.Push(newBeam);
        }
    }

    return world.WorldObjects.Cast<EnergizableTile>().Where(w => w.IsEnergized).ToList();
}

Tile GetTile(int x, int y, char c, Func<Tile, IEnumerable<Tile>> func)
{
    if (c == '.')
    {
        return new EnergizableTile(x, y, func);
    }
    else
    {
        return new MirrorTile(x, y, c, func);
    }
}

enum Direction { North, East, West, South};

record LightBeam(Tile Current, Direction Direction);

class EnergizableTile : Tile
{
    public bool IsEnergized { get; protected set; }

    public EnergizableTile(int x, int y, Func<Tile, IEnumerable<Tile>> fillTraversibleNeighboursFunc) 
        : base(x, y, true, fillTraversibleNeighboursFunc)
    {
    }

    public virtual IEnumerable<LightBeam> Flash(LightBeam beam, TileWorld world)
    {
        this.IsEnergized = true;

        var tile = beam.Direction switch
        {
            Direction.North => world.GetTileAtOrNull(beam.Current.Position.Up()),
            Direction.West => world.GetTileAtOrNull(beam.Current.Position.Left()),
            Direction.East => world.GetTileAtOrNull(beam.Current.Position.Right()),
            Direction.South => world.GetTileAtOrNull(beam.Current.Position.Down()),
            _ => throw new Exception()
        };

        if (tile == null)
            yield break;

        yield return new LightBeam(tile, beam.Direction);
    }
}

class MirrorTile : EnergizableTile
{
    private readonly char c;

    public override char CharRepresentation => this.c;

    public MirrorTile(int x, int y, char c, Func<Tile, IEnumerable<Tile>> fillTraversibleNeighboursFunc) 
        : base(x, y, fillTraversibleNeighboursFunc)
    {
        this.c = c;
    }

    public override IEnumerable<LightBeam> Flash(LightBeam beam, TileWorld world)
    {
        this.IsEnergized = true;

        if (this.c == '/')
        {
            (Tile? tile, Direction direction) = beam.Direction switch
            {
                Direction.North => (world.GetTileAtOrNull(beam.Current.Position.Right()), Direction.East),
                Direction.West => (world.GetTileAtOrNull(beam.Current.Position.Down()), Direction.South),
                Direction.East => (world.GetTileAtOrNull(beam.Current.Position.Up()), Direction.North),
                Direction.South => (world.GetTileAtOrNull(beam.Current.Position.Left()), Direction.West),
                _ => throw new Exception()
            };

            if (tile != null)
            {
                yield return new LightBeam(tile, direction);
            }
        }
        else if (this.c == '\\')
        {
            (Tile? tile, Direction direction) = beam.Direction switch
            {
                Direction.North => (world.GetTileAtOrNull(beam.Current.Position.Left()), Direction.West),
                Direction.West => (world.GetTileAtOrNull(beam.Current.Position.Up()), Direction.North),
                Direction.East => (world.GetTileAtOrNull(beam.Current.Position.Down()), Direction.South),
                Direction.South => (world.GetTileAtOrNull(beam.Current.Position.Right()), Direction.East),
                _ => throw new Exception()
            };

            if (tile != null)
            {
                yield return new LightBeam(tile, direction);
            }
        }
        else if (this.c == '-')
        {
            if (beam.Direction == Direction.East || beam.Direction == Direction.West)
            {
                foreach (var newBeam in base.Flash(beam, world))
                {
                    yield return newBeam;
                }
            }
            else
            {
                var tile = world.GetTileAtOrNull(beam.Current.Position.Left());

                if (tile != null)
                {
                    yield return new LightBeam(tile, Direction.West);
                }

                tile = world.GetTileAtOrNull(beam.Current.Position.Right());

                if (tile != null)
                {
                    yield return new LightBeam(tile, Direction.East);
                }
            }
        }
        else if (this.c == '|')
        {
            if (beam.Direction == Direction.North || beam.Direction == Direction.South)
            {
                foreach (var newBeam in base.Flash(beam, world))
                {
                    yield return newBeam;
                }
            }
            else
            {
                var tile = world.GetTileAtOrNull(beam.Current.Position.Up());

                if (tile != null)
                {
                    yield return new LightBeam(tile, Direction.North);
                }

                tile = world.GetTileAtOrNull(beam.Current.Position.Down());

                if (tile != null)
                {
                    yield return new LightBeam(tile, Direction.South);
                }
            }
        }
        else throw new Exception();
    }
}