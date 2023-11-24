namespace SantasToolbox;

public class MultiLineParser<T>
    where T : class
{
    private readonly Func<T> constructorFunc;
    private readonly Action<T, string> additionFunc;

    private T? currentGroup = null;

    public MultiLineParser(Func<T> constructorFunc, Action<T, string> additionFunc)
    {
        this.constructorFunc = constructorFunc;
        this.additionFunc = additionFunc;
    }

    public IEnumerable<T> AddRange(IEnumerator<string?> input)
    {
        var list = new List<T>();

        input.MoveNext();
        for (T? value; AddLine(input.Current, out value); input.MoveNext())
        {
            if (value != null)
            {
                list.Add(value);
            }
        }

        return list;
    }

    /// <summary>
    /// Adds line and assigns a object to out param value if the group is complete
    /// </summary>
    /// <param name="input">The input to add</param>
    /// <param name="value">Returned value if group is complete</param>
    /// <returns>A boolean value indicating if iteration over input should continue</returns>
    public bool AddLine(string? input, out T? value)
    {
        value = null;

        if (input == null)
        {
            if (currentGroup != null)
            {
                value = currentGroup;
                currentGroup = null;
                return true;
            }
            else return false;
        }

        if (string.IsNullOrWhiteSpace(input) && currentGroup == null)
            return false;

        if (string.IsNullOrWhiteSpace(input) && currentGroup != null)
        {
            value = currentGroup;
            currentGroup = null;
            return true;
        }

        if (currentGroup == null)
        {
            currentGroup = this.constructorFunc();
        }

        this.additionFunc(this.currentGroup, input);

        return true;
    }
}
