using SQLiteStudio.DatabaseNavigation;
using SQLiteStudio.Services.Models;
using System.Collections.Generic;

namespace SQLiteStudio.Models
{
    public class ItemsCollectionChangedEventArgs
    {
        public IEnumerable<ObservableTreeItem> Items { get; set; }
        public ItemsCollectionChangedEventArgs(IEnumerable<ObservableTreeItem> items)
        {
            Items = items;
        }
    }
}
