using System.Text.RegularExpressions;

Regex numRegex = new(@"\d+");

var input = new StringInputProvider("Input.txt").ToList();

var numbersNextToSymbol = new List<int>();

var gearDict = new Dictionary<(int x, int y), List<int>>();

for (int y = 0; y < input.Count; y++)
{
    var line = input[y];
    
    var numbers = numRegex.Matches(line).Select(w => new { Line = y, Str = w.Value, Num = int.Parse(w.Value), Index = w.Index }).ToList();

    foreach (var number in numbers)
    {
        int pos = number.Index;

        for (int j = 0; j < number.Str.Length; j++)
        {
            int x = pos + j;

            var surroundingSymbols = GetNeighbouringValues(x, y)
                .Where(w => IsSymbol(GetCharAt(w)))
                .Select(w => new { Symbol = GetCharAt(w), Position = w })
                .ToList();

            if (surroundingSymbols.Any())
            {
                numbersNextToSymbol.Add(number.Num);
                
                foreach (var gear in surroundingSymbols.Where(w => w.Symbol == '*'))
                {
                    if (!gearDict.ContainsKey(gear.Position))
                    {
                        gearDict[gear.Position] = new List<int>();
                    }
                    gearDict[gear.Position].Add(number.Num);
                }

                break;
            }
        }
    }
}

Console.WriteLine($"Part 1: {numbersNextToSymbol.Sum()}");

Console.WriteLine($"Part 2: {gearDict.Values.Where(w => w.Count == 2).Select(w => w[0] * w[1]).Sum()}");

IEnumerable<(int x, int y)> GetNeighbouringValues(int x, int y)
{
    if (y > 0)
    {
        if (x > 0) yield return (x - 1, y - 1);
        yield return (x, y - 1);
        if (x < input[0].Length - 1) yield return (x + 1, y - 1);
    }

    if (x > 0) yield return (x - 1, y);
    yield return (x, y);
    if (x < input[0].Length - 1) yield return (x + 1, y);

    if (y < input.Count - 1)
    {
        if (x > 0) yield return (x - 1, y + 1);
        yield return (x, y + 1);
        if (x < input[0].Length - 1) yield return (x + 1, y + 1);
    }
}

char GetCharAt((int x, int y) value)
{
    return input[value.y][value.x];
}

bool IsSymbol(char c)
{
    return !char.IsDigit(c) && c != '.';
}

void PrintSurroundingArea(int x, int y, int length)
{
    for (int yy = y - 1, c1 = 2; c1 >= 0; c1--, yy++)
    {
        if (yy < 0 || yy >= input.Count) continue;

        for (int xx = x - 1, c2 = length + 1; c2 >= 0; c2--, xx++)
        {
            if (xx < 0 || xx >= input[0].Length) continue;

            Console.Write(GetCharAt((xx, yy)));
        }

        Console.WriteLine();
    }

    Console.WriteLine("Any key to continue");
    Console.ReadKey();
}