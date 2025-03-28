using System.Collections.Generic;

namespace GraphLibrary.Models
{
    public class Graph
    {
        public List<Vertex> Vertices { get; private set; }
        public Dictionary<int, List<(int vertex, int weight)>> AdjacencyList { get; private set; }

        public Graph()
        {
            Vertices = new List<Vertex>();
            AdjacencyList = new Dictionary<int, List<(int vertex, int weight)>>();
        }

        public void AddVertex(int id, string label)
        {
            Vertices.Add(new Vertex(id, label));
            AdjacencyList[id] = new List<(int vertex, int weight)>();
        }

        public void AddEdge(int from, int to, int weight)
        {
            if (AdjacencyList.ContainsKey(from) && AdjacencyList.ContainsKey(to))
            {
                AdjacencyList[from].Add((to, weight));
            }
        }
    }
}