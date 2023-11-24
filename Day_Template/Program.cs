using System.Text.RegularExpressions;

Regex numRegex = new(@"-?\d+");
Regex hexColorRegex = new(@"#[0-9a-z][0-9a-z][0-9a-z][0-9a-z][0-9a-z][0-9a-z]");

var singleDigitIntParser = new SingleLineStringInputParser<int>(int.TryParse, str => str.ToCharArray().Select(w => w.ToString()).ToArray());
var singleDigitIntInput = new InputProvider<int>("Input.txt", singleDigitIntParser.GetValue).ToList();

var wholeStringConvertInput = new InputProvider<string?>("Input.txt", GetString).Where(w => w != null).Cast<string>().ToList();
var wholeStringInput = new StringInputProvider("Input.txt");

var commaSeperatedSingleLineParser = new SingleLineStringInputParser<int>(int.TryParse, str => str.Split(",", StringSplitOptions.RemoveEmptyEntries));
var commaSeperatedIntsInput = new InputProvider<int>("Input.txt", commaSeperatedSingleLineParser.GetValue).ToList();


static bool GetString(string? input, out string? value)
{
    value = null;

    if (input == null) return false;

    value = input ?? string.Empty;

    return true;
}