using CustomPresentationControls.Utilities;

namespace SQLiteStudio.Services.Models
{
    public interface IColumnData
    {
        bool AllowNulls { get; set; }
    }
    public class ColumnData : ObservableObject, IColumnData
    {
        private string _name;
        private string _dataType;
        private bool _isPrimaryKey;
        private bool _isForeignKey;
        private bool _allowNulls;
        private string _defaultValue;
        private bool _isAutoIncrement;

        public int Index { get; set; }
        public string Name
        {
            get { return _name; }
            set { OnPropertyChanged(ref _name, value); }
        }
        public string DataType
        {
            get { return _dataType; }
            set { OnPropertyChanged(ref _dataType, value); }
        }
        public bool IsPrimaryKey
        {
            get { return _isPrimaryKey; }
            set { OnPropertyChanged(ref _isPrimaryKey, value); }
        }
        public bool IsForeignKey
        {
            get { return _isForeignKey; }
            set { OnPropertyChanged(ref _isForeignKey, value); }
        }
        public bool AllowNulls
        {
            get { return _allowNulls; }
            set { OnPropertyChanged(ref _allowNulls, value); }
        }
        public bool IsAutoIncrement
        {
            get { return _isAutoIncrement; }
            set { OnPropertyChanged(ref _isAutoIncrement, value); }
        }
        public string DefaultValue
        {
            get { return _defaultValue; }
            set { OnPropertyChanged(ref _defaultValue, value); }
        }
        public ColumnData() { }
        //public ColumnData(string name, string dataType)
        //{
        //    Name = name;
        //    DataType = dataType;
        //}
    }
}
