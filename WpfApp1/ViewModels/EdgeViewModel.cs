namespace WpfApp1.ViewModels
{
    public class EdgeViewModel
    {
        public VertexViewModel From { get; }
        public VertexViewModel To { get; }
        public int Weight { get; }

        public EdgeViewModel(VertexViewModel from, VertexViewModel to, int weight)
        {
            From = from;
            To = to;
            Weight = weight;
        }
    }
}