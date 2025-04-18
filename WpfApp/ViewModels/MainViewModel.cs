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
        private List<(List<int> visited, string description)> _algorithmSteps;
        private int _currentStepIndex;
        private List<int> _lastPath;
        private bool _canStepAlgorithm;

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
        public bool CanStepAlgorithm
        {
            get => _canStepAlgorithm;
            set
            {
                _canStepAlgorithm = value;
                Console.WriteLine($"[DEBUG] CanStepAlgorithm set to: {_canStepAlgorithm}, Caller: {new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name}");
                OnPropertyChanged();
            }
        }

        public ICommand RunBFSCommand { get; }
        public ICommand RunDFSCommand { get; }
        public ICommand RunDijkstraCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand LoadCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand StepAlgorithmCommand { get; }
        public ICommand HighlightPathCommand { get; }
        public ICommand ForceEnableStepCommand { get; }

        public MainViewModel()
        {
            _graph = new Graph();
            RunBFSCommand = new RelayCommand(StartBFS);
            RunDFSCommand = new RelayCommand(StartDFS);
            RunDijkstraCommand = new RelayCommand(StartDijkstra);
            SaveCommand = new RelayCommand(SaveGraph);
            LoadCommand = new RelayCommand(LoadGraph);
            DeleteCommand = new RelayCommand(DeleteGraph);
            StepAlgorithmCommand = new RelayCommand(StepAlgorithm, _ => CanStepAlgorithm);
            HighlightPathCommand = new RelayCommand(HighlightPath);
            ForceEnableStepCommand = new RelayCommand(ForceEnableStep);
            LoadGraph(null);
            GridWidth = 10;
            GridHeight = 10;
            GridStep = 1;
            _algorithmSteps = new List<(List<int> visited, string description)>();
            _currentStepIndex = -1;
            _lastPath = new List<int>();
            _canStepAlgorithm = false;
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

        private void StepAlgorithm(object parameter)
        {
            if (_algorithmSteps.Any() && _currentStepIndex < _algorithmSteps.Count - 1)
            {
                _currentStepIndex++;
                var (visited, description) = _algorithmSteps[_currentStepIndex];
                UpdateVertexHighlights(visited);
                Result = $"Step {_currentStepIndex + 1}/{_algorithmSteps.Count}: {description}";
            }
            else
            {
                Result = $"No more steps to display. Total steps: {_algorithmSteps.Count}";
            }
        }

        private void HighlightPath(object parameter)
        {
            if (_lastPath.Any())
            {
                UpdateVertexHighlights(_lastPath);
                Result = "Path highlighted: " + string.Join(" -> ", _lastPath);
            }
            else
            {
                Result = "No path available to highlight.";
            }
        }

        private void ForceEnableStep(object parameter)
        {
            CanStepAlgorithm = true;
            Result = "Step Algorithm button forcibly enabled for testing.";
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

        public void StoreAlgorithmStep(List<int> visited, string description)
        {
            _algorithmSteps.Add((visited, description));
            Console.WriteLine($"[DEBUG] Stored step: {description}, Visited: {string.Join(", ", visited)}");
            CanStepAlgorithm = true;
            OnPropertyChanged(nameof(CanStepAlgorithm)); // Явное уведомление
        }

        public void ResetAlgorithmSteps()
        {
            _algorithmSteps.Clear();
            _currentStepIndex = -1;
            CanStepAlgorithm = false;
            Console.WriteLine("[DEBUG] Algorithm steps reset.");
        }

        public void StorePath(List<int> path)
        {
            _lastPath = path;
            CanStepAlgorithm = _algorithmSteps.Any();
            Console.WriteLine($"[DEBUG] Path stored: {string.Join(" -> ", path)}. Total steps: {_algorithmSteps.Count}. CanStepAlgorithm: {CanStepAlgorithm}");
            OnPropertyChanged(nameof(CanStepAlgorithm)); // Явное уведомление
            if (_algorithmSteps.Any())
            {
                Result = $"Algorithm completed. Total steps: {_algorithmSteps.Count}. Use 'Step Algorithm' to view.";
            }
            else
            {
                Result = "Algorithm completed, but no steps were saved.";
            }
        }

        private void UpdateVertexHighlights(List<int> vertices)
        {
            foreach (var vm in Vertices)
            {
                vm.IsVisited = vertices.Contains(vm.Vertex.Id);
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