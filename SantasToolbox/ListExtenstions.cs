namespace SantasToolbox;

public static class ListExtenstions
{
    public static bool AddIfNotNull<T>(this List<T> list, T? element)
        where T : class
    {
        if (element == null)
        {
            return false;
        }

        list.Add(element);
        return true;
    }

    public static IEnumerable<IList<T>> PermuteList<T>(this IList<T> sequence)
    {
        return Permute(sequence, 0, sequence.Count);

        static IEnumerable<IList<T>> Permute(IList<T> sequence, int k, int m)
        {
            if (k == m)
            {
                yield return sequence;
            }
            else
            {
                for (int i = k; i < m; i++)
                {
                    SwapPlaces(sequence, k, i);

                    foreach (var newSquence in Permute(sequence, k + 1, m))
                    {
                        yield return newSquence;
                    }

                    SwapPlaces(sequence, k, i);
                }
            }
        }

        static void SwapPlaces(IList<T> sequence, int indexA, int indexB)
        {
            T temp = sequence[indexA];
            sequence[indexA] = sequence[indexB];
            sequence[indexB] = temp;
        }
    }

    public static IEnumerable<IList<T>> GetAllOrdersOfList<T>(this IList<T> sequence)
    {
        if (sequence.Count == 1) yield return sequence;

        foreach (var element in sequence)
        {
            var list = new List<T>();
            var listWithoutElement = sequence.ToList();
            listWithoutElement.Remove(element);
            list.Add(element);

            foreach (var subsequence in listWithoutElement.GetAllOrdersOfList())
            {
                var copyList = list.ToList();

                copyList.AddRange(subsequence);
                yield return copyList;
            }

        }
    }

    /// <summary>
    /// Returns a sliding window elements *ending* with element on index lastIndexOfWindow.
    /// </summary>
    /// <param name="input">The list to perform the operation on</param>
    /// <param name="lastIndexOfWindow">The index of the last element to be included</param>
    /// <param name="windowLength">The width of the window</param>
    /// <returns>A new sublist containing the elements from the original list</returns>
    public static IList<T> GetSlidingWindow<T>(this IList<T> input, int lastIndexOfWindow, int windowLength)
    {
        int skip = lastIndexOfWindow - windowLength + 1;
        if (skip < 0)
        {
            windowLength = windowLength + skip;
            skip = 0;
        }

        return input.Skip(skip).Take(windowLength).ToList();
    }

    /// <summary>
    /// Returns a sliding window sum for a sublist *ending* with element on index poz.
    /// </summary>
    /// <param name="input">The list to perform the operation on</param>
    /// <param name="lastIndexOfWindow">The index of the last element to be included</param>
    /// <param name="windowLength">The width of the window</param>
    /// <returns>A sum of the elements</returns>
    public static long GetSlidingWindowSum<T>(this IList<T> input, int lastIndexOfWindow, int windowLength)
    {
        return input.GetSlidingWindow(lastIndexOfWindow, windowLength)
            .Sum(w => Convert.ToInt64(w));
    }
}
