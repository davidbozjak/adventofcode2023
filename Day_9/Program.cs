using System.Text.RegularExpressions;

var allSeries = new InputProvider<List<int>?>("Input.txt", GetSeries).Where(w => w != null).Cast<List<int>>().ToList();

List<int> predictions = [];
List<int> historicalPredictions = [];

foreach (var series in allSeries)
{
    List<List<int>> diffs = [series.ToList()];

    while (diffs.Last().Any(w => w != 0))
    {
        var diff = new List<int>();
        for (int i = 1; i < diffs.Last().Count; i++)
        {
            diff.Add(diffs.Last()[i] - diffs.Last()[i - 1]);
        }
        diffs.Add(diff);
    }

    diffs.Last().Add(0);

    for (int i = diffs.Count - 2; i >= 0; i--)
    {
        diffs[i].Add(diffs[i].Last() + diffs[i + 1].Last());
    }

    predictions.Add(diffs[0].Last());

    diffs.Last().Insert(0, 0);

    for (int i = diffs.Count - 2; i >= 0; i--)
    {
        diffs[i].Insert(0, diffs[i].First() - diffs[i + 1].First());
    }

    historicalPredictions.Add(diffs[0].First());
}

Console.WriteLine($"Part 1: {predictions.Sum()}");
Console.WriteLine($"Part 2: {historicalPredictions.Sum()}");

static bool GetSeries(string? input, out List<int>? value)
{
    value = null;

    if (input == null) return false;

    Regex numRegex = new(@"-?\d+");

    value = numRegex.Matches(input).Select(w => int.Parse(w.Value)).ToList();

    return true;
}