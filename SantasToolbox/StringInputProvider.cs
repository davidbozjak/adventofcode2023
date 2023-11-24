namespace SantasToolbox;

public class StringInputProvider : InputProvider<string>
{
    public StringInputProvider(string filePath)
        :base(filePath, GetString)
    { }

    static bool GetString(string? input, out string value)
    {
        value = string.Empty;

        if (string.IsNullOrWhiteSpace(input)) return false;

        value = input ?? string.Empty;

        return true;
    }
}