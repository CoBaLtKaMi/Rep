namespace GraphLibrary.Models
{
    public class Vertex
    {
        public int Id { get; set; }
        public string Label { get; set; }

        public Vertex(int id, string label)
        {
            Id = id;
            Label = label;
        }
    }
}