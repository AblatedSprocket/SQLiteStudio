using System;

namespace SQLiteStudio.AddDatabase
{
    public class NameEnteredEventArgs : EventArgs
    {
        public string Name { get; set; }
        public NameEnteredEventArgs(string name)
        {
            Name = name;
        }
    }
}
