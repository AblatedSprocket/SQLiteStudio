using CustomPresentationControls.Utilities;
using System;
using System.Text.RegularExpressions;

namespace SQLiteStudio.AddDatabase
{
    class AddDatabaseViewModel : ViewModel
    {
        #region Fields
        private string _databaseName;
        #endregion
        #region Properties
        public string DatabaseName
        {
            get { return _databaseName; }
            set
            {
                OnPropertyChanged(ref _databaseName, value);
                CommitCommand.RaiseCanExecuteChanged();
            }
        }
        #endregion
        #region Commands
        public RelayCommand CommitCommand { get; }
        #endregion
        #region Events
        public event EventHandler<NameEnteredEventArgs> NameEntered = delegate { };
        #endregion
        public AddDatabaseViewModel()
        {
            CommitCommand = new RelayCommand(OnCommit, CanCommit);
        }
        #region Command Methods
        private void OnCommit()
        {
            NameEntered(this, new NameEnteredEventArgs(_databaseName));
        }
        #region Command Validation
        private bool CanCommit()
        {
            return _databaseName != null && Regex.Match(_databaseName, "^[a-zA-Z_]+[a-zA-Z0-9_]*$").Success;
        }
        #endregion
        private void OnCancel()
        {
            NameEntered(this, new NameEnteredEventArgs(null));
        }
        #endregion
    }
}
