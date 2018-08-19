using System.Windows;
using System.Windows.Controls;

namespace SQLiteStudio.DatabaseVisualization
{
    /// <summary>
    /// Interaction logic for DatabaseVisualizationView.xaml
    /// </summary>
    public partial class DatabaseVisualizationView : UserControl
    {
        public DatabaseVisualizationView()
        {
            InitializeComponent();
        }
        private void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && DataContext is IDatabaseVisualization mainWindow)
            {
                mainWindow.ActiveDocumentSelectedText = textBox.SelectedText;
            }
        }
    }
}
