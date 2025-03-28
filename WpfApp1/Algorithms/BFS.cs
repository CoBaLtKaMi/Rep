using System.Collections.Generic;
using GraphLibrary.Models;

namespace GraphLibrary.Algorithms
{
    /// <summary>
    /// Реализация нерекурсивного алгоритма поиска в ширину (BFS)
    /// </summary>
    public static class BFS
    {
        public static List<int> Execute(Graph graph, int startVertex)
        {
            var visited = new HashSet<int>();
            var queue = new Queue<int>();
            var result = new List<int>();

            queue.Enqueue(startVertex);

            while (queue.Count > 0)
            {
                int current = queue.Dequeue();

                if (!visited.Contains(current))
                {
                    visited.Add(current);
                    result.Add(current);

                    foreach (var neighbor in graph.AdjacencyList[current])
                    {
                        if (!visited.Contains(neighbor.vertex))
                            queue.Enqueue(neighbor.vertex);
                    }
                }
            }

            return result;
        }
    }
}