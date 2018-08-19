using SQLiteStudio.Models;
using SQLiteStudio.Services.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SQLiteStudio.CreateTable
{
    /// <summary>
    /// Interaction logic for CreateTableView.xaml
    /// </summary>
    public partial class CreateTableView : UserControl
    {
        public CreateTableView()
        {
            InitializeComponent();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is IColumnData data)
            {
                data.AllowNulls = false;
            }
        }

        private void DataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            //if(DataContext is ICommittable committable)
            //{
            //    committable.CommitCommand.RaiseCanExecuteChanged();
            //}
        }

        private void DataGrid_KeyDown(object sender, KeyEventArgs e)
        {
            //if ( e.Key == Key.Enter && DataContext is ICommittable committable)
            //{
            //    committable.CommitCommand.RaiseCanExecuteChanged();
            //}
        }

        private void DataGrid_LostFocus(object sender, RoutedEventArgs e)
        {
            if (DataContext is ICommittable committable)
            {
                committable.CommitCommand.RaiseCanExecuteChanged();
            }
        }
    }
}
