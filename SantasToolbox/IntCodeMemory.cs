namespace SantasToolbox;

public interface IIntReadOnlyCodeMemory
{
    IIntReadOnlyCodeMemory Clone();

    IIntCodeMemory CloneWriteable();

    long this[long index] { get; }

    int Length { get; }
}

public interface IIntCodeMemory : IIntReadOnlyCodeMemory
{
    new long this[long index] { get; set; }
}

public class IntCodeMemory : IIntCodeMemory
{
    private readonly long[] memory;

    public IntCodeMemory(IEnumerable<long> inputProvider)
    {
        this.memory = new long[100000];
        inputProvider.ToArray().CopyTo(this.memory, 0);
    }

    public long this[long index]
    {
        get => this.memory[index];
        set => this.memory[index] = value;
    }

    public int Length => this.memory.Length;

    public IIntReadOnlyCodeMemory Clone() => new IntCodeMemory(this.memory);

    public IIntCodeMemory CloneWriteable() => new IntCodeMemory(this.memory);

}
