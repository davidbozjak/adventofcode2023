using System.Drawing;

namespace SantasToolbox;

public static class PointExtensions
{
    public static int Distance(this Point point, Point other)
    {
        return Math.Abs(point.X - other.X) + Math.Abs(point.Y - other.Y);
    }

    public static int ReadingOrder(this Point point)
    {
        return point.Y * 1000 + point.X;
    }

    public static Point Left(this Point point)
    {
        return new Point(point.X - 1, point.Y);
    }

    public static Point Right(this Point point)
    {
        return new Point(point.X + 1, point.Y);
    }

    public static Point Up(this Point point)
    {
        return new Point(point.X, point.Y - 1);
    }

    public static Point Down(this Point point)
    {
        return new Point(point.X, point.Y + 1);
    }

    public static bool IsNeighbour(this Point point, Point otherPoint)
    {
        if (point.X == otherPoint.X)
        {
            return Math.Abs(point.Y - otherPoint.Y) == 1;
        }
        else if (point.Y == otherPoint.Y)
        {
            return Math.Abs(point.X - otherPoint.X) == 1;
        }

        return false;
    }

    public static bool IsNeighbourWithDiagnoals(this Point point, Point otherPoint)
    {
        var absDiffX = Math.Abs(point.X - otherPoint.X);
        var absDiffY = Math.Abs(point.Y - otherPoint.Y);

        return absDiffX + absDiffY > 0 &&
            absDiffX < 2 &&
            absDiffY < 2;
    }
}
