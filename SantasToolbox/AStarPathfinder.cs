namespace SantasToolbox;

public interface INode
{
    int Cost { get; }
}

public static class AStarPathfinder
{
    /// <summary>
    /// Utilizes A* algorithm to find a path from start to goal node
    /// </summary>
    /// <param name="start">Starting node</param>
    /// <param name="goal">End node</param>
    /// <param name="GetHeuristicCost">A function returning best guess cost from node to end node. Provide 0 for adaptation to Dijkstra's algorithm.</param>
    /// <param name="GetNeighbours">A function that provides all reachable neighbours for each Node</param>
    /// <returns>A List of visited nodes from start to goal, or null if no path could be found</returns>
    public static List<T>? FindPath<T>(T start, T goal, Func<T, int> GetHeuristicCost, Func<T, IEnumerable<T>> GetNeighbours)
        where T : class, INode, IEquatable<T>
    {
        var openSet = new PriorityQueue<T, int>();
        openSet.Enqueue(start, 0);

        var cameFrom = new Dictionary<T, T>();
        var gScore = new Dictionary<T, int>
        {
            [start] = 0
        };

        var fScore = new Dictionary<INode, int>
        {
            [start] = GetHeuristicCost(start)
        };
        
        while (openSet.Count > 0)
        {
            var current = openSet.Dequeue();

            if (current.Equals(goal))
            {
                //reconstruct path!
                var path = new List<T>() { current };

                while (cameFrom.ContainsKey(current))
                {
                    current = cameFrom[current];
                    path.Insert(0, current);
                }

                return path;
            }

            foreach (var neighbour in GetNeighbours(current))
            {
                var tentativeScore = gScore[current] + neighbour.Cost;

                var g = gScore.ContainsKey(neighbour) ? gScore[neighbour] : int.MaxValue;
                if (tentativeScore < g)
                {
                    cameFrom[neighbour] = current;
                    gScore[neighbour] = tentativeScore;

                    var priority = tentativeScore + GetHeuristicCost(neighbour);
                    
                    fScore[neighbour] = priority;
                    openSet.Enqueue(neighbour, priority); //Considering somehow handling duplicates?
                }
            }
        }

        return null;
    }
}

public class CachedPathfinder<T>
    where T : class, INode, IEquatable<T>
{
    private readonly Dictionary<(T, T), List<T>> memcache = new();

    public List<T> FindPath(T start, T goal, Func<T, int> GetHeuristicCost, Func<T, IEnumerable<T>> GetNeighbours)
    {
        var key = (start, goal);

        if (!memcache.ContainsKey(key))
        {
            var path = AStarPathfinder.FindPath(start, goal, GetHeuristicCost, GetNeighbours);

            if (path == null)
                throw new Exception();

            memcache[key] = path;
        }

        return memcache[key];
    }
}