using System;

namespace SQLiteStudio.CreateTable
{
    public class TableCreatedEventArgs : EventArgs
    {
        public bool TableCreated { get; set; }
        public string TableName { get; set; }
        public TableCreatedEventArgs(bool tableCreated, string tableName)
        {
            TableCreated = tableCreated;
            TableName = tableName;
        }
    }
}
