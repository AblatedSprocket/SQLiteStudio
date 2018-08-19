using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteStudio.Query
{
    class QueryEventArgs : EventArgs
    {
        public string DatabasePath { get; set; }
        public QueryEventArgs() { }
        public QueryEventArgs(string databasePath)
        {
            DatabasePath = databasePath;
        }
    }
}
