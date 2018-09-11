//THIS CLASS IS COMMENTED TO PREVENT UNNECESSARY BLOAT FROM SYSTEM.DATA.SQLITE NUGET PACKAGE, HOWEVER SMALL THAT BLOAT MAY BE

//using SQLiteStudio.Services.Models;
//using SQLiteStudio.Utilities;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.SQLite;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Text.RegularExpressions;

//namespace SQLiteStudio.Services
//{
//    public class SystemSqliteService : ISqlService
//    {
//        #region Public Methods
//        public void BuildDatabase(string databasePath)
//        {
//            SQLiteConnection.CreateFile(databasePath);
//        }
//        public void BuildTable(string table, IEnumerable<ColumnData> columnData, string databasePath)
//        {
//            if (Regex.Match(table, "^[a-zA-Z_][a-zA-Z0-9_]*$").Success)
//            {
//                using (SQLiteConnection conn = new SQLiteConnection(BuildDatabaseConnectionFromPath(databasePath)))
//                {
//                    conn.Open();
//                    StringBuilder sql = new StringBuilder();
//                    sql.Append($"CREATE TABLE {table} (");
//                    int ct = 0;
//                    foreach (ColumnData column in columnData)
//                    {
//                        if (ct++ != 0)
//                        {
//                            sql.Append(", ");
//                        }
//                        sql.Append(string.Concat(' ', column.Name));
//                        sql.Append(string.Concat(' ', column.DataType));
//                        if (column.IsPrimaryKey)
//                        {
//                            sql.Append(" PRIMARY KEY");
//                        }
//                        if (!column.AllowNulls)
//                        {
//                            sql.Append(" NOT NULL");
//                        }
//                        if (column.IsAutoIncrement)
//                        {
//                            sql.Append(" AUTOINCREMENT");
//                        }
//                    }
//                    sql.Append(')');
//                    SQLiteCommand cmd = new SQLiteCommand(sql.ToString(), conn);
//                    cmd.ExecuteNonQuery();
//                }
//            }
//            else
//            {
//                throw new ArgumentException("Invalid table name provided.");
//            }
//        }
//        public void BuildTableIfNotExists(string table, List<ColumnData> columnData, string databasePath)
//        {
//            if (Regex.Match(table, "^[a-zA-Z_][a-zA-Z0-9_]*$").Success)
//            {
//                using (SQLiteConnection conn = new SQLiteConnection(BuildDatabaseConnectionFromPath(databasePath)))
//                {
//                    conn.Open();
//                    StringBuilder sql = new StringBuilder();
//                    sql.Append($"CREATE TABLE IF NOT EXISTS {table} (");
//                    int ct = 0;
//                    foreach (ColumnData column in columnData)
//                    {
//                        if (ct != 0)
//                        {
//                            sql.Append(", ");
//                        }
//                        sql.Append(string.Concat(' ', column.Name));
//                        sql.Append(string.Concat(' ', column.DataType.ToString()));
//                        if (column.IsPrimaryKey)
//                        {
//                            sql.Append(" PRIMARY KEY");
//                        }
//                        if (column.IsAutoIncrement)
//                        {
//                            sql.Append(" AUTOINCREMENT");
//                        }
//                        if (!string.IsNullOrEmpty(column.DefaultValue))
//                        {
//                            if (column.DefaultValue.IsNumeric())
//                            {
//                                sql.Append($" DEFAULT {column.DefaultValue}");
//                            }
//                            sql.Append($" DEFAULT '{column.DefaultValue}'");
//                        }
//                    }
//                    sql.Append(')');
//                    SQLiteCommand cmd = new SQLiteCommand(sql.ToString(), conn);
//                    cmd.ExecuteNonQuery();
//                }
//            }
//            else
//            {
//                throw new ArgumentException("Invalid table name provided.");
//            }
//        }
//        public void ClearTable(string table, string databasePath)
//        {
//            if (Regex.Match(table, "^[a-zA-Z_][a-zA-Z0-9_]*$").Success)
//            {
//                using (SQLiteConnection conn = new SQLiteConnection(BuildDatabaseConnectionFromPath(databasePath)))
//                {
//                    conn.Open();
//                    string sql = $"DELETE FROM {table}";
//                    using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
//                    {
//                        cmd.ExecuteNonQuery();
//                    }
//                    sql = $@"
//                    UPDATE sqlite_sequence
//                    SET seq = 0
//                    WHERE name = 'Transactions'";
//                    using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
//                    {
//                        cmd.ExecuteNonQuery();
//                    }
//                }
//            }
//            else
//            {
//                throw new ArgumentException("Invalid table name provided.");
//            }
//        }
//        public void DropTable(string table, string databasePath)
//        {

