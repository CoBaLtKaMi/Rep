using System.Collections.Generic;
using GraphLibrary.Models;

namespace GraphLibrary.Algorithms
{
    public static class DFS
    {
        public static List<int> Execute(Graph graph, int startVertex)
        {
            var (result, _) = ExecuteWithSteps(graph, startVertex);
            return result;
        }

        public static (List<int> result, List<(List<int> visited, string description)> steps) ExecuteWithSteps(Graph graph, int startVertex)
        {
            var visited = new HashSet<int>();
            var stack = new Stack<int>();
            var result = new List<int>();
            var steps = new List<(List<int> visited, string description)>();

            stack.Push(startVertex);

            while (stack.Count > 0)
            {
                int current = stack.Pop();

                if (!visited.Contains(current))
                {
                    visited.Add(current);
                    result.Add(current);
                    steps.Add((new List<int>(result), $"Visited vertex {current}"));

                    var neighbors = graph.AdjacencyList[current];
                    for (int i = neighbors.Count - 1; i >= 0; i--)
                    {
                        if (!visited.Contains(neighbors[i].vertex))
                            stack.Push(neighbors[i].vertex);
                    }
                }
            }

            return (result, steps);
        }
    }
}