using System.Diagnostics;

namespace SantasToolbox;

[DebuggerDisplay("{Start} - {End} Length: {Length}")]
public class ClosedInterval
{
    public int Start { get; }
    public int End { get; }

    public int Length => this.End - this.Start + 1;

    public double CenterPoint => this.Start + (this.Length / 2.0);

    public ClosedInterval(int start, int end)
    {
        Start = Math.Min(start, end);
        End = Math.Max(start, end);
    }

    public bool HasIntersect(ClosedInterval other)
    {
        if (this.End < other.Start) return false;
        if (this.Start > other.End) return false;

        return true;
    }

    public bool CoversWholeInterval(ClosedInterval other)
    {
        return other.Start >= this.Start && other.End <= this.End;
    }

    public bool ContainsPoint(int value)
    {
        return value >= this.Start && value <= this.End;
    }

    public ClosedInterval Union(ClosedInterval other)
    {
        if (!HasIntersect(other)) throw new Exception();

        return new ClosedInterval(Math.Min(this.Start, other.Start), Math.Max(this.End, other.End));
    }

    public ClosedInterval Intersect(ClosedInterval other)
    {
        if (!HasIntersect(other))
            throw new Exception();

        return new ClosedInterval(Math.Max(this.Start, other.Start), Math.Min(this.End, other.End));
    }

    public override string ToString()
    {
        return $"{Start} - {End} Length: {Length}";
    }
}