//            if (Regex.Match(table, "^[a-zA-Z_][a-zA-Z0-9_]*$").Success)
//            {
//                using (SQLiteConnection conn = new SQLiteConnection(BuildDatabaseConnectionFromPath(databasePath)))
//                {
//                    conn.Open();
//                    string sql = $"DROP TABLE {table}";
//                    SQLiteCommand cmd = new SQLiteCommand(sql, conn);
//                    cmd.ExecuteNonQuery();
//                }
//            }
//            else
//            {
//                throw new ArgumentException("Invalid table name provided.");
//            }
//        }
//        public void DropTableIfExists(string table, string databasePath)
//        {

//            if (Regex.Match(table, "^[a-zA-Z_][a-zA-Z0-9_]*$").Success)
//            {
//                using (SQLiteConnection conn = new SQLiteConnection(BuildDatabaseConnectionFromPath(databasePath)))
//                {
//                    conn.Open();
//                    string sql = $"DROP TABLE IF EXISTS {table}";
//                    SQLiteCommand cmd = new SQLiteCommand(sql, conn);
//                    cmd.ExecuteNonQuery();
//                }
//            }
//            else
//            {
//                throw new ArgumentException("Invalid table name provided.");
//            }
//        }
//        public IEnumerable<DataTable> ExecuteQuery(string queryText, string databasePath)
//        {
//            using (SQLiteConnection conn = new SQLiteConnection(BuildDatabaseConnectionFromPath(databasePath)))
//            {
//                List<DataTable> tables = new List<DataTable>();
//                conn.Open();
//                var queries = queryText.Split(';');
//                foreach (string query in queries)
//                {
//                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
//                    {
//                        if (query.ToLower().Contains("select"))
//                        {
//                            DataTable table = new DataTable();
//                            SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
//                            adapter.Fill(table);
//                            tables.Add(table);
//                        }
//                        else
//                        {
//                            cmd.ExecuteNonQuery();
//                        }
//                    }
//                }
//                return tables;
//            }
//        }
//        public IEnumerable<ColumnData> GetColumns(string tableName, string databasePath)
//        {
//            if (Regex.Match(tableName, "^[a-zA-Z_][a-zA-Z0-9_]*$").Success)
//            {
//                List<ColumnData> columns = new List<ColumnData>();
//                using (SQLiteConnection conn = new SQLiteConnection(BuildDatabaseConnectionFromPath(databasePath)))
//                {
//                    conn.Open();
//                    string sql = $"PRAGMA table_info({tableName})";
//                    SQLiteCommand cmd = new SQLiteCommand(sql, conn);
//                    SQLiteDataReader reader = cmd.ExecuteReader();
//                    while (reader.Read())
//                    {
//                        columns.Add(new ColumnData
//                        {
//                            Name = reader["name"].ToString(),
//                            DataType = reader["type"].ToString(),
//                            AllowNulls = !Convert.ToBoolean(reader["notnull"]),
//                            IsPrimaryKey = Convert.ToBoolean(reader["pk"]),
//                            DefaultValue = reader["dflt_value"].ToString()
//                        });
//                    }
//                    return columns;
//                }
//            }
//            else
//            {
//                throw new ArgumentException("Invalid table name provided.");
//            }
//        }
//        public IEnumerable<Item> GetItems(Item item)
//        {
//            switch (item.Type)
//            {
//                case ItemType.Folder:
//                    return GetDatabaseItems(item.Path);
//                case ItemType.Database:
//                    return GetTableItems(item.Path);
//                case ItemType.Table:
//                    return GetColumnItems(item);
//                case ItemType.Column:
//                    return GetColumnDataItems(item);
//                default:
//                    return null;
//            }
//        }
//        public IEnumerable<string> GetTables(string databasePath)
//        {
//            using (SQLiteConnection conn = new SQLiteConnection(BuildDatabaseConnectionFromPath(databasePath)))
//            {
//                conn.Open();
//                List<string> tables = new List<string>();
//                string sql = @"
//                    SELECT name FROM sqlite_master WHERE type='table'";
//                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
//                SQLiteDataReader reader = cmd.ExecuteReader();
//                while (reader.Read())
//                {
//                    tables.Add(reader["name"].ToString());
//                }
//                return tables;
//            }
//        }
//        public DataTable GetRowData(string table, string databasePath)
//        {
//            if (Regex.Match(table, "^[a-zA-Z_][a-zA-Z0-9_]*$").Success)
//            {
//                DataTable cachedData = new DataTable();
//                using (SQLiteConnection conn = new SQLiteConnection(BuildDatabaseConnectionFromPath(databasePath)))
//                {
//                    conn.Open();
//                    string sql = $"SELECT * FROM {table}";
//                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(sql, conn);
//                    adapter.Fill(cachedData);
//                    return cachedData;
//                }
//            }
//            else
//            {
//                throw new ArgumentException("Invalid table name provided.");
//            }
//        }
//        public void RenameColumns(string table, List<ColumnData> oldColumns, List<string> newColumnNames, string databasePath)
//        {
//            if (Regex.Match(table, "^[a-zA-Z_][a-zA-Z0-9_]*$").Success)
//            {
//                List<ColumnData> newColumns = oldColumns.Select((c, i) => { c.Name = newColumnNames[i]; return c; }).ToList();
//                using (SQLiteConnection conn = new SQLiteConnection(BuildDatabaseConnectionFromPath(databasePath)))
//                {
//                    string sql = $@"
//                        ALTER TABLE {table}
//                        RENAME TO {table}_temp";
//                    using (SQLiteCommand cmd = new SQLiteCommand(sql, conn))
//                    {
//                        cmd.ExecuteNonQuery();
//                    }
//                    BuildTable(table, newColumns, databasePath);
//                    StringBuilder columns = new StringBuilder($"INSERT INTO {table} (");
//                    StringBuilder values = new StringBuilder("SELECT ");
//                    for (int i = 0; i < oldColumns.Count; i++)
//                    {
//                        if (i > 0)
//                        {
//                            columns.Append(", ");
//                            values.Append(", ");
//                        }
//                        columns.Append(newColumnNames[i]);
//                        values.Append(oldColumns[i].Name);
//                    }
//                }
//            }
//        }
//        public void RenameTable(string oldTableName, string newTableName, string databasePath)
//        {
//            if (Regex.Match(newTableName, "^[a-zA-Z_][a-zA-Z0-9_]*$").Success)
//            {
//                using (SQLiteConnection conn = new SQLiteConnection(BuildDatabaseConnectionFromPath(databasePath)))
//                {
//                    string sql = $"ALTER TABLE {oldTableName} RENAME TO {newTableName}";
//                    SQLiteCommand cmd = new SQLiteCommand(sql, conn);
//                    cmd.ExecuteNonQuery();
//                }
//            }
//            else
//            {
//                throw new ArgumentException("Invalid table name provided.");
//            }
//        }
//        public void ReplaceTable(string table, List<ColumnData> oldColumns, List<ColumnData> newColumns, string databasePath)
//        {
//            if (Regex.Match(table, "^[a-zA-Z_][a-zA-Z0-9_]*$").Success)
//            {
//                using (SQLiteConnection conn = new SQLiteConnection(BuildDatabaseConnectionFromPath(databasePath)))
//                {
//                    //Begin transaction
//                    StringBuilder sql = new StringBuilder($"BEGIN TRANSACTION; ");
//                    //Use old table as temp table
//                    sql.Append("ALTER TABLE {table} RENAME TO {table}_temp;");
//                    //Create replacement table
//                    sql.Append($"CREATE TABLE {table} (");
//                    int ct = 0;
//                    foreach (ColumnData column in newColumns)
//                    {
//                        if (ct != 0)
//                        {
//                            sql.Append(", ");
//                        }
//                        sql.Append(string.Concat(' ', column.Name));
//                        sql.Append(string.Concat(' ', column.DataType));
//                        if (column.IsPrimaryKey)
//                        {
//                            sql.Append(" PRIMARY KEY");
//                        }
//                        if (!column.AllowNulls)
//                        {
//                            sql.Append(" NOT NULL");
//                        }
//                        if (column.IsAutoIncrement)
//                        {
//                            sql.Append(" AUTOINCREMENT");
//                        }
//                    }
//                    sql.Append("); ");
//                    //Fill replacement table
//                    sql.Append($"INSERT INTO {table} (");
//                    IEnumerable<ColumnData> orderedColumns = newColumns.OrderBy(c => c.Index);
//                    int oldColumnCount = oldColumns.Select(c => c.Index).Max();
//                    for (int i = 0; i < oldColumnCount; i++)
//                    {
//                        if (i != 0)
//                        {
//                            sql.Append(", ");
//                        }
//                        sql.Append(newColumns[i].Name);
//                    }
//                    sql.Append($"); SELECT ");
//                    ct = 0;
//                    foreach (string columnName in oldColumns.Select(c => c.Name))
//                    {
//                        if (ct != 0)
//                        {
//                            sql.Append(", ");
//                        }
//                        sql.Append(columnName);
//                    }
//                    sql.Append($" FROM {table}; ");
//                    //Delete temp table
//                    sql.Append($"DROP TABLE {table}_temp; transaction commit");
//                    SQLiteCommand cmd = new SQLiteCommand(sql.ToString(), conn);
//                    cmd.ExecuteNonQuery();
//                }
//            }
//            else
//            {
//                throw new ArgumentException("Invalid table name provided.");
//            }
//        }
//        public void TestQuery(string databasePath)
//        {
//            using (SQLiteConnection conn = new SQLiteConnection(BuildDatabaseConnectionFromPath(databasePath)))
//            {
//                conn.Open();
//                string sql = $"SELECT Id, strftime('%m-%Y', PostDate) as monthyear, PostDate FROM Transactions";
//                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
//                SQLiteDataReader reader = cmd.ExecuteReader();
//                while (reader.Read())
//                {
//                    string va = reader["Id"].ToString();
//                    string val = reader["monthyear"].ToString();
//                    string val2 = reader["PostDate"].ToString();
//                }
//            }

