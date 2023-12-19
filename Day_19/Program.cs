using System.Text.RegularExpressions;

var parts = new InputProvider<Part?>("Parts.txt", GetPart).Where(w => w != null).Cast<Part>().ToList();
var rules = new InputProvider<Rule?>("Rules.txt", GetRule).Where(w => w != null).Cast<Rule>().ToDictionary(w => w.Name, w => w);

var acceptedRule = new Rule("A", Enumerable.Empty<string>());
var rejectedRule = new Rule("R", Enumerable.Empty<string>());

rules.Add(acceptedRule.Name, acceptedRule);
rules.Add(rejectedRule.Name, rejectedRule);

var acceptedRegions = GetRangesForRule(rules["in"], rules["A"]);

Console.WriteLine($"Part 1: {parts.Where(w => acceptedRegions.Any(ww => IsAccepted(ww, w))).Sum(w => w.X + w.M + w.A + w.S)}");
Console.WriteLine($"Part 2: {acceptedRegions.Sum(w => (long)w.X.Length * (long)w.M.Length * (long)w.A.Length * (long)w.S.Length)}");


// To test the accepted regions via all the parts provided for part 1:

foreach (var part in parts)
{
    Rule? currentRule = rules["in"];

    while (currentRule != acceptedRule && currentRule != rejectedRule)
    {
        currentRule = currentRule.SortPart(part, rules);
    }

    var matching = acceptedRegions.Where(w => IsAccepted(w, part)).ToList();

    if (currentRule == acceptedRule && matching.Count == 0)
    {
        throw new Exception("False negative: The slow way says part should be accepted, fast way rejects it");
    }
    else if (currentRule == rejectedRule && matching.Count > 0)
    {
        throw new Exception("False positive: The slow way says part should be rejected, fast way accepts it");
    }
}

bool IsAccepted((string _, ClosedInterval X, ClosedInterval M, ClosedInterval A, ClosedInterval S) intervals, Part part)
{
    return intervals.X.ContainsPoint(part.X) &&
        intervals.M.ContainsPoint(part.M) &&
        intervals.A.ContainsPoint(part.A) &&
        intervals.S.ContainsPoint(part.S);
}

List<(string path, ClosedInterval X, ClosedInterval M, ClosedInterval A, ClosedInterval S)> GetRangesForRule(Rule currentRule, Rule endRule)
{
    List<(string path, ClosedInterval X, ClosedInterval M, ClosedInterval A, ClosedInterval S)> lst = new();

    var followingrules = currentRule.GetFollowingRules(rules).ToList();

    foreach (var ruleAcceptanceRange in followingrules)
    {
        if (ruleAcceptanceRange.rule == endRule)
        {
            lst.Add(($"{ruleAcceptanceRange.rule.Name}", ruleAcceptanceRange.X, ruleAcceptanceRange.M, ruleAcceptanceRange.A, ruleAcceptanceRange.S));
        }
        else
        {
            var inner = GetRangesForRule(ruleAcceptanceRange.rule, endRule);

            foreach (var obj in inner)
            {
                lst.Add(($"{ruleAcceptanceRange.rule.Name}->{obj.path}", obj.X.Intersect(ruleAcceptanceRange.X), obj.M.Intersect(ruleAcceptanceRange.M), obj.A.Intersect(ruleAcceptanceRange.A), obj.S.Intersect(ruleAcceptanceRange.S)));
            }
        }
    }

    return lst;
}

static bool GetPart(string? input, out Part? value)
{
    value = null;

    if (input == null) return false;

    Regex numRegex = new(@"-?\d+");

    var numbers = numRegex.Matches(input).Select(w => int.Parse(w.Value)).ToList();

    if (numbers.Count != 4) throw new Exception();

    value = new Part(numbers[0], numbers[1], numbers[2], numbers[3]);

    return true;
}

