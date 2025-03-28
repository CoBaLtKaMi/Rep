using System;
using System.Collections.Generic;
using System.Linq;
using GraphLibrary.Models;

namespace GraphLibrary.Algorithms
{
    /// <summary>
    /// Реализация алгоритма Дейкстры для поиска кратчайших путей
    /// </summary>
    public static class Dijkstra
    {
        private class MinHeap
        {
            private List<(int vertex, int distance)> heap = new List<(int vertex, int distance)>();

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
                heap[0] = heap[heap.Count-1];
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

        public static (int[] distances, int[] previous) Execute(Graph graph, int startVertex)
        {
            var vertexIds = graph.Vertices.Select(v => v.Id).ToList();
            int n = graph.Vertices.Count;
            int[] distances = new int[n];
            int[] previous = new int[n];
            var heap = new MinHeap();

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
                var (currentVertex, currentDistance) = heap.Dequeue();
                int currentIndex = vertexIds.IndexOf(currentVertex);

                if (currentDistance > distances[currentIndex]) continue;

                foreach (var (neighbor, weight) in graph.AdjacencyList[currentVertex])
                {
                    int neighborIndex = vertexIds.IndexOf(neighbor);
                    int newDist = distances[currentIndex] + weight;

                    if (newDist < distances[neighborIndex])
                    {
                        distances[neighborIndex] = newDist;
                        previous[neighborIndex] = currentIndex;
                        heap.Enqueue((neighbor, newDist));
                    }
                }
            }

            return (distances, previous);
        }
    }
}