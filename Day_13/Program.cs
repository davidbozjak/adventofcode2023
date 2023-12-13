var parser = new MultiLineParser<Group>(() => new Group(), (g, v) => g.Add(v));
using var inputProvider = new InputProvider<Group?>("Input.txt", parser.AddLine)
{
    EndAtEmptyLine = false
};

var groups = inputProvider.Where(w => w != null).Cast<Group>().ToList();

Console.WriteLine($"Part 1: {groups.Sum(w => w.GetNumber())}");

Console.WriteLine($"Part 2: {groups.Sum(w => w.GetSmudgedNumber())}");

class Group
{
    private readonly List<string> horizontalLines = new List<string>();
    private readonly Cached<List<string>> cachedVerticalLinesLines;

    public Group()
    {
        this.cachedVerticalLinesLines = new Cached<List<string>>(() =>
        {
            var list = new List<string>();
            for (int i = 0; i < this.horizontalLines[0].Length; i++)
            {
                var verticalLine = new string(this.horizontalLines.Select(w => w[i]).ToArray());
                list.Add(verticalLine);
            }
            return list;
        });
    }

    public void Add(string value)
    {
        this.horizontalLines.Add(value);
        this.cachedVerticalLinesLines.Reset();
    }

    public void AddRange(IEnumerable<string> values)
    {
        this.horizontalLines.AddRange(values);
        this.cachedVerticalLinesLines.Reset();
    }

    public long GetSmudgedNumber()
    {
        var oldValue = this.GetNumber();

        for (int i = 0; i < this.horizontalLines.Count; i++)
        {
            var before = this.horizontalLines[..i];
            var after = this.horizontalLines[(i + 1)..];

            for (int x = 0; x < this.horizontalLines[0].Length; x++)
            {
                var line = this.horizontalLines[i];
                var modifiedLine = line[..x] + Swap(line[x]) + line[(x + 1)..];

                if (line.Length != modifiedLine.Length)
                    throw new Exception();

                var list = new List<string>();
                list.AddRange(before);
                list.Add(modifiedLine);
                list.AddRange(after);

                if (list.Count != this.horizontalLines.Count) 
                    throw new Exception();

                var group = new Group();
                group.AddRange(list);

                var number = group.GetNumber(oldValue);

                if (number != null)
                {
                    return number.Value;
                }
            }
        }

        throw new Exception();

        char Swap(char c)
        {
            if (c == '.')
                return '#';
            else if (c == '#')
                return '.';
            else throw new Exception();
        }
    }

    public long? GetNumber(long? numberToIgnore = null)
    {
        var horizontalLines = GetHorizontalLine();

        foreach (var horizontal in horizontalLines)
        {
            var value = (horizontal + 1) * 100;

            if (value != numberToIgnore)
            {
                return value;
            }
        }
        
        var verticalLines = GetVerticalLine();

        foreach (var vertical in verticalLines)
        {
            var value = vertical + 1;

            if (value != numberToIgnore)
            {
                return value;
            }
        }

        return null;
    }

    private IEnumerable<long> GetHorizontalLine()
    {
        return FindMirroredLine(this.horizontalLines);
    }

    private IEnumerable<long> GetVerticalLine()
    {
        return FindMirroredLine(this.cachedVerticalLinesLines.Value);
    }

    private IEnumerable<long> FindMirroredLine(List<string> lines)
    {
        for (int i = 0; i < lines.Count - 1; i++)
        {
            var line1 = lines[i];
            var line2 = lines[i + 1];

            if (line1 == line2)
            {
                // when we find the line, all the rest of the lines need to match
                if (ConfirmMirrorImage(i, lines))
                {
                    yield return i;
                }
            }
        }
    }

    private bool ConfirmMirrorImage(int mirroredLine, List<string> lines)
    {
        for (int i = mirroredLine, j = mirroredLine + 1; i >= 0 && j <= lines.Count - 1; i--, j++)
        {
            var line1 = lines[i];
            var line2 = lines[j];

            if (line1 != line2)
            {
                return false;
            }
        }

        return true;
    }
}