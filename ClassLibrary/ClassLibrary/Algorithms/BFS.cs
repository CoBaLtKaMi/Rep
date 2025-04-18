using System.Collections.Generic;
using GraphLibrary.Models;

namespace GraphLibrary.Algorithms
{
    public static class BFS
    {
        public static List<int> Execute(Graph graph, int startVertex)
        {
            var (result, _) = ExecuteWithSteps(graph, startVertex);
            return result;
        }

        public static (List<int> result, List<(List<int> visited, string description)> steps) ExecuteWithSteps(Graph graph, int startVertex)
        {
            var visited = new HashSet<int>();
            var queue = new Queue<int>();
            var result = new List<int>();
            var steps = new List<(List<int> visited, string description)>();

            queue.Enqueue(startVertex);

            while (queue.Count > 0)
            {
                int current = queue.Dequeue();

                if (!visited.Contains(current))
                {
                    visited.Add(current);
                    result.Add(current);
                    steps.Add((new List<int>(result), $"Visited vertex {current}"));

                    foreach (var (vertex, weight) in graph.AdjacencyList[current])
                    {
                        if (!visited.Contains(vertex))
                            queue.Enqueue(vertex);
                    }
                }
            }

            return (result, steps);
        }
    }
}