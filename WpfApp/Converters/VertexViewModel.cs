using System.ComponentModel;
using System.Runtime.CompilerServices;
using GraphLibrary.Models;

namespace WpfApp.ViewModels
{
    public class VertexViewModel : INotifyPropertyChanged
    {
        private int _gridX;
        private int _gridY;
        private double _x;
        private double _y;
        private bool _isVisited;

        public Vertex Vertex { get; }
        public int GridX
        {
            get => _gridX;
            set { _gridX = value; OnPropertyChanged(); }
        }
        public int GridY
        {
            get => _gridY;
            set { _gridY = value; OnPropertyChanged(); }
        }
        public double X
        {
            get => _x;
            set { _x = value; OnPropertyChanged(); }
        }
        public double Y
        {
            get => _y;
            set { _y = value; OnPropertyChanged(); }
        }
        public bool IsVisited
        {
            get => _isVisited;
            set { _isVisited = value; OnPropertyChanged(); }
        }

        public VertexViewModel(Vertex vertex)
        {
            Vertex = vertex;
        }

        public void UpdateCanvasPosition(double stepSize, double offsetX, double offsetY, int halfWidth, int halfHeight)
        {
            X = offsetX + (_gridX + halfWidth) * stepSize;
            Y = offsetY + (_gridY + halfHeight) * stepSize;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}