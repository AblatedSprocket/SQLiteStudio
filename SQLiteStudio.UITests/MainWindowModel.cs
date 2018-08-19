using CustomPresentationControls.Utilities;
using SQLiteStudio.CreateTable;
using SQLiteStudio.DatabaseNavigation;
using SQLiteStudio.DatabaseVisualization;
using SQLiteStudio.Services;
using SQLiteStudio.Services.Models;
using SQLiteStudio.Utilities;
using System.Collections.ObjectModel;
using System.Linq;

namespace SQLiteStudio.UITests
{
    public class MainWindowModel : ViewModel
    {
        public DatabaseNavigationViewModel DatabaseNavigation { get; set; }
        public DatabaseVisualizationViewModel DatabaseVisualizationViewModel { get; set; }
        public MainWindowModel()
        {
            string databasePath = @"D:\Dev\Database";
            ISqlService sqlService = new SqliteService();
            DatabaseNavigation = new DatabaseNavigationViewModel();
            ObservableTreeItem item = new ObservableTreeItem
            {
                Name = "Database",
                Path = databasePath,
                Type = ItemTypeModel.Folder
            };
            CreateTableDialog dialog = new CreateTableDialog(databasePath, sqlService);
            //dialog.ShowDialog();
            item.Items = new ObservableCollection<ObservableTreeItem>(sqlService.GetItems(item.Translate()).Select(i => i.Translate()));
            DatabaseNavigation.Items.Add(item);
            DatabaseNavigation.RefreshDatabases();
            DatabaseVisualizationViewModel = new DatabaseVisualizationViewModel();
            DatabaseVisualizationViewModel.Documents.Add(new Document
            {
                FilePath = @"Users\Andy\file.sqe"
            });
            DatabaseVisualizationViewModel.AvailableDatabases = new ObservableCollection<string> { databasePath + @"\home.db" };
            DatabaseVisualizationViewModel.ActiveDatabase = DatabaseVisualizationViewModel.AvailableDatabases[0];
        }
    }
}
