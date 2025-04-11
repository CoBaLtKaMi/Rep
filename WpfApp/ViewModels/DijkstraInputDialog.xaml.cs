using System.Linq;
using System.Windows;
using WpfApp.ViewModels;

namespace WpfApp.Views
{
    public partial class DijkstraInputDialog : Window
    {
        public int? StartVertexId { get; private set; }
        public int? EndVertexId { get; private set; }

        public DijkstraInputDialog(MainViewModel viewModel)
        {
            InitializeComponent();
            var vertexIds = viewModel.Vertices.Select(v => v.Vertex.Id.ToString()).ToList();
            StartVertexComboBox.ItemsSource = vertexIds;
            EndVertexComboBox.ItemsSource = vertexIds;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(StartVertexComboBox.SelectedItem?.ToString(), out int startId) &&
                int.TryParse(EndVertexComboBox.SelectedItem?.ToString(), out int endId))
            {
                StartVertexId = startId;
                EndVertexId = endId;
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Please select both start and end vertices.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}