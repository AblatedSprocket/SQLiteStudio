using SQLiteStudio.DatabaseNavigation;
using SQLiteStudio.Services.Models;
using System;

namespace SQLiteStudio.Utilities
{
    public static class Translations
    {
        public static ObservableTreeItem Translate(this Item source)
        {
            return new ObservableTreeItem
            {
                Name = source.Name,
                Path = source.Path,
                Type = (ItemTypeModel)Enum.Parse(typeof(ItemTypeModel), source.Type.ToString())
            };
        }
        public static Item Translate(this ObservableTreeItem source)
        {
            return new Item
            {
                Name = source.Name,
                Path = source.Path,
                Type = (ItemType)Enum.Parse(typeof(ItemType), source.Type.ToString())
            };
        }
    }
}
