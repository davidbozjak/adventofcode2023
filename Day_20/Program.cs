using System.Diagnostics;
using System.Text;

var input = new StringInputProvider("Input.txt").ToList();

var signals = new List<Signal>();
Func<Module, bool, Module, Signal?> signalFactory = (receiver, ishigh, sender) =>
{
    var signal = new Signal(signals.Count + 1, receiver, ishigh, sender);
    signals.Add(signal);
    return signal;
};

Queue<Signal> signalProcessingQueue = new();
var modules = new List<Module>();

foreach (var line in input)
{
    var name = line[..line.IndexOf(' ')];

    Module module = name[0] switch
    {
        '%' => new FlipFlopModule(name[1..], signalFactory, signalProcessingQueue),
        '&' => new ConjunctionModule(name[1..], signalFactory, signalProcessingQueue),
        _ => new BroadcastModule(name, signalFactory, signalProcessingQueue)
    };

    modules.Add(module);
}

//Manually adding output as it does not appear "normally" in the list
//modules.Add(new BroadcastModule("output", signalFactory, signalProcessingQueue)); // for testing of example 2
var output = new BroadcastModule("rx", signalFactory, signalProcessingQueue);
modules.Add(output);

foreach (var line in input)
{
    var parts = line.Split([" ", "->", ",", "%", "&"], StringSplitOptions.RemoveEmptyEntries);
    if (parts.Length < 2)
        throw new Exception();

    var module = modules.First(w => w.Name == parts[0]);

    for (int i = 1; i < parts.Length; i++)
    {
        var connectedModule = modules.FirstOrDefault(w => w.Name == parts[i]);
        if (connectedModule != null)
        {
            module.ConnectOutput(connectedModule);
            connectedModule.ConnectInput(module);
        }
    }
}

var broadcasters = modules.Where(w => w.Name == "broadcaster");

if (broadcasters.Count() != 1)
    throw new Exception();

var broadcaster = broadcasters.First();

var primaryCycleRemaining = new List<Module>();
var secondaryCycleRemaining = new List<Module>();

foreach (var firstConnection in output.Inputs)
{
    primaryCycleRemaining.Add(firstConnection);

    foreach (var secondConnection in firstConnection.Inputs)
    {
        secondaryCycleRemaining.Add(secondConnection);
    }
}

//not sure how to detect this from input, deduced by manually inspecting the input lines
bool signalTargetForPrimary = false;
bool signalTargetForSecondary = true;   

List<long> primaryCycleLengths = new List<long>();
List<long> secondaryCycleLengths = new List<long>();

for (int pressOfButton = 1; pressOfButton <= 1000 || (primaryCycleRemaining.Count > 0 && secondaryCycleRemaining.Count > 0) ; pressOfButton++)
{
    var broadcastSignal = signalFactory(broadcaster, false, null);
    signalProcessingQueue.Enqueue(broadcastSignal);

    while (signalProcessingQueue.Count > 0)
    {
        var signalToProcess = signalProcessingQueue.Dequeue();
       
        if (primaryCycleRemaining.Contains(signalToProcess.Sender) && signalToProcess.IsHigh == signalTargetForPrimary)
        {
            primaryCycleLengths.Add(pressOfButton);
            primaryCycleRemaining.Remove(signalToProcess.Sender);
        }

        if (secondaryCycleRemaining.Contains(signalToProcess.Sender) && signalToProcess.IsHigh == signalTargetForSecondary)
        {
            secondaryCycleLengths.Add(pressOfButton);
            secondaryCycleRemaining.Remove(signalToProcess.Sender);
        }

        signalToProcess.Receiver.ReceiveSignal(signalToProcess);
    }

    if (pressOfButton == 1000)
    {
        var highPulses = signals.Where(w => w.IsHigh).ToList();
        var lowPulses = signals.Where(w => !w.IsHigh).ToList();

        Console.WriteLine($"Part 1: {highPulses.Count * lowPulses.Count}");
    }
}

var cycles = primaryCycleRemaining.Count == 0 ? primaryCycleLengths : secondaryCycleLengths;
var lcm = MathUtils.LeastCommonMultiple(cycles);

Console.WriteLine($"Part 2: {lcm}");

