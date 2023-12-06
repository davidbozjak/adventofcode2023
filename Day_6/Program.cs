//test input, repalce with the real thing
List<(int time, int distance)> races = [(7, 9), (15, 40), (30, 200)];

List<long> waysToWin = new();

foreach ((int raceTime, int distanceToBeat) in races)
{
    long numberOfWaysToBeatTime = GetWaysToWin(raceTime, distanceToBeat);

    waysToWin.Add(numberOfWaysToBeatTime);
}

Console.WriteLine($"Part 1: {waysToWin.Aggregate((a, b) => a * b)}");

//test input, repalce with the real thing
var part2 = GetWaysToWin(71530, 940200);
Console.WriteLine($"Part 2: {part2}");

long GetWaysToWin(long raceTime, long distanceToBeat)
{
    long numberOfWaysToBeatTime = 0;
    for (int holdTime = 1; holdTime < raceTime; holdTime++)
    {
        var dsitance = (raceTime - holdTime) * holdTime;

        if (dsitance > distanceToBeat)
        {
            numberOfWaysToBeatTime++;
        }
    }

    return numberOfWaysToBeatTime;
}