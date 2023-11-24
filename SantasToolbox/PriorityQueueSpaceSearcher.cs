namespace SantasToolbox;

public abstract record SearchSpaceState(int Score, int Priority)
{
    public abstract IEnumerable<SearchSpaceState> GetSubsequentStates();

    public abstract string GetStringHash();
}

public class PriorityQueueSpaceSearcher<T>
    where T : SearchSpaceState
{
    public bool DiscardVisited { get; init; }

    public bool EnableTracing { get; init; }

    public T FindHighestScore(T initialState, Func<T, bool> IsEndStateFunc, Func<T, T?, bool> DiscardStateFunc)
    {
        var openSet = new PriorityQueue<T, int>();
        openSet.Enqueue(initialState, 0);

        T? currentBestState = null;

        HashSet<string> visitedStates = new();

        while (openSet.Count > 0)
        {
            var current = openSet.Dequeue();

            if (IsEndStateFunc(current))
            {
                if (currentBestState == null || current.Score > currentBestState.Score)
                {
                    if (this.EnableTracing)
                    {
                        Console.WriteLine($"{DateTime.Now.TimeOfDay}: Found new best: {current.Score} Open nodes: {openSet.Count}");
                    }
                    currentBestState = current;
                }
                continue;
            }

            foreach (var followingState in current.GetSubsequentStates())
            {
                if (this.DiscardVisited)
                {
                    if (visitedStates.Contains(followingState.ToString()))
                        continue;

                    visitedStates.Add(followingState.GetStringHash());
                }

                if (followingState == null)
                    throw new Exception();

                if (DiscardStateFunc((T)followingState, currentBestState))
                    continue;

                openSet.Enqueue((T)followingState, followingState.Priority);
            }
        }

        if (currentBestState == null) 
            throw new Exception();

        return currentBestState;
    }
}
