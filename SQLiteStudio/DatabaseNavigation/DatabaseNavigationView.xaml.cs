using SQLiteStudio.Services.Models;
using System.Windows.Controls;

namespace SQLiteStudio.DatabaseNavigation
{
    /// <summary>
    /// Interaction logic for DatabaseNavigationView.xaml
    /// </summary>
    public partial class DatabaseNavigationView : UserControl
    {
        public DatabaseNavigationView()
        {
            InitializeComponent();
        }

        private void FolderView_Expanded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (e.OriginalSource is TreeViewItem treeViewItem && treeViewItem.Header is ObservableTreeItem item)
            {
                if (DataContext is IDatabaseNavigation databaseNavigation)
                {
                    if (databaseNavigation.ExpandTreeItemCommand.CanExecute(null))
                    {
                        databaseNavigation.ExpandTreeItemCommand.Execute(item);
                    }
                }
            }
        }

        private void FolderView_SelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is ObservableTreeItem item && DataContext is IDatabaseNavigation databaseNavigation)
            {
                databaseNavigation.SelectedItem = item;
            }
        }
    }
}
