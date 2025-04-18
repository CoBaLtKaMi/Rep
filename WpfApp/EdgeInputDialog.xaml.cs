using System.Windows;

namespace WpfApp.Views
{
    public partial class EdgeInputDialog : Window
    {
        public string Weight => WeightTextBox.Text;

        public EdgeInputDialog()
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