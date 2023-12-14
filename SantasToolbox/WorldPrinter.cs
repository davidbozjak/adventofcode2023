using System.Drawing;
using System.Text;

namespace SantasToolbox;

public interface IWorldObject
{
    Point Position { get; }

    char CharRepresentation { get; }

    int Z { get; }
}

public interface IWorld
{
    IEnumerable<IWorldObject> WorldObjects { get; }
}

public class WorldPrinter
{
    private readonly bool skipEmptyLines;
    private readonly bool clearScreenFirst;
    private readonly int frameSize;

    public WorldPrinter(bool skipEmptyLines = true, bool clearScreenFirst = true, int frameSize = 5)
    {
        this.skipEmptyLines = skipEmptyLines;
        this.clearScreenFirst = clearScreenFirst;
        this.frameSize = frameSize;
    }

    public void Print(IWorld world)
    {
        if (clearScreenFirst)
            Console.Clear();

        Print(world, Console.WriteLine);
    }

    public void Print(IWorld world, IWorldObject objectOfInterest)
    {
        if (clearScreenFirst)
            Console.Clear();

        Print(world, objectOfInterest, Console.WriteLine);
    }

    public void Print(IWorld world, int minX, int maxX, int minY, int maxY)
    {
        if (clearScreenFirst)
            Console.Clear();

        Print(world, minX, maxX, minY, maxY, Console.WriteLine);
    }

    public void PrintToFile(IWorld world, string filename)
    {
        using (var writer = new StreamWriter(filename))
        {
            Print(world, writer.WriteLine);
        }
    }

    public void PrintToFile(IWorld world, IWorldObject objectOfInterest, string filename)
    {
        using (var writer = new StreamWriter(filename))
        {
            Print(world, objectOfInterest, writer.WriteLine);
        }
    }

    public void PrintToFile(IWorld world, int minX, int maxX, int minY, int maxY, string filename)
    {
        using (var writer = new StreamWriter(filename))
        {
            Print(world, minX, maxX, minY, maxY, writer.WriteLine);
        }
    }

    public string PrintToString(IWorld world)
    {
        var stringBuilder = new StringBuilder();

        Print(world, w => stringBuilder.AppendLine(w));

        return stringBuilder.ToString();
    }

    private void Print(IWorld world, IWorldObject objectOfInterest, Action<string> printLineFunc)
    {
        Print(world, objectOfInterest.Position.X - frameSize, objectOfInterest.Position.X + frameSize, objectOfInterest.Position.Y - frameSize, objectOfInterest.Position.Y + frameSize, printLineFunc);
    }

    private void Print(IWorld world, Action<string> printLineFunc)
    {
        if (!world.WorldObjects.Any())
        {
            printLineFunc("<<< Blank World >>>");
            return;
        }

        int maxX = world.WorldObjects.Max(w => w.Position.X);
        int maxY = world.WorldObjects.Max(w => w.Position.Y);

        int minX = world.WorldObjects.Min(w => w.Position.X);
        int minY = world.WorldObjects.Min(w => w.Position.Y);

        Print(world, minX, maxX, minY, maxY, printLineFunc);
    }

    private void Print(IWorld world, int minX, int maxX, int minY, int maxY, Action<string> printLineFunc)
    {
        for (int y = minY; y <= maxY; y++)
        {
            var row = new StringBuilder(new string(Enumerable.Repeat(' ', (maxX - minX) + 1).ToArray()));

            foreach (var track in world.WorldObjects.Where(w => w.Position.Y == y && w.Position.X >= minX && w.Position.X <= maxX).OrderBy(w => w.Z))
            {
                row[track.Position.X - minX] = track.CharRepresentation;
            }

            if (!skipEmptyLines || !string.IsNullOrWhiteSpace(row.ToString()))
            {
                printLineFunc(row.ToString());
            }
        }
    }
}
