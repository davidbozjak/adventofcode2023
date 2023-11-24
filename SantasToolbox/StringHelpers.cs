using System.Text;

namespace SantasToolbox;

public static class StringHelpers
{
    public static StringBuilder RemoveAllOccurrencesOfChar(this StringBuilder stringBuilder, char charToRemove)
    {
        for (int i = 0; i < stringBuilder.Length; i++)
        {
            if (stringBuilder[i] == charToRemove)
            {
                stringBuilder.Remove(i, 1);
                i--;
            }
        }

        return stringBuilder;
    }

    /// <summary>
    /// Calculates Levenshtein Distance between strings
    /// </summary>
    public static int GetDistanceToString(this string str, string targetString)
    {
        // from (credit) https://www.csharpstar.com/csharp-string-distance-algorithm/

        int n = str.Length;
        int m = targetString.Length;
        int[,] d = new int[n + 1, m + 1];

        // Step 1
        if (n == 0)
        {
            return m;
        }

        if (m == 0)
        {
            return n;
        }

        // Step 2
        for (int i = 0; i <= n; d[i, 0] = i++)
        {
        }

        for (int j = 0; j <= m; d[0, j] = j++)
        {
        }

        // Step 3
        for (int i = 1; i <= n; i++)
        {
            //Step 4
            for (int j = 1; j <= m; j++)
            {
                // Step 5
                int cost = (targetString[j - 1] == str[i - 1]) ? 0 : 1;

                // Step 6
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost);
            }
        }
        // Step 7
        return d[n, m];
    }
}
