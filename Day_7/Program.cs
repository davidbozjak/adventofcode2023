using System.Text.RegularExpressions;

var hands = new InputProvider<CardHand?>("Input.txt", GetCardHand).Where(w => w != null).Cast<CardHand>().ToList();

hands.Sort();

Console.WriteLine($"Part 1: {GetTotalWinnings()}");

CardHand.AllowJokers = true;
hands = new InputProvider<CardHand?>("Input.txt", GetCardHand).Where(w => w != null).Cast<CardHand>().ToList();
hands.Sort();

Console.WriteLine($"Part 2: {GetTotalWinnings()}");

long GetTotalWinnings()
{
    long totalWinnings = 0;

    for (int i = 0, rank = 1; i < hands.Count; i++, rank++)
    {
        totalWinnings += hands[i].Bid * rank;
    }

    return totalWinnings;
}

static bool GetCardHand(string? input, out CardHand? value)
{
    value = null;

    if (input == null) return false;

    Regex numRegex = new(@"-?\d+");

    var parts = input.Split(" ");
    var hand = parts[0];
    var bid = int.Parse(parts[1]);

    value = new CardHand(hand, bid);

    return true;
}

public enum CardHandType
{
    FiveOfAKind = 7,
    FourOfAKind = 6,
    FullHouse = 5,
    ThreeOfAKind = 4,
    TwoPair = 3,
    OnePair = 2,
    HighCard = 1
}

class CardHand : IComparable<CardHand>
{
    public static bool AllowJokers = false;

    private readonly Cached<CardHandType> cachedType;

    public string Hand { get; }

    public int Bid { get; }

    public CardHandType Type => this.cachedType.Value;

    public CardHand(string hand, int bid)
    {
        if (hand.Length != 5)
            throw new Exception();

        if (bid <= 0)
            throw new Exception();

        this.Hand = hand;
        this.Bid = bid;

        this.cachedType = new Cached<CardHandType>(GetHandType);
    }

    private CardHandType GetHandType()
    {
        var cardMap = Hand.GroupBy(w => w).ToDictionary(w => w.Key, w => w.Count());
        bool hasJoker = cardMap.Keys.Any(w => w == 'J');

        if (!AllowJokers || !hasJoker)
        {
            return GetType(Hand);
        }
        else
        {
            CardHandType max = CardHandType.HighCard;
            foreach (char c in cardMap.Keys)
            {
                var modifiedHand = Hand.Replace('J', c);

                var type = GetType(modifiedHand);

                if ((int)type > (int)max)
                {
                    max = type;
                }
            }

            return max;
        }

        static CardHandType GetType(string hand)
        {
            var cardMap = hand.GroupBy(w => w).ToDictionary(w => w.Key, w => w.Count());
            var values = cardMap.Values.OrderByDescending(w => w).ToList();

            if (values[0] == 5)
                return CardHandType.FiveOfAKind;
            else if (values[0] == 4)
                return CardHandType.FourOfAKind;
            else if (values[0] == 3 && values[1] == 2)
                return CardHandType.FullHouse;
            else if (values[0] == 3)
                return CardHandType.ThreeOfAKind;
            else if (values[0] == 2 && values[1] == 2)
                return CardHandType.TwoPair;
            else if (values[0] == 2)
                return CardHandType.OnePair;
            else return CardHandType.HighCard;
        }
    }

    public int GetCardStrength(int cardIndex)
    {
        if (cardIndex < 0 || cardIndex >= Hand.Length)
            throw new Exception("invalid index");

        return Hand[cardIndex] switch
        {
            'A' => 14,
            'K' => 13,
            'Q' => 12,
            'J' => AllowJokers ? 1 : 11,
            'T' => 10,
            '9' => 9,
            '8' => 8,
            '7' => 7,
            '6' => 6,
            '5' => 5,
            '4' => 4,
            '3' => 3,
            '2' => 2,
            _ => throw new Exception()
        };
    }

    public int CompareTo(CardHand? other)
    {
        if (other == null)
            return -1;

        if ((int)this.Type < (int)other.Type)
        {
            return -1;
        }
        else if ((int)this.Type > (int)other.Type)
        {
            return 1;
        }
        else
        {
            if (this.Hand.Length != other.Hand.Length)
                throw new Exception();

            for (int i = 0; i < this.Hand.Length; i++)
            {
                if (this.GetCardStrength(i) < other.GetCardStrength(i))
                {
                    return -1;
                }
                else if (this.GetCardStrength(i) > other.GetCardStrength(i))
                {
                    return 1;
                }
            }

            return 0;
        }
    }
}