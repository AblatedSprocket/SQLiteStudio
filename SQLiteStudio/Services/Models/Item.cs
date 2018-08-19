using CustomPresentationControls.FileExplorer;
using CustomPresentationControls.Utilities;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SQLiteStudio.Services.Models
{
    public enum ItemType
    {
        Folder,
        Database,
        Table,
        Column,
        DataType
    }
    public class Item
    {
        public string Name;
        public string Path;
        public ItemType Type;
        public ObservableCollection<Item> Items { get; set; } = new ObservableCollection<Item>();
    }
    
}
