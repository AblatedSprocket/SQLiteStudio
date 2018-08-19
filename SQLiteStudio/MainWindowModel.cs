using CustomPresentationControls;
using CustomPresentationControls.Utilities;
using SQLiteStudio.DatabaseNavigation;
using SQLiteStudio.DatabaseVisualization;
using SQLiteStudio.Models;
using SQLiteStudio.Services;
using SQLiteStudio.Services.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace SQLiteStudio
{
    class MainWindowModel
    {
        #region Fields
        private ISqlService _sqliteService = new SqliteService();
        #endregion
        #region Properties
        public DatabaseNavigationViewModel DatabaseNavigationViewModel { get; }
        public DatabaseVisualizationViewModel DatabaseVisualizationViewModel { get; }
        #endregion
        #region Commands
        public RelayCommand<Item> NewQueryCommand { get; }
        #endregion
        #region Events
        EventHandler<DocumentRequestedEventArgs> QueryCreated = delegate { };
        #endregion
        public MainWindowModel()
        {
            DatabaseNavigationViewModel = new DatabaseNavigationViewModel();
            DatabaseNavigationViewModel.DocumentRequested += OnQueryRequested;
            DatabaseNavigationViewModel.ItemsCollectionChanged += OnItemsCollectionChanged;
            DatabaseVisualizationViewModel = new DatabaseVisualizationViewModel();
        }
        #region Event Handlers
        private void OnQueryRequested(object sender, DocumentRequestedEventArgs e)
        {
            DatabaseVisualizationViewModel.AddNewDocument(e.DatabasePath);
        }
        private void OnItemsCollectionChanged(object sender, ItemsCollectionChangedEventArgs e)
        {
            DatabaseVisualizationViewModel.AvailableDatabases = new ObservableCollection<string>(e.Items.Select(i => i.Path));
        }
        #endregion
        #region Private Methods
        #endregion
    }
}
