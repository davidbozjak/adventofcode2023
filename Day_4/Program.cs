using System.Text.RegularExpressions;

var input = new InputProvider<Ticket?>("Input.txt", GetTicket).Where(w => w != null).Cast<Ticket>().ToList();

var dict = input.ToDictionary(w => w.Id, w => w);

Console.WriteLine($"Part 1: {input.Sum(w => w.Score)}");

var startingTicket = input[0];

int allTicketCount = 0;

foreach (var ticket in input)
{
    allTicketCount += ticket.GetTotalFollowupCards(dict);
}

Console.WriteLine($"Part 2: {allTicketCount}");

static bool GetTicket(string? input, out Ticket? value)
{
    value = null;

    if (input == null) return false;

    Regex numRegex = new(@"-?\d+");

    var parts = input.Split(':', '|');

    var id = int.Parse(numRegex.Match(parts[0]).Value);
    var numbers = numRegex.Matches(parts[1]).Select(w => int.Parse(w.Value));
    var winningNumbers = numRegex.Matches(parts[2]).Select(w => int.Parse(w.Value));

    value = new Ticket(id, numbers, winningNumbers);

    return true;
}

class Ticket
{
    private static Dictionary<int, int> memoic = new Dictionary<int, int>();

    public int Id { get; }

    public int Score => cachedScore.Value;

    public int AmountMatching => cachedNum.Value;

    public HashSet<int> WinningNumbers { get; }
    public HashSet<int> Numbers { get; }

    private Cached<int> cachedScore;
    private Cached<int> cachedNum;

    public Ticket(int id, IEnumerable<int> numbers, IEnumerable<int> winningNumbers)
    {
        this.Id = id;
        this.Numbers = numbers.ToHashSet();
        this.WinningNumbers = winningNumbers.ToHashSet();

        this.cachedScore = new Cached<int>(GetScore);
        this.cachedNum = new Cached<int>(GetNumberMatching);
    }

    public IEnumerable<int> GetCopiesIndices()
    {
        for (int i = 1; i <= AmountMatching; i++)
            yield return this.Id + i;
    }

    public int GetTotalFollowupCards(Dictionary<int, Ticket> allCardsDict)
    {
        if (!memoic.ContainsKey(this.Id))
        {
            memoic[this.Id] = GetTotalFollowupCards_Rec(allCardsDict);
        }

        return memoic[this.Id];

        int GetTotalFollowupCards_Rec(Dictionary<int, Ticket> allCardsDict)
        {
            int sum = 1;

            foreach (var id in this.GetCopiesIndices())
            {
                var card = allCardsDict[id];
                sum += card.GetTotalFollowupCards(allCardsDict);
            }

            return sum;
        }
    }

    private int GetNumberMatching()
    {
        int count = WinningNumbers.Count(w => Numbers.Contains(w));

        return count;
    }

    private int GetScore()
    {
        return (int)Math.Pow(2, AmountMatching - 1);
    }
}