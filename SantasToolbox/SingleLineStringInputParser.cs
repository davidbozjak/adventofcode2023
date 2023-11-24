namespace SantasToolbox;

/// <summary>
/// A parser, to be used with <see cref="InputProvider"/>, when the input comes as a seperated line of many inputs 
/// </summary>
public class SingleLineStringInputParser<T>
{
    public delegate bool StringToTConverter(string? input, out T result);
    public delegate string[] StringSplitter(string? input);

    private readonly Queue<T> parserInputs = new();
    private readonly StringToTConverter converter;
    private readonly StringSplitter? splitter;

    public SingleLineStringInputParser(StringToTConverter converter)
        : this(converter, null)
    {
    }

    public SingleLineStringInputParser(StringToTConverter converter, StringSplitter? splitter)
    {
        this.converter = converter;
        this.splitter = splitter;
    }

    public bool GetValue(string? input, out T value)
    {
        value = default;

        if (string.IsNullOrWhiteSpace(input))
        {
            if (parserInputs.Count <= 0)
                return false;
        }
        else
        {
            string[] split;

            if (this.splitter != null)
            {
                split = this.splitter(input);
            }
            else
            {
                split = input.Split(',');
            }

            var itemsToAdd = split
                .Where(w => this.converter(w, out _) == true)
                .Select(w => { this.converter(w, out T value); return value; })
                .ToList();

            itemsToAdd.ForEach(w => this.parserInputs.Enqueue(w));
        }

        if (this.parserInputs.Count > 0)
        {
            value = this.parserInputs.Dequeue();
            return true;
        }
        else return false;
    }
}
