using System.Drawing;
using System.Globalization;

var instructions = new InputProvider<Instruction?>("Input.txt", GetInstruction).Where(w => w != null).Cast<Instruction>().ToList();

var part1Lines = GetLinesFromInput(instructions);
Console.WriteLine($"Part 1: {GetVolume(part1Lines, printProgress: false)}");

instructions = new InputProvider<Instruction?>("Input.txt", GetInstructionFlipped).Where(w => w != null).Cast<Instruction>().ToList();
var part2Lines = GetLinesFromInput(instructions);
Console.WriteLine($"Part 2: {GetVolume(part2Lines, printProgress: false)}");

static List<Line> GetLinesFromInput(List<Instruction> instructions)
{
    var current = new Point(0, 0);

    var lines = new List<Line>();

    foreach (var instruction in instructions)
    {
        if (instruction.Direction == 'L')
        {
            var newPoz = new Point(current.X - instruction.Steps, current.Y);
            lines.Add(new Line(newPoz.X, current.X, current.Y, true));
            current = newPoz;
        }
        else if (instruction.Direction == 'R')
        {
            var newPoz = new Point(current.X + instruction.Steps, current.Y);
            lines.Add(new Line(current.X, newPoz.X, current.Y, true));
            current = newPoz;
        }
        else if (instruction.Direction == 'U')
        {
            var newPoz = new Point(current.X, current.Y - instruction.Steps);
            lines.Add(new Line(newPoz.Y, current.Y, current.X, false));
            current = newPoz;
        }
        else if (instruction.Direction == 'D')
        {
            var newPoz = new Point(current.X, current.Y + instruction.Steps);
            lines.Add(new Line(current.Y, newPoz.Y, current.X, false));
            current = newPoz;
        }
        else throw new Exception();
    }

    return lines;
}

static long GetVolume(List<Line> lines, bool debugPaint = false, bool printProgress = false)
{
    var printer = new WorldPrinter();
    var filledInWorld = new TileWorld(Enumerable.Empty<string>(), false, GetTile);

    var minY = lines.Where(w => w.isHorizontal).Min(w => w.StaticDimension);
    var maxY = lines.Where(w => w.isHorizontal).Max(w => w.StaticDimension);

    long count = 0;
    double prevPercent = -1;
    for (long y = minY; y <= maxY; y++)
    {
        if (printProgress)
        {
            var percentDone = Math.Round((double)(y - minY) / (maxY - minY) * 100.0, 0);
            if (percentDone != prevPercent)
            {
                Console.WriteLine($"{DateTime.Now.TimeOfDay}: {percentDone}% done");
                prevPercent = percentDone;
            }
        }

        var linesOnY = lines.Where(w => w.IsOnY(y)).ToList();

        var verticalLines = linesOnY.Where(w => !w.isHorizontal).ToList();
        linesOnY.RemoveAll(verticalLines.Contains);

        foreach (var verticalLine in verticalLines)
        {
            if (linesOnY.Any(w => w.ContainsPoint(verticalLine.StaticDimension, y)))
                continue;

            linesOnY.Add(new Line(verticalLine.StaticDimension, verticalLine.StaticDimension, y, true));
        }

        linesOnY = linesOnY.OrderBy(w => w.Start).ToList();

        bool inside = false;
        for (int i = 0; i < linesOnY.Count; i++)
        {
            var line = linesOnY[i];

            if (inside)
            {
                var prevLine = linesOnY[i - 1];
                var contained = new Line(prevLine.End, line.Start, y, true);
                count += contained.Length - 1; //-1 because line.Start will be counted in the next section as well
                if (debugPaint) PaintLine(filledInWorld, contained);
            }

            if (debugPaint) PaintLine(filledInWorld, line);
            count += line.Length + 1;

            if (line.Length <= 1)
            {
                inside = !inside;
            }
            else
            {
                bool isLJ = ContainsPoint(line.Start, y - 1) && ContainsPoint(line.End, y - 1);
                bool isF7 = ContainsPoint(line.Start, y + 1) && ContainsPoint(line.End, y + 1);

                if (isLJ || isF7)
                {
                    //don't flip, do nothing
                }
                else
                {
                    inside = !inside;
                }
            }
        }
    }

    if (debugPaint)
    {
        if (count != filledInWorld.WorldObjects.Count())
            throw new Exception();

        printer.PrintToFile(filledInWorld, "filledoutput.txt");
    }

    return count;

    bool ContainsPoint(long x, long y)
    {
        return lines.Any(w => w.ContainsPoint(x, y));
    }

    void PaintLine(TileWorld world, Line interval)
    {
        if (interval.isHorizontal)
        {
            for (long x = interval.Start; x <= interval.End; x++)
            {
                world.GetOrCreateTileAt((int)x, (int)interval.StaticDimension);
            }
        }
        else
        {
            for (long y = interval.Start; y <= interval.End; y++)
            {
                world.GetOrCreateTileAt((int)interval.StaticDimension, (int)y);
            }
        }
    }

    static Tile GetTile(int x, int y, char c, Func<Tile, IEnumerable<Tile>> func)
    {
        return new Tile(x, y, true, func);
    }
}

static bool GetInstruction(string? input, out Instruction? value)
{
    value = null;

    if (input == null) return false;

    var parts = input.Split(" ");

    value = new Instruction(parts[0][0], int.Parse(parts[1]), parts[2]);

    return true;
}

static bool GetInstructionFlipped(string? input, out Instruction? value)
{
    value = null;

    if (input == null) return false;

    var parts = input.Split(" ");

    var steps = int.Parse(parts[2][2..^2], NumberStyles.HexNumber);
    var direction = parts[2][7] switch
    {
        '0' => 'R',
        '1' => 'D',
        '2' => 'L',
        '3' => 'U',
        _ => throw new Exception()
    };

    value = new Instruction(direction, steps, "");

    return true;
}

record Instruction (char Direction, int Steps, string color);

record Line(long Start, long End, long StaticDimension, bool isHorizontal)
{
    public long Length => End - Start;

    public bool IsOnY(long y)
    {
        if (isHorizontal)
        {
            return StaticDimension == y;
        }
        else
        {
            return y >= Start && y <= End;
        }
    }

    public bool ContainsPoint(long x, long y)
    {
        if (isHorizontal)
        {
            if (y != StaticDimension)
                return false;

            return x >= Start && x <= End;
        }
        else
        {
            if (x != StaticDimension)
                return false;

            return y >= Start && y <= End;
        }
    }
}