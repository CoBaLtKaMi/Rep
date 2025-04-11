using System.Collections.Generic;
using GraphLibrary.Models;

namespace GraphLibrary.Algorithms
{
    /// <summary>
    /// Реализация нерекурсивного алгоритма поиска в глубину (DFS)
    /// </summary>
    public static class DFS
    {
        public static List<int> Execute(Graph graph, int startVertex)
        {
            var visited = new HashSet<int>();
            var stack = new Stack<int>();
            var result = new List<int>();

            stack.Push(startVertex);

            while (stack.Count > 0)
            {
                int current = stack.Pop();

                if (!visited.Contains(current))
                {
                    visited.Add(current);
                    result.Add(current);

                    var neighbors = graph.AdjacencyList[current];
                    for (int i = neighbors.Count - 1; i >= 0; i--)
                    {
                        if (!visited.Contains(neighbors[i].vertex))
                            stack.Push(neighbors[i].vertex);
                    }
                }
            }

            return result;
        }
    }
}