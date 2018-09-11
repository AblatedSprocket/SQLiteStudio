using Microsoft.Data.Sqlite;
using SQLiteStudio.Services.Models;
using SQLiteStudio.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SQLiteStudio.Services
{
    public interface ISqlService
    {
        void BuildDatabase(string databasePath);
        void BuildTable(string table, IEnumerable<ColumnData> columnData, string databasePath);
        void ClearTable(string table, string databasePath);
        void DropTable(string table, string databasePath);
        IEnumerable<DataTable> ExecuteQuery(string query, string databasePath);
        IEnumerable<ColumnData> GetColumns(string table, string databasePath);
        IEnumerable<Item> GetItems(Item item);
        IEnumerable<string> GetTables(string databasePath);
        DataTable GetRowData(string table, string databasePath);
        void RenameTable(string oldTableName, string newTableName, string databasePath);
        void ReplaceTable(string table, List<ColumnData> oldColumns, List<ColumnData> newColumns, string databasePath);
    }
    public class SqliteService : ISqlService
    {
        #region Public Methods
        static SqliteService()
        {
            SQLitePCL.Batteries.Init();
        }
        public void BuildDatabase(string databasePath)
        {
            File.Create(databasePath);
        }
        public void BuildTable(string table, IEnumerable<ColumnData> columnData, string databasePath)
        {
            if (Regex.Match(table, "^[a-zA-Z_][a-zA-Z0-9_]*$").Success)
            {
                using (SqliteConnection conn = new SqliteConnection(BuildDatabaseConnectionFromPath(databasePath)))
                {
                    conn.Open();
                    StringBuilder sql = new StringBuilder();
                    sql.Append($"CREATE TABLE {table} (");
                    int ct = 0;
                    foreach (ColumnData column in columnData)
                    {
                        if (ct++ != 0)
                        {
                            sql.Append(", ");
                        }
                        sql.Append(string.Concat(' ', column.Name));
                        sql.Append(string.Concat(' ', column.DataType));
                        if (column.IsPrimaryKey)
                        {
                            sql.Append(" PRIMARY KEY");
                        }
                        if (!column.AllowNulls)
                        {
                            sql.Append(" NOT NULL");
                        }
                        if (column.IsAutoIncrement)
                        {
                            sql.Append(" AUTOINCREMENT");
                        }
                    }
                    sql.Append(')');
                    //string sql = @"
                    //CREATE TABLE IF NOT EXISTS Transactions (
                    //    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    //    Vendor VARCHAR(20) NULL,
                    //    Amount DECIMAL NOT NULL,
                    //    Type VARCHAR(10) NOT NULL,
                    //    Category VARCHAR(20) NULL,
                    //    TransactionDate DATE NULL,
                    //    PostDate DATE NOT NULL,
                    //    Description VARCHAR(255) NOT NULL,
                    //    Account VARCHAR(10) NOT NULL,
                    //    SerialNumber VARCHAR(10) NOT NULL
                    //)";
                    SqliteCommand cmd = new SqliteCommand(sql.ToString(), conn);
                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                throw new ArgumentException("Invalid table name provided.");
            }
        }
        public void BuildTableIfNotExists(string table, List<ColumnData> columnData, string databasePath)
        {
            if (Regex.Match(table, "^[a-zA-Z_][a-zA-Z0-9_]*$").Success)
            {
                using (SqliteConnection conn = new SqliteConnection(BuildDatabaseConnectionFromPath(databasePath)))
                {
                    conn.Open();
                    StringBuilder sql = new StringBuilder();
                    sql.Append($"CREATE TABLE IF NOT EXISTS {table} (");
                    int ct = 0;
                    foreach (ColumnData column in columnData)
                    {
                        if (ct++ != 0)
                        {
                            sql.Append(", ");
                        }
                        sql.Append(string.Concat(' ', column.Name));
                        sql.Append(string.Concat(' ', column.DataType.ToString()));
                        if (column.IsPrimaryKey)
                        {
                            sql.Append(" PRIMARY KEY");
                        }
                        if (column.IsAutoIncrement)
                        {
                            sql.Append(" AUTOINCREMENT");
                        }
                        if (!string.IsNullOrEmpty(column.DefaultValue))
                        {
                            if (column.DefaultValue.IsNumeric())
                            {
                                sql.Append($" DEFAULT {column.DefaultValue}");
                            }
                            sql.Append($" DEFAULT '{column.DefaultValue}'");
                        }
                    }
                    sql.Append(')');
                    //Maxlength cannot be specified along with primary key
                    //string sql = @"
                    //CREATE TABLE IF NOT EXISTS Transactions (
                    //    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    //    Vendor VARCHAR(20) NULL,
                    //    Amount DECIMAL NOT NULL,
                    //    Type VARCHAR(10) NOT NULL,
                    //    Category VARCHAR(20) NULL,
                    //    TransactionDate DATE NULL,
                    //    PostDate DATE NOT NULL,
                    //    Description VARCHAR(255) NOT NULL,
                    //    Account VARCHAR(10) NOT NULL,
                    //    SerialNumber VARCHAR(10) NOT NULL
                    //)";
                    SqliteCommand cmd = new SqliteCommand(sql.ToString(), conn);
                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                throw new ArgumentException("Invalid table name provided.");
            }
        }
        public void ClearTable(string table, string databasePath)
        {
            if (Regex.Match(table, "^[a-zA-Z_][a-zA-Z0-9_]*$").Success)
            {
                using (SqliteConnection conn = new SqliteConnection(BuildDatabaseConnectionFromPath(databasePath)))
                {
                    conn.Open();
                    string sql = $"DELETE FROM {table}";
                    using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    sql = $@"
                    UPDATE sqlite_sequence
                    SET seq = 0
                    WHERE name = 'Transactions'";
                    using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                throw new ArgumentException("Invalid table name provided.");
            }
        }
        public void DropTable(string table, string databasePath)
        {

            if (Regex.Match(table, "^[a-zA-Z_][a-zA-Z0-9_]*$").Success)
            {
                using (SqliteConnection conn = new SqliteConnection(BuildDatabaseConnectionFromPath(databasePath)))
                {
                    conn.Open();
                    string sql = $"DROP TABLE {table}";
                    SqliteCommand cmd = new SqliteCommand(sql, conn);
                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                throw new ArgumentException("Invalid table name provided.");
            }
        }
        public void DropTableIfExists(string table, string databasePath)
        {

            if (Regex.Match(table, "^[a-zA-Z_][a-zA-Z0-9_]*$").Success)
            {
                using (SqliteConnection conn = new SqliteConnection(BuildDatabaseConnectionFromPath(databasePath)))
                {
                    conn.Open();
                    string sql = $"DROP TABLE IF EXISTS {table}";
                    SqliteCommand cmd = new SqliteCommand(sql, conn);
                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                throw new ArgumentException("Invalid table name provided.");
            }
        }
        public IEnumerable<DataTable> ExecuteQuery(string queryText, string databasePath)
        {
            using (SqliteConnection conn = new SqliteConnection(BuildDatabaseConnectionFromPath(databasePath)))
            {
                List<DataTable> tables = new List<DataTable>();
                conn.Open();
                var queries = queryText.Split(';');
                foreach (string query in queries)
                {
                    using (SqliteCommand cmd = new SqliteCommand(query, conn))
                    {
                        if (query.ToLower().Contains("select"))
                        {
                            DataTable table = new DataTable();
                            SqliteDataReader reader = cmd.ExecuteReader();
                            table.Load(reader);
                            tables.Add(table);
                        }
                        else
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                return tables;
            }
        }
        public IEnumerable<ColumnData> GetColumns(string tableName, string databasePath)
        {
            if (Regex.Match(tableName, "^[a-zA-Z_][a-zA-Z0-9_]*$").Success)
            {
                List<ColumnData> columns = new List<ColumnData>();
                using (SqliteConnection conn = new SqliteConnection(BuildDatabaseConnectionFromPath(databasePath)))
                {
                    conn.Open();
                    string sql = $"PRAGMA table_info({tableName})";
                    SqliteCommand cmd = new SqliteCommand(sql, conn);
                    SqliteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        columns.Add(new ColumnData
                        {
                            Name = reader["name"].ToString(),
                            DataType = reader["type"].ToString(),
                            AllowNulls = !Convert.ToBoolean(reader["notnull"]),
                            IsPrimaryKey = Convert.ToBoolean(reader["pk"]),
                            DefaultValue = reader["dflt_value"].ToString()
                        });
                    }
                    return columns;
                }
            }
            else
            {
                throw new ArgumentException("Invalid table name provided.");
            }
        }
        public IEnumerable<Item> GetItems(Item item)
        {
            switch (item.Type)
            {
                case ItemType.Folder:
                    return GetDatabaseItems(item.Path);
                case ItemType.Database:
                    return GetTableItems(item.Path);
                case ItemType.Table:
                    return GetColumnItems(item);
                case ItemType.Column:
                    return GetColumnDataItems(item);
                default:
                    return null;
            }
        }
        public IEnumerable<string> GetTables(string databasePath)
        {
            using (SqliteConnection conn = new SqliteConnection(BuildDatabaseConnectionFromPath(databasePath)))
            {
                conn.Open();
                List<string> tables = new List<string>();
                string sql = @"
                    SELECT name FROM sqlite_master WHERE type='table'";
                SqliteCommand cmd = new SqliteCommand(sql, conn);
                SqliteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    tables.Add(reader["name"].ToString());
                }
                return tables;
            }
        }
        public DataTable GetRowData(string table, string databasePath)
        {
            if (Regex.Match(table, "^[a-zA-Z_][a-zA-Z0-9_]*$").Success)
            {
                DataSet dataSet = new DataSet();
                dataSet.Tables.Add(new DataTable());
                dataSet.EnforceConstraints = false;
                using (SqliteConnection conn = new SqliteConnection(BuildDatabaseConnectionFromPath(databasePath)))
                {
                    conn.Open();
                    string sql = $"SELECT * FROM {table}";
                    SqliteCommand cmd = new SqliteCommand(sql, conn);
                    SqliteDataReader reader = cmd.ExecuteReader();
                    dataSet.Tables[0].Load(reader);
                    return dataSet.Tables[0];
                }
            }
            else
            {
                throw new ArgumentException("Invalid table name provided.");
            }
        }
        public void RenameColumns(string table, List<ColumnData> oldColumns, List<string> newColumnNames, string databasePath)
        {
            if (Regex.Match(table, "^[a-zA-Z_][a-zA-Z0-9_]*$").Success)
            {
                List<ColumnData> newColumns = oldColumns.Select((c, i) => { c.Name = newColumnNames[i]; return c; }).ToList();
                using (SqliteConnection conn = new SqliteConnection(BuildDatabaseConnectionFromPath(databasePath)))
                {
                    string sql = $@"
                        ALTER TABLE {table}
                        RENAME TO {table}_temp";
                    using (SqliteCommand cmd = new SqliteCommand(sql, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    BuildTable(table, newColumns, databasePath);
                    StringBuilder columns = new StringBuilder($"INSERT INTO {table} (");
                    StringBuilder values = new StringBuilder("SELECT ");
                    for (int i = 0; i < oldColumns.Count; i++)
                    {
                        if (i > 0)
                        {
                            columns.Append(", ");
                            values.Append(", ");
                        }
                        columns.Append(newColumnNames[i]);
                        values.Append(oldColumns[i].Name);
                    }
                }
            }
        }
        public void RenameTable(string oldTableName, string newTableName, string databasePath)
        {
            if (Regex.Match(newTableName, "^[a-zA-Z_][a-zA-Z0-9_]*$").Success)
            {
                using (SqliteConnection conn = new SqliteConnection(BuildDatabaseConnectionFromPath(databasePath)))
                {
                    string sql = $"ALTER TABLE {oldTableName} RENAME TO {newTableName}";
                    SqliteCommand cmd = new SqliteCommand(sql, conn);
                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                throw new ArgumentException("Invalid table name provided.");
            }
        }
        public void ReplaceTable(string table, List<ColumnData> oldColumns, List<ColumnData> newColumns, string databasePath)
        {
            if (Regex.Match(table, "^[a-zA-Z_][a-zA-Z0-9_]*$").Success)
            {
                using (SqliteConnection conn = new SqliteConnection(BuildDatabaseConnectionFromPath(databasePath)))
                {
                    //Begin transaction
                    StringBuilder sql = new StringBuilder($"BEGIN TRANSACTION; ");
                    //Use old table as temp table
                    sql.Append("ALTER TABLE {table} RENAME TO {table}_temp;");
                    //Create replacement table
                    sql.Append($"CREATE TABLE {table} (");
                    int ct = 0;
                    foreach (ColumnData column in newColumns)
                    {
                        if (ct != 0)
                        {
                            sql.Append(", ");
                        }
                        sql.Append(string.Concat(' ', column.Name));
                        sql.Append(string.Concat(' ', column.DataType));
                        if (column.IsPrimaryKey)
                        {
                            sql.Append(" PRIMARY KEY");
                        }
                        if (!column.AllowNulls)
                        {
                            sql.Append(" NOT NULL");
                        }
                        if (column.IsAutoIncrement)
                        {
                            sql.Append(" AUTOINCREMENT");
                        }
                    }
                    sql.Append("); ");
                    //Fill replacement table
                    sql.Append($"INSERT INTO {table} (");
                    IEnumerable<ColumnData> orderedColumns = newColumns.OrderBy(c => c.Index);
                    int oldColumnCount = oldColumns.Select(c => c.Index).Max();
                    for (int i = 0; i < oldColumnCount; i++)
                    {
                        if (i != 0)
                        {
                            sql.Append(", ");
                        }
                        sql.Append(newColumns[i].Name);
                    }
                    sql.Append($"); SELECT ");
                    ct = 0;
                    foreach (string columnName in oldColumns.Select(c => c.Name))
                    {
                        if (ct != 0)
                        {
                            sql.Append(", ");
                        }
                        sql.Append(columnName);
                    }
                    sql.Append($" FROM {table}; ");
                    //Delete temp table
                    sql.Append($"DROP TABLE {table}_temp; transaction commit");
                    SqliteCommand cmd = new SqliteCommand(sql.ToString(), conn);
                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                throw new ArgumentException("Invalid table name provided.");
            }
        }
        public void TestQuery(string databasePath)
        {
            using (SqliteConnection conn = new SqliteConnection(BuildDatabaseConnectionFromPath(databasePath)))
            {
                conn.Open();
                string sql = $"SELECT Id, strftime('%m-%Y', PostDate) as monthyear, PostDate FROM Transactions";
                SqliteCommand cmd = new SqliteCommand(sql, conn);
                SqliteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string va = reader["Id"].ToString();
                    string val = reader["monthyear"].ToString();
                    string val2 = reader["PostDate"].ToString();
                }
            }

        }
        #endregion
        #region Private Methods
        private string BuildDatabaseConnectionFromPath(string dbFilePath)
        {
            //@"Data Source=C:\Data\Database\home.db;version=3;new=False"
            SqliteConnectionStringBuilder builder = new SqliteConnectionStringBuilder();
            builder.DataSource = dbFilePath;
            builder.Mode = SqliteOpenMode.ReadWriteCreate;

            return builder.ToString();
        }
        private IEnumerable<Item> GetColumnItems(Item table)
        {
            return GetColumns(table.Name, table.Path).Select(c => new Item
            {
                Name = c.Name,
                Path = String.Concat(table.Path, ';', table.Name),
                Type = ItemType.Column
            });
        }
        private IEnumerable<Item> GetColumnDataItems(Item column)
        {
            Item columnData = new Item();
            string[] info = column.Path.Split(';');
            string path = info[0];
            string table = info[1];
            using (SqliteConnection conn = new SqliteConnection(BuildDatabaseConnectionFromPath(path)))
            {
                conn.Open();
                string sql = $"PRAGMA table_info({table});";
                SqliteCommand cmd = new SqliteCommand(sql, conn);
                SqliteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string val = reader["name"].ToString();
                    string val2 = reader["type"].ToString();
                    if (reader["name"].ToString() == column.Name)
                    {
                        return new Item[]
                        {
                            new Item
                            {
                                Name = reader["type"].ToString(),
                                Path = String.Concat(path, ';', table),
                                Type = ItemType.DataType
                            }
                        };
                    }
                }
                return null;
            }
        }
        private IEnumerable<Item> GetDatabaseItems(string databasePath)
        {
            return Directory.GetFiles(databasePath, "*.db").Select(f => new Item
            {
                Name = Path.GetFileNameWithoutExtension(f),
                Path = Path.GetFullPath(f),
                Type = ItemType.Database
            });
        }
        private IEnumerable<Item> GetTableItems(string databasePath)
        {
            return GetTables(databasePath).Select(t => new Item
            {
                Name = t,
                Path = databasePath,
                Type = ItemType.Table
            });
        }
        private DbType InferDbType(string dataType)
        {
            string dataCheck = dataType.ToLower();
            if (string.IsNullOrEmpty(dataType))
            {
                return DbType.Object;
            }
            if (dataCheck.Contains("int"))
            {
                return DbType.Int32;
            }
            else if (dataCheck.Contains("char") || dataCheck.Contains("clob") || dataCheck.Contains("text"))
            {
                return DbType.String;
            }
            else if (dataCheck.Contains("real") || dataCheck.Contains("floa") || dataCheck.Contains("doub"))
            {
                return DbType.Double;
            }
            else
            {
                return DbType.Int32;
            }
        }
    }
    #endregion
}