static bool GetRule(string? input, out Rule? value)
{
    value = null;

    if (input == null) return false;

    var name = input[..input.IndexOf('{')];
    var conditions = input[(input.IndexOf('{') + 1)..input.IndexOf('}')].Split(',');

    value = new Rule(name, conditions);

    return true;
}

record Part(int X, int M, int A, int S);

class Rule
{
    public string Name { get; }

    private readonly string[] instructions;

    public Rule(string name, IEnumerable<string> instructions)
    {
        this.Name = name;
        this.instructions = instructions.ToArray();
    }

    public Rule SortPart(Part part, Dictionary<string, Rule> avaliableRulesDict)
    {
        for (int i = 0; i < instructions.Length;  i++)
        {
            var instruction = instructions[i];

            if (!instruction.Contains(':'))
            {
                return avaliableRulesDict[instruction];
            }

            (char variable, char operation, int thresholdValue, string followingRule) = ParseInstruction(instruction);

            int value = variable switch
            {
                'x' => part.X,
                'm' => part.M,
                'a' => part.A,
                's' => part.S,
                _ => throw new Exception()
            };

            bool accepted = operation switch
            {
                '<' => value < thresholdValue,
                '>' => value > thresholdValue,
                _ => throw new Exception()
            };

            if (accepted)
            {
                return avaliableRulesDict[followingRule];
            }
        }

        throw new Exception();
    }

    public IEnumerable<(ClosedInterval X, ClosedInterval M, ClosedInterval A, ClosedInterval S, Rule rule)> GetFollowingRules(Dictionary<string, Rule> avaliableRulesDict)
    {
        ClosedInterval X = new ClosedInterval(1, 4000), M = new ClosedInterval(1, 4000), A = new ClosedInterval(1, 4000), S = new ClosedInterval(1, 4000);

        for (int i = 0; i < instructions.Length; i++)
        {
            var instruction = instructions[i];

            if (!instruction.Contains(':'))
            {
                yield return (X, M, A, S, avaliableRulesDict[instruction]);
                yield break;
            }

            (char variable, char operation, int thresholdValue, string followingRule) = ParseInstruction(instruction);

            ClosedInterval acceptRange = operation switch
            {
                '<' => new ClosedInterval(1, thresholdValue - 1),
                '>' => new ClosedInterval(thresholdValue + 1, 4000),
                _ => throw new Exception()
            };

            ClosedInterval rejectRange = operation switch
            {
                '<' => new ClosedInterval(thresholdValue, 4000),
                '>' => new ClosedInterval(1, thresholdValue),
                _ => throw new Exception()
            };

            if (acceptRange.Length + rejectRange.Length != 4000)
                throw new Exception();

            if (variable == 'x')
            {
                acceptRange = acceptRange.Intersect(X);
                yield return (acceptRange, M, A, S, avaliableRulesDict[followingRule]);
                X = rejectRange.Intersect(X);
            }
            else if (variable == 'm')
            {
                acceptRange = acceptRange.Intersect(M);
                yield return (X, acceptRange, A, S, avaliableRulesDict[followingRule]);
                M = rejectRange.Intersect(M);
            }
            else if (variable == 'a')
            {
                acceptRange = acceptRange.Intersect(A);
                yield return (X, M, acceptRange, S, avaliableRulesDict[followingRule]);
                A = rejectRange.Intersect(A);
            }
            else if (variable == 's')
            {
                acceptRange = acceptRange.Intersect(S);
                yield return (X, M, A, acceptRange, avaliableRulesDict[followingRule]);
                S = rejectRange.Intersect(S);
            }
            else throw new Exception();
        }
    }

    private (char, char, int, string) ParseInstruction(string instruction)
    {
        char variable = instruction[0];
        char operation = instruction[1];
        int thresholdValue = int.Parse(instruction[2..instruction.IndexOf(':')]);
        string followingRule = instruction[(instruction.IndexOf(':') + 1)..];

        return (variable, operation, thresholdValue, followingRule);
    }
}