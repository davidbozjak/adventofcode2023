using System.Text.RegularExpressions;

var wholeStringInput = new StringInputProvider("Input.txt").ToList();

int maxRed = 12;
int maxGreen = 13;
int maxBlue = 14;

int sumOfPossibleGames = 0;
int sumOfPowerOfSets = 0;

foreach (var line in wholeStringInput)
{
    var parts = line.Split(':', ';');

    int gameId = int.Parse(parts[0][parts[0].IndexOf(' ')..]);

    bool possible = true;

    int minNumOfRed = 0, minNumOfBlue = 0, minNumOfGreen = 0;

    for (int i = 1; i < parts.Length; i++)
    {
        var gameSummary = parts[i];

        (int numOfRed, int numOfBlue, int numOfGreen) = GetGameInfo(gameSummary);

        if (numOfRed > minNumOfRed)
        {
            minNumOfRed = numOfRed;
        }

        if (numOfRed > maxRed)
        {
            possible = false;
        }

        if (numOfBlue > minNumOfBlue)
        {
            minNumOfBlue = numOfBlue;
        }

        if (numOfBlue > maxBlue)
        {
            possible = false;
        }

        if (numOfGreen > minNumOfGreen)
        {
            minNumOfGreen = numOfGreen;
        }

        if (numOfGreen > maxGreen)
        {
            possible = false;
        }
    }

    if (possible)
    {
        sumOfPossibleGames += gameId;
    }

    int power = minNumOfRed * minNumOfBlue * minNumOfGreen;

    sumOfPowerOfSets += power;
}

Console.WriteLine($"Part 1: {sumOfPossibleGames}");
Console.WriteLine($"Part 2: {sumOfPowerOfSets}");

(int numOfRed, int numOfBlue, int numOfGreen) GetGameInfo(string gameSummary)
{
    string digitsRegex = @"(\d+)";
    Regex greenRegex = new($"{digitsRegex} green");
    Regex redRegex = new($"{digitsRegex} red");
    Regex blueRegex = new($"{digitsRegex} blue");

    int numOfRed = int.Parse("0" + redRegex.Match(gameSummary).Groups[1].Value);
    int numOfGreen = int.Parse("0" + greenRegex.Match(gameSummary).Groups[1].Value);
    int numOfBlue = int.Parse("0" + blueRegex.Match(gameSummary).Groups[1].Value);

    return (numOfRed, numOfBlue, numOfGreen);
}