//        }
//        #endregion
//        #region Private Methods
//        private string BuildDatabaseConnectionFromPath(string dbFilePath)
//        {
//            return $@"Data Source={dbFilePath};version=3;new=False";
//        }
//        private IEnumerable<Item> GetColumnItems(Item table)
//        {
//            return GetColumns(table.Name, table.Path).Select(c => new Item
//            {
//                Name = c.Name,
//                Path = String.Concat(table.Path, ';', table.Name),
//                Type = ItemType.Column
//            });
//        }
//        private IEnumerable<Item> GetColumnDataItems(Item column)
//        {
//            Item columnData = new Item();
//            string[] info = column.Path.Split(';');
//            string path = info[0];
//            string table = info[1];
//            using (SQLiteConnection conn = new SQLiteConnection(BuildDatabaseConnectionFromPath(path)))
//            {
//                conn.Open();
//                string sql = $"PRAGMA table_info({table});";
//                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
//                SQLiteDataReader reader = cmd.ExecuteReader();
//                while (reader.Read())
//                {
//                    string val = reader["name"].ToString();
//                    string val2 = reader["type"].ToString();
//                    if (reader["name"].ToString() == column.Name)
//                    {
//                        return new Item[]
//                        {
//                            new Item
//                            {
//                                Name = reader["type"].ToString(),
//                                Path = String.Concat(path, ';', table),
//                                Type = ItemType.DataType
//                            }
//                        };
//                    }
//                }
//                return null;
//            }
//        }
//        private IEnumerable<Item> GetDatabaseItems(string databasePath)
//        {
//            return Directory.GetFiles(databasePath, "*.db").Select(f => new Item
//            {
//                Name = Path.GetFileNameWithoutExtension(f),
//                Path = Path.GetFullPath(f),
//                Type = ItemType.Database
//            });
//        }
//        private IEnumerable<Item> GetTableItems(string databasePath)
//        {
//            return GetTables(databasePath).Select(t => new Item
//            {
//                Name = t,
//                Path = databasePath,
//                Type = ItemType.Table
//            });
//        }
//        private DbType InferDbType(string dataType)
//        {
//            string dataCheck = dataType.ToLower();
//            if (string.IsNullOrEmpty(dataType))
//            {
//                return DbType.Object;
//            }
//            if (dataCheck.Contains("int"))
//            {
//                return DbType.Int32;
//            }
//            else if (dataCheck.Contains("char") || dataCheck.Contains("clob") || dataCheck.Contains("text"))
//            {
//                return DbType.String;
//            }
//            else if (dataCheck.Contains("real") || dataCheck.Contains("floa") || dataCheck.Contains("doub"))
//            {
//                return DbType.Double;
//            }
//            else
//            {
//                return DbType.Int32;
//            }
//        }
//    }
//    #endregion
//}
