using CustomPresentationControls;
using CustomPresentationControls.Utilities;
using SQLiteStudio.AddDatabase;
using SQLiteStudio.CreateTable;
using SQLiteStudio.Models;
using SQLiteStudio.Services;
using SQLiteStudio.Services.Models;
using SQLiteStudio.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows;

namespace SQLiteStudio.DatabaseNavigation
{
    public interface IDatabaseNavigation
    {
        ObservableTreeItem SelectedItem { get; set; }
        RelayCommand<ObservableTreeItem> ExpandTreeItemCommand { get; }
    }
    public class DatabaseNavigationViewModel : ViewModel, IDatabaseNavigation
    {
        #region Fields
        private ISqlService _sqliteService;
        private ObservableTreeItem _selectedItem;
        #endregion
        #region Properties
        public ObservableTreeItem SelectedItem
        {
            get { return _selectedItem; }
            set { OnPropertyChanged(ref _selectedItem, value); }
        }
        public ObservableCollection<ObservableTreeItem> Items { get; set; } = new ObservableCollection<ObservableTreeItem>();
        #endregion
        #region Commands
        public RelayCommand<ObservableTreeItem> AddDatabaseCommand { get; }
        public RelayCommand<ObservableTreeItem> AddTableCommand { get; }
        public RelayCommand<ObservableTreeItem> ExpandTreeItemCommand { get; }
        public RelayCommand<ObservableTreeItem> NewQueryCommand { get; }
        public RelayCommand SearchFolderCommand { get; }
        #endregion
        #region Events
        public event EventHandler<DocumentRequestedEventArgs> DocumentRequested = delegate { };
        public event EventHandler<ItemsCollectionChangedEventArgs> ItemsCollectionChanged = delegate { };
        #endregion
        public DatabaseNavigationViewModel() : this(new SqliteService()) { }
        public DatabaseNavigationViewModel(ISqlService sqlService)
        {
            _sqliteService = sqlService;
            AddDatabaseCommand = new RelayCommand<ObservableTreeItem>(OnNewDatabaseRequested);
            AddTableCommand = new RelayCommand<ObservableTreeItem>(OnNewTableRequested);
            ExpandTreeItemCommand = new RelayCommand<ObservableTreeItem>(OnTreeItemExpanded);
            NewQueryCommand = new RelayCommand<ObservableTreeItem>(OnNewQueryRequested);
            SearchFolderCommand = new RelayCommand(OnSearchFolderRequested);
        }
        #region Command Methods
        private void OnNewDatabaseRequested(ObservableTreeItem item)
        {
            try
            {
                if (item != null && SelectedItem.Type == ItemTypeModel.Folder)
                {
                    AddDatabaseDialog dialog = new AddDatabaseDialog();
                    if (dialog.ShowDialog() ?? false)
                    {
                        string databasePath = dialog.DatabaseName.EndsWith(".db") ? Path.Combine(SelectedItem.Path, dialog.DatabaseName) : Path.Combine(SelectedItem.Path, String.Concat(dialog.DatabaseName, ".db"));
                        _sqliteService.BuildDatabase(databasePath);
                        item.Items.Add(new ObservableTreeItem
                        {
                            Name = dialog.DatabaseName,
                            Path = databasePath,
                            Type = ItemTypeModel.Database
                        });
                        ItemsCollectionChanged(this, new ItemsCollectionChangedEventArgs(Items.SelectMany(i => i.Items)));
                    }
                }
                else
                {
                    WpfMessageBox.ShowDialog("Invalid Item Selected", "Please select a folder to add a database to.", MessageBoxButton.OK, MessageIcon.Error);
                }
            }
            catch (Exception ex)
            {
                WpfMessageBox.ShowDialog("Error Building Database", ex.Message, MessageBoxButton.OK, MessageIcon.Error);
            }
        }
        private void OnNewQueryRequested(ObservableTreeItem item)
        {
            if (item.Type == ItemTypeModel.Database || item.Type == ItemTypeModel.Table)
            {
                DocumentRequested(this, new DocumentRequestedEventArgs(item.Path));
            }
        }
        private void OnNewTableRequested(ObservableTreeItem item)
        {
            if (item != null && SelectedItem.Type == ItemTypeModel.Database)
            {
                CreateTableDialog dialog = new CreateTableDialog(SelectedItem.Path, _sqliteService);
                if (dialog.ShowDialog() ?? false)
                {
                    ObservableTreeItem newItem = new ObservableTreeItem
                    {
                        Name = dialog.TableName,
                        Path = dialog.DatabasePath,
                        Type = ItemTypeModel.Table
                    };
                    newItem.Items = new ObservableCollection<ObservableTreeItem>(_sqliteService.GetItems(newItem.Translate()).Select(i => i.Translate()));
                    item.Items.Add(newItem);
                }
            }
            else
            {
                WpfMessageBox.ShowDialog("Invalid Operation", "Please select a database for this operation.", MessageBoxButton.OK, MessageIcon.Error);
            }
        }
        private void OnSearchFolderRequested()
        {
            OpenFolderDialog dialog = new OpenFolderDialog();
            if (dialog.ShowDialog() ?? false)
            {
                Items.Add(new ObservableTreeItem
                {
                    Name = Path.GetFileNameWithoutExtension(dialog.Path),
                    Path = Path.GetFullPath(dialog.Path),
                    Type = ItemTypeModel.Folder,
                    Items = new ObservableCollection<ObservableTreeItem>(Directory.GetFiles(dialog.Path, "*.db").Select(f => new ObservableTreeItem
                    {
                        Name = Path.GetFileNameWithoutExtension(f),
                        Path = Path.GetFullPath(f),
                        Type = ItemTypeModel.Database
                    })),
                });
                if ((Items.SelectMany(i => i.Items) is IEnumerable<ObservableTreeItem> databases) && databases.Any())
                {
                    ItemsCollectionChanged(this, new ItemsCollectionChangedEventArgs(databases));
                }
            }
        }
        private void OnTreeItemExpanded(ObservableTreeItem item)
        {
            foreach (ObservableTreeItem subItem in item.Items)
            {
                if (subItem.Type != ItemTypeModel.DataType)
                {
                    subItem.Items = new ObservableCollection<ObservableTreeItem>(_sqliteService.GetItems(subItem.Translate()).Select(i => i.Translate()));
                }
            }
        }
        #endregion
        #region Private Methods
        public void RefreshDatabases()
        {
            foreach (ObservableTreeItem item in Items)
            {
                item.Items = new ObservableCollection<ObservableTreeItem>(_sqliteService.GetItems(item.Translate()).Select(i => i.Translate()));
            }
        }
        #endregion
    }
}
