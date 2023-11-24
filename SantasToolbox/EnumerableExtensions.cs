namespace SantasToolbox;

public static class EnumerableExtensions
{
    /// <summary>
    /// Provides all different sublists of k elements from enumerable
    /// </summary>
    /// <param name="k"></param>
    /// <returns></returns>
    public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> elements, int k)
    {
        return k == 0 ? new[] { Array.Empty<T>() } :
          elements.SelectMany((e, i) =>
            elements.Skip(i + 1).Combinations(k - 1).Select(c => (new[] { e }).Concat(c)));
    }

    public static T GetMostFrequentElement<T>(this IEnumerable<T> elements)
    {
        return elements.GroupBy(w => w).OrderByDescending(w => w.Count()).Select(w => w.Key).First();
    }
}
