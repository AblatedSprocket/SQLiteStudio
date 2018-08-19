using CustomPresentationControls.Utilities;

namespace SQLiteStudio
{
    public class Document : ObservableObject
    {
        private string _filePath;
        private string _databasePath;
        private string _text;
        private string _selectedText;
        
        public string FilePath
        {
            get { return _filePath; }
            set { OnPropertyChanged(ref _filePath, value); }
        }
        public string DatabasePath
        {
            get { return _databasePath; }
            set { OnPropertyChanged(ref _databasePath, value); }
        }
        public string Text
        {
            get { return _text; }
            set { OnPropertyChanged(ref _text, value); }
        }
        public string SelectedText
        {
            get { return _selectedText; }
            set { OnPropertyChanged(ref _selectedText, value); }
        }
        public bool IsSaved { get; set; }
    }
}