class BroadcastModule : Module
{
    public BroadcastModule(string name, Func<Module, bool, Module, Signal> signalFactory, Queue<Signal> signalProcessingQueue) 
        : base(name, signalFactory, signalProcessingQueue)
    {
    }

    public override string Type => "Broadcaster";

    protected override void ReceiveSignalEx(Signal signal)
    {
        SendSignal(signal.IsHigh);
    }

    protected override void SendSignalEx(bool isHigh)
    {
        
    }

    protected override void ConnectInputEx(Module input)
    {
        
    }

    public override string GetState()
    {
        // stateless
        return $"({this.Name})";
    }
}

class FlipFlopModule : Module
{
    private bool isOn = false;

    public FlipFlopModule(string name, Func<Module, bool, Module, Signal> signalFactory, Queue<Signal> signalProcessingQueue)
        : base(name, signalFactory, signalProcessingQueue)
    {
    }

    public override string Type => "Flip-Flop";

    public override string GetState()
    {
        return $"({this.Name}) - [{this.isOn}]";
    }

    protected override void ConnectInputEx(Module input)
    {
        
    }

    protected override void ReceiveSignalEx(Signal signal)
    {
        if (signal.IsHigh)
        {
            //nothing happens, ignore the signal
        }
        else
        {
            if (isOn)
            {
                isOn = false;
                SendSignal(false);
            }
            else
            {
                isOn = true;
                SendSignal(true);
            }

        }
    }

    protected override void SendSignalEx(bool isHigh)
    {
        
    }
}

class ConjunctionModule : Module
{
    private readonly Dictionary<Module, bool> mostRecentReceived = new();
    public ConjunctionModule(string name, Func<Module, bool, Module, Signal> signalFactory, Queue<Signal> signalProcessingQueue) 
        : base(name, signalFactory, signalProcessingQueue)
    {
    }

    public override string Type => "Conjunction";

    public override string GetState()
    {
        var builder = new StringBuilder();
        builder.Append($"({this.Name}) - [");

        foreach (var key in mostRecentReceived.Keys.OrderBy(w => w.Name))
        {
            builder.Append($"[({key.Name}) - {mostRecentReceived[key]}]");
        }
        builder.Append(")");

        return builder.ToString();
    }

    protected override void ConnectInputEx(Module input)
    {
        mostRecentReceived.Add(input, false);
    }

    protected override void ReceiveSignalEx(Signal signal)
    {
        if (signal.Sender == null) 
            throw new Exception();

        mostRecentReceived[signal.Sender] = signal.IsHigh;

        if(mostRecentReceived.Values.All(w => w))
        {
            SendSignal(false);
        }
        else
        {
            SendSignal(true);
        }
    }

    protected override void SendSignalEx(bool isHigh)
    {
        
    }
}

[DebuggerDisplay("{Name} - {Type}")]
abstract class Module
{
    private readonly List<Module> inputModules = new();
    private readonly List<Module> outputModules = new();
    private readonly Func<Module, bool, Module, Signal?> signalFactory;
    private readonly Queue<Signal> signalProcessingQueue;

    public string Name { get; }

    public abstract string Type { get; }

    public IEnumerable<Module> Inputs => this.inputModules.AsReadOnly();
    public IEnumerable<Module> Outputs => this.outputModules.AsReadOnly();

    public Module(string name, Func<Module, bool, Module, Signal?> signalFactory, Queue<Signal> signalProcessingQueue)
    {
        this.Name = name;
        this.signalFactory = signalFactory;
        this.signalProcessingQueue = signalProcessingQueue;
    }

    public void ConnectOutput(Module output)
    {
        this.outputModules.Add(output);
    }

    public void ConnectInput(Module input)
    {
        inputModules.Add(input);

        ConnectInputEx(input);
    }

    protected abstract void ConnectInputEx(Module input);

    public void SendSignal(bool isHigh)
    {
        SendSignalEx(isHigh);

        foreach (var output in outputModules)
        {
            signalProcessingQueue.Enqueue(signalFactory(output, isHigh, this));
        }
    }

    protected abstract void SendSignalEx(bool isHigh);

    public void ReceiveSignal(Signal signal)
    {
        ReceiveSignalEx(signal);
    }

    protected abstract void ReceiveSignalEx(Signal signal);

    public abstract string GetState();
}

record Signal(int Id, Module Receiver, bool IsHigh, Module? Sender);