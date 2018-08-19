using SQLiteStudio.Services;
using System.Windows;

namespace SQLiteStudio.CreateTable
{
    /// <summary>
    /// Interaction logic for CreateTableDialog.xaml
    /// </summary>
    public partial class CreateTableDialog : Window
    {
        private CreateTableViewModel _createTableViewModel;
        public string TableName { get; set; }
        public string DatabasePath { get; set; }
        public CreateTableDialog(string databasePath, ISqlService sqlService)
        {
            InitializeComponent();
            DatabasePath = databasePath;
            _createTableViewModel = new CreateTableViewModel(databasePath, sqlService);
            _createTableViewModel.TableCreated += OnTableCreated;
            DataContext = _createTableViewModel;
        }
        private void OnTableCreated(object sender, TableCreatedEventArgs e)
        {
            TableName = e.TableName;
            DialogResult = e.TableCreated;
        }
    }
}
