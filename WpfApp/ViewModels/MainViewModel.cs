using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using GraphLibrary.Models;
using Newtonsoft.Json;
using WpfApp.Views;

namespace WpfApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private Graph _graph;
        private string _result;
        private int _gridWidth;
        private int _gridHeight;
        private int _gridStep;

        public ObservableCollection<VertexViewModel> Vertices { get; } = new ObservableCollection<VertexViewModel>();
        public ObservableCollection<EdgeViewModel> Edges { get; } = new ObservableCollection<EdgeViewModel>();
        public Graph Graph => _graph;

        public string Result
        {
            get => _result;
            set { _result = value; OnPropertyChanged(); }
        }
        public int GridWidth
        {
            get => _gridWidth;
            set { _gridWidth = value; OnPropertyChanged(); UpdateVertexPositions(); }
        }
        public int GridHeight
        {
            get => _gridHeight;
            set { _gridHeight = value; OnPropertyChanged(); UpdateVertexPositions(); }
        }
        public int GridStep
        {
            get => _gridStep;
            set { _gridStep = value; OnPropertyChanged(); UpdateVertexPositions(); }
        }

        public ICommand RunBFSCommand { get; }
        public ICommand RunDFSCommand { get; }
        public ICommand RunDijkstraCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand LoadCommand { get; }
        public ICommand DeleteCommand { get; }

        public MainViewModel()
        {
            _graph = new Graph();
            RunBFSCommand = new RelayCommand(StartBFS);
            RunDFSCommand = new RelayCommand(StartDFS);
            RunDijkstraCommand = new RelayCommand(StartDijkstra);
            SaveCommand = new RelayCommand(SaveGraph);
            LoadCommand = new RelayCommand(LoadGraph);
            DeleteCommand = new RelayCommand(DeleteGraph);
            LoadGraph(null);
            GridWidth = 10;
            GridHeight = 10;
            GridStep = 1;
        }

        private void StartBFS(object parameter)
        {
            if (_graph.AdjacencyList.Any())
            {
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.StartBFS();
                }
            }
            else
            {
                Result = "Graph is empty.";
            }
        }

        private void StartDFS(object parameter)
        {
            if (_graph.AdjacencyList.Any())
            {
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.StartDFS();
                }
            }
            else
            {
                Result = "Graph is empty.";
            }
        }

        private void StartDijkstra(object parameter)
        {
            if (_graph.AdjacencyList.Any())
            {
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.StartDijkstra();
                }
            }
            else
            {
                Result = "Graph is empty.";
            }
        }

        private void SaveGraph(object parameter)
        {
            var data = new GraphData
            {
                Vertices = Vertices.Select(v => new VertexData { Id = v.Vertex.Id, Label = v.Vertex.Label, X = v.X / (GridStep * 30), Y = -v.Y / (GridStep * 30) }).ToList(),
                Edges = Edges.Select(e => new EdgeData { From = e.From.Vertex.Id, To = e.To.Vertex.Id, Weight = e.Weight }).ToList(),
                GridWidth = GridWidth,
                GridHeight = GridHeight,
                GridStep = GridStep
            };
            File.WriteAllText("graph.json", JsonConvert.SerializeObject(data));
            Result = "Graph saved.";
        }

        private void LoadGraph(object parameter)
        {
            if (File.Exists("graph.json"))
            {
                var data = JsonConvert.DeserializeObject<GraphData>(File.ReadAllText("graph.json"));
                _graph = new Graph();
                Vertices.Clear();
                Edges.Clear();

                GridWidth = data.GridWidth;
                GridHeight = data.GridHeight;
                GridStep = data.GridStep;

                foreach (var v in data.Vertices)
                {
                    _graph.AddVertex(v.Id, v.Label);
                    Vertices.Add(new VertexViewModel(new Vertex(v.Id, v.Label)) { X = v.X * GridStep * 30, Y = -v.Y * GridStep * 30 });
                }

                foreach (var e in data.Edges)
                {
                    _graph.AddEdge(e.From, e.To, e.Weight);
                    var from = Vertices.First(v => v.Vertex.Id == e.From);
                    var to = Vertices.First(v => v.Vertex.Id == e.To);
                    Edges.Add(new EdgeViewModel(from, to, e.Weight));
                }
                Result = "Graph loaded.";
            }
        }

        private void DeleteGraph(object parameter)
        {
            if (File.Exists("graph.json"))
            {
                File.Delete("graph.json");
                Vertices.Clear();
                Edges.Clear();
                _graph = new Graph();
                Result = "Graph deleted and reset.";
            }
            else
            {
                Result = "No saved graph to delete.";
            }
        }

        private void UpdateVertexPositions()
        {
            if (GridWidth > 0 && GridHeight > 0 && GridStep > 0)
            {
                foreach (var vertex in Vertices)
                {
                    int x = (int)(vertex.X / (vertex.X == 0 ? 1 : GridStep * 30));
                    int y = (int)(-vertex.Y / (vertex.Y == 0 ? 1 : GridStep * 30));
                    vertex.X = x * GridStep * 30;
                    vertex.Y = -y * GridStep * 30;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class GraphData
    {
        public List<VertexData> Vertices { get; set; }
        public List<EdgeData> Edges { get; set; }
        public int GridWidth { get; set; }
        public int GridHeight { get; set; }
        public int GridStep { get; set; }
    }

    public class VertexData
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class EdgeData
    {
        public int From { get; set; }
        public int To { get; set; }
        public int Weight { get; set; }
    }
}