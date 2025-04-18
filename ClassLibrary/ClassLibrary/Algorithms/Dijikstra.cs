using System;
using System.Collections.Generic;
using System.Linq;
using GraphLibrary.Models;

namespace GraphLibrary.Algorithms
{
    public static class Dijkstra
    {
        private class MinHeap
        {
            private readonly List<(int vertex, int distance)> heap = new List<(int vertex, int distance)>();

            public void Enqueue((int vertex, int distance) item)
            {
                heap.Add(item);
                int index = heap.Count - 1;
                while (index > 0)
                {
                    int parent = (index - 1) / 2;
                    if (heap[parent].distance <= heap[index].distance) break;
                    (heap[parent], heap[index]) = (heap[index], heap[parent]);
                    index = parent;
                }
            }

            public (int vertex, int distance) Dequeue()
            {
                if (heap.Count == 0) throw new InvalidOperationException("Heap is empty");
                var result = heap[0];
                heap[0] = heap[heap.Count - 1];
                heap.RemoveAt(heap.Count - 1);
                int index = 0;
                while (true)
                {
                    int left = 2 * index + 1;
                    int right = 2 * index + 2;
                    int smallest = index;

                    if (left < heap.Count && heap[left].distance < heap[smallest].distance)
                        smallest = left;
                    if (right < heap.Count && heap[right].distance < heap[smallest].distance)
                        smallest = right;

                    if (smallest == index) break;
                    (heap[index], heap[smallest]) = (heap[smallest], heap[index]);
                    index = smallest;
                }
                return result;
            }

            public int Count => heap.Count;
        }

        public static (int[] distances, int[] previous, List<int> path) Execute(Graph graph, int startVertex, int? endVertex = null)
        {
            var (distances, previous, path, _) = ExecuteWithSteps(graph, startVertex, endVertex);
            return (distances, previous, path);
        }

        public static (int[] distances, int[] previous, List<int> path, List<(List<int> visited, string description)> steps) ExecuteWithSteps(Graph graph, int startVertex, int? endVertex = null)
        {
            var vertexIds = graph.Vertices.Select(v => v.Id).ToList();
            int n = graph.Vertices.Count;
            int[] distances = new int[n];
            int[] previous = new int[n];
            var heap = new MinHeap();
            List<int> path = new List<int>();
            var steps = new List<(List<int> visited, string description)>();
            var visited = new HashSet<int>();
            var visitedList = new List<int>(); // To maintain order for steps

            for (int i = 0; i < n; i++)
            {
                distances[i] = int.MaxValue;
                previous[i] = -1;
            }

            int startIndex = vertexIds.IndexOf(startVertex);
            distances[startIndex] = 0;
            heap.Enqueue((startVertex, 0));

            while (heap.Count > 0)
            {
                var current = heap.Dequeue();
                int currentVertex = current.vertex;
                int currentDistance = current.distance;
                int currentIndex = vertexIds.IndexOf(currentVertex);

                if (currentDistance > distances[currentIndex]) continue;

                if (!visited.Contains(currentVertex))
                {
                    visited.Add(currentVertex);
                    visitedList.Add(currentVertex);
                    steps.Add((new List<int>(visitedList), $"Visited vertex {currentVertex}"));

                    foreach (var neighbor in graph.AdjacencyList[currentVertex])
                    {
                        int neighborVertex = neighbor.vertex;
                        int weight = neighbor.weight;
                        int neighborIndex = vertexIds.IndexOf(neighborVertex);
                        int newDist = distances[currentIndex] + weight;

                        if (newDist < distances[neighborIndex])
                        {
                            distances[neighborIndex] = newDist;
                            previous[neighborIndex] = currentIndex;
                            heap.Enqueue((neighborVertex, newDist));
                        }
                    }
                }
            }

            if (endVertex.HasValue)
            {
                int endIndex = vertexIds.IndexOf(endVertex.Value);
                if (distances[endIndex] != int.MaxValue)
                {
                    int current = endIndex;
                    while (current != -1)
                    {
                        path.Add(vertexIds[current]);
                        current = previous[current];
                    }
                    path.Reverse();
                }
            }

            return (distances, previous, path, steps);
        }
    }
}