using System;

namespace SQLiteStudio.Models
{
    public class DocumentRequestedEventArgs : EventArgs
    {
        public string DatabasePath { get; set; }
        public DocumentRequestedEventArgs() { }
        public DocumentRequestedEventArgs(string databasePath)
        {
            DatabasePath = databasePath;
        }
    }
}
