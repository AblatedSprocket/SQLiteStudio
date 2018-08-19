using CustomPresentationControls.Utilities;
using SQLiteStudio.Services.Models;
using System.Collections.ObjectModel;

namespace SQLiteStudio.DatabaseNavigation
{
    public enum ItemTypeModel
    {
        Folder,
        Database,
        Table,
        Column,
        DataType
    }
    public class ObservableTreeItem : ObservableObject
    {
        private ObservableCollection<ObservableTreeItem> _items = new ObservableCollection<ObservableTreeItem>();
        private string _name;
        private string _path;
        private ItemTypeModel _type;

        public ObservableCollection<ObservableTreeItem> Items
        {
            get { return _items; }
            set { OnPropertyChanged(ref _items, value); }
        }
        public string Name
        {
            get { return _name; }
            set { OnPropertyChanged(ref _name, value); }
        }
        public string Path
        {
            get { return _path; }
            set { OnPropertyChanged(ref _path, value); }
        }
        public ItemTypeModel Type
        {
            get { return _type; }
            set { OnPropertyChanged(ref _type, value); }
        }
    }
}
