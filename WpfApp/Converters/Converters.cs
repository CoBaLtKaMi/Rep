using GraphLibrary.Models;
using System.Linq;

namespace GraphLibrary.Converters
{
    public static class GraphConverter
    {
        public static int[,] ToAdjacencyMatrix(Graph graph)
        {
            int size = graph.Vertices.Count;
            int[,] matrix = new int[size, size];

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    matrix[i, j] = int.MaxValue;

            var vertexIds = graph.Vertices.Select(v => v.Id).ToList();
            foreach (var vertex in graph.AdjacencyList)
            {
                int fromIndex = vertexIds.IndexOf(vertex.Key);
                foreach (var edge in vertex.Value)
                {
                    int toIndex = vertexIds.IndexOf(edge.vertex);
                    matrix[fromIndex, toIndex] = edge.weight;
                }
            }

            return matrix;
        }
    }
}