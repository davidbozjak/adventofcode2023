using System.Collections;

namespace SantasToolbox;

public class InputProvider<T> : IEnumerator<T>, IEnumerable<T>
{
    public delegate bool StringToTConverter(string? input, out T result);

    private readonly StringToTConverter converter;
    private readonly Cached<StreamReader> fileStream;

    public InputProvider(string filePath, StringToTConverter converter)
    {
        this.converter = converter;

        this.fileStream = new Cached<StreamReader>(() => new StreamReader(filePath));
    }

    public bool EndAtEmptyLine { get; set; } = true;

    public T? Current { get; private set; }

    object? IEnumerator.Current => this.Current;

    public void Dispose()
    {
        this.fileStream?.Dispose();
    }

    public bool MoveNext()
    {
        var line = this.fileStream.Value.ReadLine();

        if (this.EndAtEmptyLine && line?.Length <= 0)
        {
            return false;
        }

        if (this.converter(line, out T result))
        {
            this.Current = result;
            return true;
        }
        else
        {
            this.Current = default;
            return false;
        }
    }

    public void Reset()
    {
        this.fileStream.Reset();
    }

    public IEnumerator<T?> GetEnumerator()
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
