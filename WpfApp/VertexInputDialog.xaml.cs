using System.Windows;

namespace WpfApp.Views
{
    public partial class VertexInputDialog : Window
    {
        public string VertexLabel => LabelTextBox.Text;

        public VertexInputDialog()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}