var commaSeperatedSingleLineParser = new SingleLineStringInputParser<string>(StringInputProvider.GetString, str => str.Split(",", StringSplitOptions.RemoveEmptyEntries));
var inputs = new InputProvider<string>("Input.txt", commaSeperatedSingleLineParser.GetValue).ToList();

Console.WriteLine($"Part 1: {inputs.Select(GetHash).Sum()}");

List<Lens>[] boxes = Enumerable.Range(0, 256).Select(w => new List<Lens>()).ToArray();

foreach (var input in inputs)
{
    if (input.Contains('='))
    {
        var parts = input.Split('=');
        if (parts.Length != 2)
            throw new Exception();

        var label = parts[0];
        var focalStrength = int.Parse(parts[1]);

        var newLens = new Lens(label, focalStrength);

        int boxIndex = GetHash(label);

        var box = boxes[boxIndex];

        var index = box.FindIndex(w => w.label == label);

        if (index == -1)
        {
            box.Add(newLens);
        }
        else
        {
            box[index] = newLens;
        }
    }
    else if (input.EndsWith('-'))
    {
        var label = input[..^1];

        int boxIndex = GetHash(label);

        var box = boxes[boxIndex];

        box.RemoveAll(w => w.label == label);
    }
    else throw new Exception();
}

int total = 0;

for (int i = 0; i < boxes.Length; i++)
{
    total += GetFocusingPowerOfBox(i, boxes[i]);
}

Console.WriteLine($"Part 2: {total}");

int GetFocusingPowerOfBox(int id, List<Lens> box)
{
    var focusingPower = 0;

    for (int i = 0; i < box.Count; i++)
    {
        var slotNumber = i + 1;
        focusingPower += (id+1) * slotNumber * box[i].focalStrength;
    }

    return focusingPower;
}

static int GetHash(string input)
{
    int hash = 0;
    foreach (var c in input)
    {
        hash += c;
        hash *= 17;
        hash %= 256;
    }
    return hash;
}

record Lens(string label, int focalStrength);