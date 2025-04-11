using GraphLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WpfApp.ViewModels;

namespace WpfApp.Views
{
    public partial class MainWindow : Window
    {
        private MainViewModel _vm;
        private VertexViewModel _startVertex;
        private Line _tempLine;
        private HashSet<(int, int)> _occupiedPositions;
        private const double VertexSize = 40; // Увеличиваем зону чувствительности до 40x40

        public MainWindow()
        {
            InitializeComponent();
            _vm = new MainViewModel();
            DataContext = _vm;
            _occupiedPositions = new HashSet<(int, int)>();
            Loaded += MainWindow_Loaded;
            _vm.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(_vm.GridStep) || e.PropertyName == nameof(_vm.GridWidth) || e.PropertyName == nameof(_vm.GridHeight)) GenerateGrid(); };
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            GenerateGrid();
            GraphCanvas.SizeChanged += (s, args) => GenerateGrid();
        }

        private void GenerateGrid()
        {
            var itemsToRemove = GraphCanvas.Children.OfType<Line>().Where(l => !(l.DataContext is EdgeViewModel)).ToList();
            foreach (var item in itemsToRemove)
            {
                GraphCanvas.Children.Remove(item);
            }

            double canvasWidth = GraphCanvas.ActualWidth;
            double canvasHeight = GraphCanvas.ActualHeight;
            int gridWidth = _vm.GridWidth;
            int gridHeight = _vm.GridHeight;
            double stepSize = _vm.GridStep * 30;

            int halfWidth = gridWidth / 2;
            int halfHeight = gridHeight / 2;

            double gridTotalWidth = gridWidth * stepSize;
            double gridTotalHeight = gridHeight * stepSize;
            double offsetX = (canvasWidth - gridTotalWidth) / 2;
            double offsetY = (canvasHeight - gridTotalHeight) / 2;

            for (int x = -halfWidth; x <= halfWidth; x++)
            {
                var line = new Line
                {
                    X1 = offsetX + (x + halfWidth) * stepSize,
                    Y1 = offsetY,
                    X2 = offsetX + (x + halfWidth) * stepSize,
                    Y2 = offsetY + gridTotalHeight,
                    Stroke = Brushes.LightGray,
                    StrokeThickness = 1
                };
                Canvas.SetZIndex(line, -1);
                GraphCanvas.Children.Add(line);
            }

            for (int y = -halfHeight; y <= halfHeight; y++)
            {
                var line = new Line
                {
                    X1 = offsetX,
                    Y1 = offsetY + (y + halfHeight) * stepSize,
                    X2 = offsetX + gridTotalWidth,
                    Y2 = offsetY + (y + halfHeight) * stepSize,
                    Stroke = Brushes.LightGray,
                    StrokeThickness = 1
                };
                Canvas.SetZIndex(line, -1);
                GraphCanvas.Children.Add(line);
            }

            var xAxis = new Line
            {
                X1 = offsetX,
                Y1 = offsetY + (halfHeight * stepSize),
                X2 = offsetX + gridTotalWidth,
                Y2 = offsetY + (halfHeight * stepSize),
                Stroke = Brushes.Black,
                StrokeThickness = 3
            };
            Canvas.SetZIndex(xAxis, 0);
            GraphCanvas.Children.Add(xAxis);

            var yAxis = new Line
            {
                X1 = offsetX + (halfWidth * stepSize),
                Y1 = offsetY,
                X2 = offsetX + (halfWidth * stepSize),
                Y2 = offsetY + gridTotalHeight,
                Stroke = Brushes.Black,
                StrokeThickness = 3
            };
            Canvas.SetZIndex(yAxis, 0);
            GraphCanvas.Children.Add(yAxis);
        }
        private void GraphCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                var position = e.GetPosition(GraphCanvas);
                double stepSize = _vm.GridStep * 30;
                int halfWidth = _vm.GridWidth / 2;
                int halfHeight = _vm.GridHeight / 2;

                double gridTotalWidth = _vm.GridWidth * stepSize;
                double gridTotalHeight = _vm.GridHeight * stepSize;
                double offsetX = (GraphCanvas.ActualWidth - gridTotalWidth) / 2;
                double offsetY = (GraphCanvas.ActualHeight - gridTotalHeight) / 2;

                double x = position.X;
                double y = position.Y;

                // Проверяем, попал ли клик в область вершины (40x40)
                VertexViewModel clickedVertex = null;
                foreach (var vertex in _vm.Vertices)
                {
                    double dx = Math.Abs(vertex.X - x);
                    double dy = Math.Abs(vertex.Y - y);
                    if (dx <= VertexSize / 2 && dy <= VertexSize / 2) // Зона 40x40
                    {
                        clickedVertex = vertex;
                        break;
                    }
                }

                if (clickedVertex != null)
                {
                    // Клик по вершине
                    if (_startVertex == null)
                    {
                        _startVertex = clickedVertex;
                        _tempLine = new Line
                        {
                            Stroke = Brushes.Black,
                            StrokeThickness = 2,
                            X1 = _startVertex.X,
                            Y1 = _startVertex.Y,
                            X2 = _startVertex.X,
                            Y2 = _startVertex.Y
                        };
                        GraphCanvas.Children.Add(_tempLine);
                        _vm.Result = $"Selected start vertex: {_startVertex.Vertex.Label}-{_startVertex.Vertex.Id}";
                    }
                    else if (clickedVertex != _startVertex)
                    {
                        var endVertex = clickedVertex;
                        var dialog = new EdgeInputDialog();
                        if (dialog.ShowDialog() == true)
                        {
                            if (int.TryParse(dialog.Weight, out int weight))
                            {
                                _vm.Graph.AddEdge(_startVertex.Vertex.Id, endVertex.Vertex.Id, weight);
                                _vm.Edges.Add(new EdgeViewModel(_startVertex, endVertex, weight));
                                _vm.Result = $"Edge from {_startVertex.Vertex.Id} to {endVertex.Vertex.Id} with weight {weight} added.";
                            }
                            else
                            {
                                _vm.Result = "Invalid weight.";
                            }
                        }
                        GraphCanvas.Children.Remove(_tempLine);
                        _startVertex = null;
                        _tempLine = null;
                    }
                }
                else if (_startVertex == null) // Создание новой вершины, только если не выбрана начальная
                {
                    double gridXRaw = (x - offsetX) / stepSize - halfWidth;
                    double gridYRaw = (y - offsetY) / stepSize - halfHeight;
                    int gridX = (int)Math.Round(gridXRaw);
                    int gridY = (int)Math.Round(gridYRaw);

                    if (gridX >= -halfWidth && gridX <= halfWidth && gridY >= -halfHeight && gridY <= halfHeight)
                    {
                        var positionKey = (gridX, gridY);
                        if (_occupiedPositions.Contains(positionKey))
                        {
                            _vm.Result = $"Position ({gridX}, {gridY}) is already occupied.";
                            return;
                        }
                        var dialog = new VertexInputDialog();
                        if (dialog.ShowDialog() == true)
                        {
                            if (int.TryParse(dialog.VertexId, out int id) && !string.IsNullOrEmpty(dialog.VertexLabel))
                            {
                                if (_vm.Graph.AdjacencyList.ContainsKey(id))
                                {
                                    _vm.Result = $"Vertex with ID {id} already exists.";
                                    return;
                                }

                                _vm.Graph.AddVertex(id, dialog.VertexLabel);
                                var vertex = new VertexViewModel(new Vertex(id, dialog.VertexLabel))
                                {
                                    X = offsetX + (gridX + halfWidth) * stepSize,
                                    Y = offsetY + (gridY + halfHeight) * stepSize
                                };
                                _vm.Vertices.Add(vertex);
                                _occupiedPositions.Add(positionKey);
                                _vm.Result = $"Vertex {dialog.VertexLabel}-{id} added at ({gridX}, {gridY}).";
                            }
                            else
                            {
                                _vm.Result = "Invalid ID or Label.";
                            }
                        }
                    }
                    else
                    {
                        _vm.Result = $"Click outside grid bounds (±{halfWidth}x±{halfHeight}).";
                    }
                }
            }
        }

        private void GraphCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_startVertex != null && _tempLine != null)
            {
                var position = e.GetPosition(GraphCanvas);
                _tempLine.X2 = position.X;
                _tempLine.Y2 = position.Y;
            }
        }

        private void GraphCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Логика убрана, пути создаются через клики
        }
    }
}