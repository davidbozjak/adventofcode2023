using System.Text;
using System.Text.RegularExpressions;

var rows = new StringInputProvider("Input.txt").ToList();

Console.WriteLine($"Part 1: {GetCountForRows(rows)}");

rows = rows.Select(w => MultiplyInput(w, 5)).ToList();

Console.WriteLine($"Part 2: {GetCountForRows(rows)}");

long GetCountForRows(IEnumerable<string> rows)
{
    long countAll = 0;

    foreach (var row in rows)
    {
        var representation = row[..row.IndexOf(' ')];
        var instructions = GetInstructionsFromRow(row);

        var count = CountPossibilities(representation, instructions, 0, 0, 0, "", new Dictionary<string, long>());

        //Console.WriteLine($"{row} -- {count}");

        countAll += count;
    }

    return countAll;
}

string MultiplyInput(string row, int factor)
{
    var parts = row.Split(' ');

    var builder = new StringBuilder();

    for (int i = 0; i < factor; i++)
    {
        builder.Append(parts[0]);
        builder.Append('?');
    }
    builder.Remove(builder.Length - 1, 1);

    builder.Append(' ');

    for (int i = 0; i < factor; i++)
    {
        builder.Append(parts[1]);
        builder.Append(',');
    }
    builder.Remove(builder.Length - 1, 1);

    return builder.ToString();
}


List<int> GetInstructionsFromRow(string row)
{
    Regex numRegex = new(@"\d+");
    var instructions = numRegex.Matches(row).Select(w => int.Parse(w.Value)).ToList();
    return instructions;
}

long CountPossibilities(string str, List<int> instructions, int indexInStr, int indexInInstructions, int countOfBroken, string debugHelp, Dictionary<string, long> memoizationDict)
{
    var key = $"[{indexInStr}][{indexInInstructions}][{countOfBroken}]";

    if (!memoizationDict.ContainsKey(key))
    {
        memoizationDict[key] = CountPossibilities_Internal(str, instructions, indexInStr, indexInInstructions, countOfBroken, debugHelp, memoizationDict);
    }

    return memoizationDict[key];

    long CountPossibilities_Internal(string str, List<int> instructions, int indexInStr, int indexInInstructions, int countOfBroken, string debugHelp, Dictionary<string, long> memoizationDict)
    {
        if (indexInStr == str.Length)
        {
            if (indexInInstructions == instructions.Count)
            {
                return 1;
            }
            else if (indexInInstructions == instructions.Count - 1 && countOfBroken == instructions[indexInInstructions])
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        else
        {
            if (str[indexInStr] == '.')
            {
                if (countOfBroken == 0)
                {
                    return CountPossibilities(str, instructions, indexInStr + 1, indexInInstructions, 0, debugHelp + '.', memoizationDict);
                }
                else if (countOfBroken == instructions[indexInInstructions])
                {
                    return CountPossibilities(str, instructions, indexInStr + 1, indexInInstructions + 1, 0, debugHelp + '.', memoizationDict);
                }
                else
                {
                    return 0;
                }
            }
            else if (str[indexInStr] == '#')
            {
                if (indexInInstructions >= instructions.Count)
                {
                    return 0;
                }
                else if (countOfBroken == 0)
                {
                    return CountPossibilities(str, instructions, indexInStr + 1, indexInInstructions, 1, debugHelp + '#', memoizationDict);
                }
                else if (countOfBroken > instructions[indexInInstructions])
                {
                    return 0;
                }
                else
                {
                    return CountPossibilities(str, instructions, indexInStr + 1, indexInInstructions, countOfBroken + 1, debugHelp + '#', memoizationDict);
                }
            }
            else if (str[indexInStr] == '?')
            {
                long sum = 0;

                // pretend that it is a '.'
                if (countOfBroken == 0)
                {
                    sum += CountPossibilities(str, instructions, indexInStr + 1, indexInInstructions, 0, debugHelp + '.', memoizationDict);
                }
                else if (countOfBroken == instructions[indexInInstructions])
                {
                    sum += CountPossibilities(str, instructions, indexInStr + 1, indexInInstructions + 1, 0, debugHelp + '.', memoizationDict);
                }
                else
                {
                    sum += 0;
                }

                // pretend that it is a '#'
                if (indexInInstructions >= instructions.Count)
                {
                    sum += 0;
                }
                else if (countOfBroken == 0)
                {
                    sum += CountPossibilities(str, instructions, indexInStr + 1, indexInInstructions, 1, debugHelp + '#', memoizationDict);
                }
                else if (countOfBroken > instructions[indexInInstructions])
                {
                    sum += 0;
                }
                else
                {
                    sum += CountPossibilities(str, instructions, indexInStr + 1, indexInInstructions, countOfBroken + 1, debugHelp + '#', memoizationDict);
                }

                return sum;
            }
            else
            {
                throw new Exception();
            }
        }
    }
}