using CustomPresentationControls.Utilities;
using SQLiteStudio.Services;
using SQLiteStudio.Services.Models;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace SQLiteStudio.CreateTable
{
    public interface ICommittable
    {
        RelayCommand CommitCommand { get; }
    }
    public class CreateTableViewModel : ViewModel, ICommittable
    {
        #region Fields
        private ISqlService _sqliteService;
        private string _databasePath;
        private string _tableName;
        private ObservableCollection<ColumnData> _tableColumns = new ObservableCollection<ColumnData>();
        #endregion
        #region Properties
        public string TableName
        {
            get { return _tableName; }
            set
            {
                OnPropertyChanged(ref _tableName, value);
                CommitCommand.RaiseCanExecuteChanged();
            }
        }
        public ObservableCollection<ColumnData> TableColumns
        {
            get { return _tableColumns; }
            set
            {
                OnPropertyChanged(ref _tableColumns, value);
                CommitCommand.RaiseCanExecuteChanged();
            }
        }
        #endregion
        #region Commands
        public RelayCommand CancelCommand { get; }
        public RelayCommand CommitCommand { get; }
        #endregion
        #region Events
        public event EventHandler<TableCreatedEventArgs> TableCreated = delegate { };
        #endregion
        public CreateTableViewModel(string databasePath) : this(databasePath, new SqliteService()) { }
        public CreateTableViewModel(string databasePath, ISqlService sqliteService)
        {
            _databasePath = databasePath;
            _sqliteService = sqliteService;
            CancelCommand = new RelayCommand(OnCancel);
            CommitCommand = new RelayCommand(OnCommit, CanCommit);
            TableColumns.CollectionChanged += TableColumnsChanged;
        }
        #region Command Methods
        private void OnCancel()
        {
            TableCreated(this, new TableCreatedEventArgs(false, null));
        }
        private void OnCommit()
        {
            _sqliteService.BuildTable(_tableName, _tableColumns, _databasePath);
            TableCreated(this, new TableCreatedEventArgs(true, _tableName));
        }
        #endregion
        #region Command Validation
        private bool CanCommit()
        {
            if( !string.IsNullOrEmpty(_tableName) && !_tableName.Contains("sqlite_") && _tableColumns.Count > 0 && Regex.Match(_tableName, "^[a-zA-Z_]+[a-zA-Z0-9_]+$").Success)
            {
                foreach(ColumnData column in _tableColumns)
                {
                    if (string.IsNullOrEmpty(column.Name))
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
        #region Private Methods
        private void TableColumnsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CommitCommand.RaiseCanExecuteChanged();
        }
        #endregion
    }
}
