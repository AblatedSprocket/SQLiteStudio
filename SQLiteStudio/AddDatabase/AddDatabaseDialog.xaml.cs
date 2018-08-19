using System.Windows;

namespace SQLiteStudio.AddDatabase
{
    /// <summary>
    /// Interaction logic for AddDatabaseDialog.xaml
    /// </summary>
    public partial class AddDatabaseDialog : Window
    {
        public string DatabaseName { get; set; }
        private AddDatabaseViewModel _viewModel;
        public AddDatabaseDialog()
        {
            InitializeComponent();
            _viewModel = new AddDatabaseViewModel();
            _viewModel.NameEntered += OnNameEntered;
            DataContext = _viewModel;
        }
        private void OnNameEntered(object sender, NameEnteredEventArgs e)
        {
            if (e.Name != null)
            {
                DatabaseName = e.Name;
                DialogResult = true;
            }
            else
            {
                DialogResult = false;
            }
        }
    }
}
