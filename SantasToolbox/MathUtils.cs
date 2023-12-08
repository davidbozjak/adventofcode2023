public static class MathUtils
{
    public static long GreatestCommonFactor(long a, long b)
    {
        while (b != 0)
        {
            long temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    public static long LeastCommonMultiple(long a, long b)
    {
        return (a / GreatestCommonFactor(a, b)) * b;
    }

    public static long LeastCommonMultiple(IEnumerable<long> numbers)
    {
        return numbers.Aggregate(LeastCommonMultiple);
    }
}