using CustomPresentationControls;
using CustomPresentationControls.Utilities;
using SQLiteStudio.Services;
using SQLiteStudio.Services.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Windows;

namespace SQLiteStudio.DatabaseVisualization
{
    public interface IDatabaseVisualization
    {
        string ActiveDocumentSelectedText { get; set; }
    }
    public class DatabaseVisualizationViewModel : ViewModel, IDatabaseVisualization
    {
        #region Fields
        private int _documentCount = 0;
        private string _activeDocumentSelectedText;
        private Document _activeDocument;
        private ObservableCollection<Document> _documents = new ObservableCollection<Document>();
        private ObservableCollection<DataTable> _resultSets = new ObservableCollection<DataTable>();
        private string _activeDatabase;
        private ObservableCollection<string> _availableDatabases;
        private bool _hasResults;
        private ISqlService _sqliteService;
        #endregion
        #region Properties
        public string ActiveDocumentSelectedText
        {
            get { return _activeDocumentSelectedText; }
            set { OnPropertyChanged(ref _activeDocumentSelectedText, value); }
        }
        public Document ActiveDocument
        {
            get { return _activeDocument; }
            set { OnPropertyChanged(ref _activeDocument, value); }
        }
        public ObservableCollection<Document> Documents
        {
            get { return _documents; }
            set { OnPropertyChanged(ref _documents, value); }
        }
        public ObservableCollection<DataTable> ResultSets
        {
            get { return _resultSets; }
            set { OnPropertyChanged(ref _resultSets, value); }
        }
        public string ActiveDatabase
        {
            get { return _activeDatabase; }
            set { OnPropertyChanged(ref _activeDatabase, value); }
        }
        public ObservableCollection<string> AvailableDatabases
        {
            get { return _availableDatabases; }
            set { OnPropertyChanged(ref _availableDatabases, value); }
        }
        public bool HasResults
        {
            get { return _hasResults; }
            set { OnPropertyChanged(ref _hasResults, value); }
        }
        #endregion
        #region Commands
        public RelayCommand<Document> ExecuteCommand { get; }
        public RelayCommand NewDocumentCommand { get; }
        public RelayCommand OpenDocumentCommand { get; }
        public RelayCommand<Document> SaveDocumentCommand { get; }
        public RelayCommand SaveAllDocumentsCommand { get; }
        public RelayCommand<Document> SaveDocumentAsCommand { get; }
        #endregion
        public DatabaseVisualizationViewModel() : this(new SqliteService()) { }
        public DatabaseVisualizationViewModel(ISqlService sqlService)
        {
            _sqliteService = sqlService;

            ExecuteCommand = new RelayCommand<Document>(OnExecute);
            NewDocumentCommand = new RelayCommand(OnAddDocument);
            OpenDocumentCommand = new RelayCommand(OnOpen);
            SaveDocumentCommand = new RelayCommand<Document>(OnSave);
            SaveAllDocumentsCommand = new RelayCommand(OnSaveAll);
            SaveDocumentAsCommand = new RelayCommand<Document>(OnSaveAs);

            ResultSets.CollectionChanged += OnResultSetsCollectionChanged;

            Documents.Add(new Document { FilePath = $"document{_documentCount}" });

            ActiveDocument = Documents[0];
        }
        #region Public Methods
        public void AddNewDocument(string documentDatabase)
        {
            Documents.Add(new Document
            {
                FilePath = $"Document{++_documentCount}",
                DatabasePath = documentDatabase
            });
        }
        #endregion
        #region Command Methods
        private void OnAddDocument()
        {
            Documents.Add(new Document
            {
                FilePath = $"Document{++_documentCount}",
                DatabasePath = ActiveDocument.DatabasePath
            });
        }
        private void OnExecute(Document document)
        {
            ResultSets.Clear();
            try
            {
                IEnumerable<DataTable> results;
                if (string.IsNullOrEmpty(ActiveDocumentSelectedText))
                {
                    results = _sqliteService.ExecuteQuery(_activeDocument.Text, _activeDocument.DatabasePath);
                }
                else
                {
                    results = _sqliteService.ExecuteQuery(ActiveDocumentSelectedText, _activeDocument.DatabasePath);
                }
                foreach (DataTable table in results)
                {
                    ResultSets.Add(table);
                }
            }
            catch (Exception ex)
            {
                WpfMessageBox.ShowDialog("Execution Error", ex.Message, MessageBoxButton.OK, MessageIcon.Error);
            }
        }
        private void OnOpen()
        {
            OpenFolderDialog dialog = new OpenFolderDialog();
            if (dialog.ShowDialog() ?? false)
            {
                Documents.Add(new Document
                {
                    FilePath = dialog.Path,
                    Text = File.ReadAllText(dialog.Path),
                    IsSaved = true
                });
            }
        }
        private void OnSave(Document document)
        {
            if (document.IsSaved)
            {
                File.WriteAllText($"{document.FilePath}.sqe", document.Text);
            }
            else
            {
                OnSaveAs(document);
            }
        }
        private void OnSaveAll()
        {
            foreach(Document document in Documents)
            {
                OnSave(document);
            }
        }
        private void OnSaveAs(Document document)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            if (dialog.ShowDialog() ?? false)
            {
                document.FilePath = dialog.Path.EndsWith(".sqe") ? dialog.Path : string.Concat(dialog.Path, ".sqe");
                if (!File.Exists(document.FilePath) || (WpfMessageBox.ShowDialog("File Exists", "Overwrite existing file?", MessageBoxButton.YesNo, MessageIcon.Warning) ?? false))
                {
                    File.WriteAllText($"{document.FilePath}", document.Text);
                    document.IsSaved = true;
                }
            }
        }
        #endregion
        #region Private Methods
        private void OnResultSetsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            HasResults = _resultSets.Count > 0;
        }
        #endregion
    }
}
