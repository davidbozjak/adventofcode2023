using System.Text.RegularExpressions;

Regex numRegex = new(@"\d");

var stringInputs = new InputProvider<string?>("Input.txt", GetString).Where(w => w != null).Cast<string>().ToList();

Console.WriteLine("Part 1: " + ProcessInput(stringInputs, str => numRegex.Matches(str).Select(w => int.Parse(w.Value)).ToList()));
Console.WriteLine("Part 2: " + ProcessInput(stringInputs, GetFirstAndLastNumbers));

int ProcessInput(IEnumerable<string> input, Func<string, IEnumerable<int>> func)
{
    int totalSum = 0;

    foreach (var str in stringInputs)
    {
        var ints = func(str);

        int number = ints.First() * 10 + ints.Last();

        totalSum += number;
    }

    return totalSum;
}

IEnumerable<int> GetFirstAndLastNumbers(string str)
{
    var matches = new (string, int)[]
    { 
        ("1", 1), ("one", 1), 
        ("2", 2), ("two", 2), 
        ("3", 3), ("three", 3), 
        ("4", 4), ("four", 4), 
        ("5", 5), ("five", 5), 
        ("6", 6), ("six", 6),
        ("7", 7), ("seven", 7), 
        ("8", 8), ("eight", 8), 
        ("9", 9), ("nine", 9) };

    var remaining = matches.ToList();

    var firstIndexes = matches.Select(w => new { Index = str.IndexOf(w.Item1), Item = w.Item2 }).Where(w => w.Index > -1).OrderBy(w => w.Index).ToList();
    var lastIndexes = matches.Select(w => new { Index = str.LastIndexOf(w.Item1), Item = w.Item2 }).Where(w => w.Index > -1).OrderBy(w => w.Index).ToList();

    return new[] { firstIndexes.First().Item, lastIndexes.Last().Item };
}

static bool GetString(string? input, out string? value)
{
    value = null;

    if (input == null) return false;

    value = input ?? string.Empty;

    return true;
}