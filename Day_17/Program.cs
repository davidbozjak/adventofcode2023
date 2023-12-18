var inputStrings = new StringInputProvider("Input.txt").ToList();
var world = new TileWorld(inputStrings, false, GetTile);
var printer = new WorldPrinter();

//printer.Print(world);

var start = (CostTile)world.GetTileAt(0, 0);
var goal = (CostTile)world.GetTileAt(world.MaxX, world.MaxY);

var path = FindPath(start, goal, Part1Validation, (_, __, ___) => true);

Console.WriteLine($"Part 1: {path.Sum(w => w.Cost)}");

//var markedWorld = new WorldWithMarkings<CostTile>(world, '*', path);
//printer.Print(markedWorld);
//Console.WriteLine($"Part 1: {path.Sum(w => w.Cost)}");

path = FindPath(start, goal, Part2Validation, (_, __, countSameDirection) => countSameDirection >= 4);

//var markedWorld = new WorldWithMarkings<CostTile>(world, '*', path);
//printer.Print(markedWorld);
Console.WriteLine($"Part 2: {path.Sum(w => w.Cost)}");

Tile GetTile(int x, int y, char c, Func<Tile, IEnumerable<Tile>> func)
{
    return new CostTile(x, y, c, func);
}

bool Part1Validation(Direction direction, Direction? lastDirection, int countInSameDirection)
{
    if (direction == lastDirection && countInSameDirection > 2)
        return false;

    return true;
}

bool Part2Validation(Direction direction, Direction? lastDirection, int countInSameDirection)
{
    if (direction != lastDirection && countInSameDirection < 4)
    {
        return false;
    }
    else if (direction == lastDirection && countInSameDirection > 9)
    {
        return false;
    }

    return true;
}

List<CostTile> FindPath(CostTile start, CostTile goal, Func<Direction, Direction?, int, bool> validateNextFunc, Func<Tile, Direction?, int, bool> validateFinalStateFunc)
{
    PriorityQueue<State, int> queue = new();
    // part 1:
    //queue.Enqueue(new State(start, null, 0, 0), 0);
    // part 2:
    queue.Enqueue(new State(start, Direction.Right, 1, 0), 0);
    queue.Enqueue(new State(start, Direction.Down, 1, 0), 0);

    Dictionary<(Tile, Direction, int), int> minCostToHere = new Dictionary<(Tile, Direction, int), int>()
    {
        { (start, Direction.Down, 0) , 0 }
    };

    Dictionary<(CostTile, Direction?, int), (CostTile, Direction?, int)> cameFrom = new();

    var results = new List<(List<CostTile>, int)>();

    while (queue.Count > 0)
    {
        var current = queue.Dequeue();

        Direction? lastDirection = current.directionToHere;
        var currentState = (current.Tile, current.directionToHere, current.countSameDirection);

        if (current.Tile == goal && validateFinalStateFunc(currentState.Tile, currentState.directionToHere, currentState.countSameDirection))
        {
            //reconstruct path
            var list = new List<CostTile>();
            while (cameFrom.ContainsKey(currentState))
            {
                list.Add(currentState.Tile);
                currentState = cameFrom[currentState];
            }
            return list;
        }

        foreach (var next in current.Tile.TraversibleNeighbours)
        {
            var direction = GetDirection(next, current.Tile);

            if (AreOposite(direction, lastDirection))
                continue;

            int countSameDirection = direction == current.directionToHere ? current.countSameDirection + 1 : 1;

            if (!validateNextFunc(direction, lastDirection, currentState.countSameDirection))
            {
                continue;
            }

            // part 1
            //if (countSameDirection > 3)
            //{
            //    continue;
            //}

            // part 2:
            //if (direction != lastDirection && current.countSameDirection < 4)
            //{
            //    continue;
            //}
            //else if (countSameDirection > 10)
            //{
            //    continue;
            //}

            var key = ((CostTile)next, direction, countSameDirection);

            if (!minCostToHere.ContainsKey(key))
            {
                minCostToHere[key] = int.MaxValue;
                cameFrom[key] = currentState;
            }

            var cost = current.CostToHere + next.Cost;

            if (cost > minCostToHere[key])
                continue;

            minCostToHere[key] = cost;
            cameFrom[key] = currentState;

            queue.Enqueue(new State((CostTile)next, direction, countSameDirection, cost), cost + goal.Position.Distance(next.Position));
        }
    }

    throw new Exception();
}

Direction GetDirection(Tile current, Tile prev)
{
    if (!current.Position.IsNeighbour(prev.Position))
        throw new Exception();

    if (current.Position == prev.Position.Up())
        return Direction.Up;
    else if (current.Position == prev.Position.Down())
        return Direction.Down;
    else if (current.Position == prev.Position.Left())
        return Direction.Left;
    else if (current.Position == prev.Position.Right())
        return Direction.Right;
    else throw new Exception();
}

bool AreOposite(Direction dir1, Direction? dir2)
{
    if (dir2 == null)
        return false;

    return dir1 switch
    {
        Direction.Up => dir2 == Direction.Down,
        Direction.Down => dir2 == Direction.Up,
        Direction.Left => dir2 == Direction.Right,
        Direction.Right => dir2 == Direction.Left,
        _ => throw new Exception()
    };
}

record State(CostTile Tile, Direction? directionToHere, int countSameDirection, int CostToHere/*, List<CostTile> PathToHere*/);

enum Direction { Up, Down, Left, Right };

class CostTile : Tile, IEquatable<CostTile>
{
    private readonly char c;

    public override char CharRepresentation => this.c;

    public override int Cost { get; }

    public CostTile(int x, int y, char c, Func<Tile, IEnumerable<Tile>> fillTraversibleNeighboursFunc) 
        : base(x, y, true, fillTraversibleNeighboursFunc)
    {
        this.c = c;
        this.Cost = c - '0';
    }

    public bool Equals(CostTile? other)
    {
        return base.Equals(other);
    }
}