using System.Collections;

public class CyclicalElementProvider<T> : IEnumerator<T>, IEnumerable<T>
{
    private int currentIndex;

    private readonly Func<T>[] funcs;

    public CyclicalElementProvider(IEnumerable<Func<T>> funcs)
    {
        this.currentIndex = 0;
        this.funcs = funcs.ToArray();
    }

    public int CycleLength => this.funcs.Length;

    public T Current => this.funcs[this.currentIndex]();

    object IEnumerator.Current => this.Current;

    public bool MoveNext()
    {
        this.currentIndex++;
        if (this.currentIndex >= this.funcs.Length)
        {
            this.currentIndex = 0;
        }
        return true;
    }

    public void Reset()
    {
        this.currentIndex = 0;
    }

    public void Dispose()
    {

    }

    public IEnumerator<T> GetEnumerator()
    {
        while (this.MoveNext())
        {
            yield return this.Current;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}