using GraphLibrary.Algorithms;
using GraphLibrary.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System;
using Newtonsoft.Json;
using System.IO;
using System.Linq;


namespace WpfApp1.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private Graph _graph;
        private string _result;
        private string _vertexId;
        private string _vertexLabel;
        private string _fromId;
        private string _toId;
        private string _weight;

        public ObservableCollection<VertexViewModel> Vertices { get; } = new ObservableCollection<VertexViewModel>();
        public ObservableCollection<EdgeViewModel> Edges { get; } = new ObservableCollection<EdgeViewModel>();

        public string Result
        {
            get => _result;
            set { _result = value; OnPropertyChanged(); }
        }
        public string VertexId
        {
            get => _vertexId;
            set { _vertexId = value; OnPropertyChanged(); }
        }
        public string VertexLabel
        {
            get => _vertexLabel;
            set { _vertexLabel = value; OnPropertyChanged(); }
        }
        public string FromId
        {
            get => _fromId;
            set { _fromId = value; OnPropertyChanged(); }
        }
        public string ToId
        {
            get => _toId;
            set { _toId = value; OnPropertyChanged(); }
        }
        public string Weight
        {
            get => _weight;
            set { _weight = value; OnPropertyChanged(); }
        }

        public ICommand AddVertexCommand { get; }
        public ICommand AddEdgeCommand { get; }
        public ICommand RunBFSCommand { get; }
        public ICommand RunDFSCommand { get; }
        public ICommand RunDijkstraCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand LoadCommand { get; }
        public ICommand DeleteCommand { get; } // Добавлено

        public MainViewModel()
        {
            _graph = new Graph();
            AddVertexCommand = new RelayCommand(AddVertex);
            AddEdgeCommand = new RelayCommand(AddEdge);
            RunBFSCommand = new RelayCommand(RunBFS);
            RunDFSCommand = new RelayCommand(RunDFS);
            RunDijkstraCommand = new RelayCommand(RunDijkstra);
            SaveCommand = new RelayCommand(SaveGraph);
            LoadCommand = new RelayCommand(LoadGraph);
            DeleteCommand = new RelayCommand(DeleteGraph); // Добавлено
            LoadGraph(null); // Загрузка при старте
        }
        private void DeleteGraph(object parameter)
        {
            if (File.Exists("graph.json"))
            {
                File.Delete("graph.json"); // Удаляем файл
                Vertices.Clear(); // Очищаем вершины в интерфейсе
                Edges.Clear(); // Очищаем ребра в интерфейсе
                _graph = new Graph(); // Создаем новый пустой граф
                Result = "Graph deleted and reset.";
            }
            else
            {
                Result = "No saved graph to delete.";
            }
        }


        private void AddVertex(object parameter)
        {
            if (int.TryParse(VertexId, out int id) && !string.IsNullOrEmpty(VertexLabel))
            {
                if (!_graph.AdjacencyList.ContainsKey(id))
                {
                    _graph.AddVertex(id, VertexLabel);
                    double radius = 150, centerX = 200, centerY = 200;
                    int count = Vertices.Count;
                    double angle = 2 * Math.PI * count / (count + 1);
                    Vertices.Add(new VertexViewModel(new Vertex(id, VertexLabel))
                    {
                        X = centerX + radius * Math.Cos(angle),
                        Y = centerY + radius * Math.Sin(angle)
                    });
                    VertexId = VertexLabel = "";
                }
            }
        }

        private void AddEdge(object parameter)
        {
            if (int.TryParse(FromId, out int from) && int.TryParse(ToId, out int to) && int.TryParse(Weight, out int weight))
            {
                _graph.AddEdge(from, to, weight);
                var fromVm = Vertices.FirstOrDefault(v => v.Vertex.Id == from);
                var toVm = Vertices.FirstOrDefault(v => v.Vertex.Id == to);
                if (fromVm != null && toVm != null)
                {
                    Edges.Add(new EdgeViewModel(fromVm, toVm, weight));
                    FromId = ToId = Weight = "";
                }
            }
        }
        private void RunBFS(object parameter)
        {
            if (Vertices.Any())
            {
                var result = BFS.Execute(_graph, Vertices[0].Vertex.Id);
                Result = "BFS: " + string.Join(" -> ", result);
                foreach (var vm in Vertices) vm.IsVisited = result.Contains(vm.Vertex.Id);
            }
        }

        private void RunDFS(object parameter)
        {
            if (Vertices.Any())
            {
                var result = DFS.Execute(_graph, Vertices[0].Vertex.Id);
                Result = "DFS: " + string.Join(" -> ", result);
                foreach (var vm in Vertices) vm.IsVisited = result.Contains(vm.Vertex.Id);
            }
        }

        private void RunDijkstra(object parameter)
        {
            if (Vertices.Any())
            {
                var (distances, previous) = Dijkstra.Execute(_graph, Vertices[0].Vertex.Id);
                var indexToId = _graph.Vertices.Select((v, i) => (v.Id, i)).ToDictionary(x => x.i, x => x.Id);
                Result = "Dijkstra: " + string.Join(", ", distances.Select((d, i) => $"{indexToId[i]}: {d}"));
                foreach (var vm in Vertices)
                    vm.IsVisited = distances[_graph.Vertices.FindIndex(v => v.Id == vm.Vertex.Id)] != int.MaxValue;
            }
        }

        private void SaveGraph(object parameter)
        {
            var data = new GraphData
            {
                Vertices = Vertices.Select(v => new VertexData { Id = v.Vertex.Id, Label = v.Vertex.Label, X = v.X, Y = v.Y }).ToList(),
                Edges = Edges.Select(e => new EdgeData { From = e.From.Vertex.Id, To = e.To.Vertex.Id, Weight = e.Weight }).ToList()
            };
            File.WriteAllText("graph.json", JsonConvert.SerializeObject(data));
        }

        private void LoadGraph(object parameter)
        {
            if (File.Exists("graph.json"))
            {
                var data = JsonConvert.DeserializeObject<GraphData>(File.ReadAllText("graph.json"));
                _graph = new Graph();
                Vertices.Clear();
                Edges.Clear();

                foreach (var v in data.Vertices)
                {
                    _graph.AddVertex(v.Id, v.Label);
                    Vertices.Add(new VertexViewModel(new Vertex(v.Id, v.Label)) { X = v.X, Y = v.Y });
                }

                foreach (var e in data.Edges)
                {
                    _graph.AddEdge(e.From, e.To, e.Weight);
                    var from = Vertices.First(v => v.Vertex.Id == e.From);
                    var to = Vertices.First(v => v.Vertex.Id == e.To);
                    Edges.Add(new EdgeViewModel(from, to, e.Weight));
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



