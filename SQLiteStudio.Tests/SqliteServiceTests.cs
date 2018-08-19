using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLiteStudio.Services;
using SQLiteStudio.Services.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace SQLiteStudio.Tests
{
    [TestClass]
    public class SqliteServiceTests
    {
        string _databasePath = @"D:\Dev\Database\home.db";
        string _testTable = "TestTable";
        SqliteService _sqliteService = new SqliteService();
        [TestInitialize]
        public void InitializeTestTable()
        {
            _sqliteService.BuildTableIfNotExists(_testTable, new List<ColumnData> { new ColumnData { Name = "TestName", DataType = "character" } }, _databasePath);
        }
        #region BuildTable
        [TestMethod]
        public void BuildTable_WhenTableExists_ThrowsException()
        {
            //ARRANGE
            string validTableName = "ValidTable";
            //ACT
            try
            {
                _sqliteService.BuildTableIfNotExists(validTableName, new List<ColumnData> { new ColumnData { Name = "Test", DataType = "Varchar" } }, _databasePath);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
            //ASSERT
            Assert.ThrowsException<SQLiteException>(() => _sqliteService.BuildTable(validTableName, new List<ColumnData> { new ColumnData { Name = "Test", DataType = "Varchar" } }, _databasePath));
            try
            {
                _sqliteService.DropTableIfExists(validTableName, _databasePath);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }
        [TestMethod]
        public void BuildTable_WhenTableNotExists_CreatesAndDestroysTableWithoutExceptions()
        {
            //ARRANGE
            string validTableName = "_ValidTable";
            //ACT
            try
            {
                _sqliteService.BuildTable(validTableName, new List<ColumnData> { new ColumnData { Name = "Test", DataType = "Varchar" } }, _databasePath);
                _sqliteService.DropTable(validTableName, _databasePath);
            }
            //Assert
            catch (Exception)
            {
                Assert.Fail();
            }
        }
        [TestMethod]
        public void BuildTable_WhenTableNameIsInvalidFormat_ThrowsArgumentException()
        {
            //ARRANGE
            string invalidTableName = "9234InvalidTable";
            //ACT
            //ASSERT
            Assert.ThrowsException<ArgumentException>(() => _sqliteService.BuildTable(invalidTableName, new List<ColumnData> { new ColumnData { Name = "Test", DataType = "Varchar" } }, _databasePath));
        }
        #endregion
        #region ExecuteQuery
        [TestMethod]
        public void InsertStatement_WhenTableExists_InsertsData()
        {
            try
            {
                //ACT
                _sqliteService.ExecuteQuery("INSERT INTO TestTable (TestName) Values ('for')", _databasePath);
                DataTable table = _sqliteService.GetRowData("TestTable", _databasePath);
                //ASSERT
                Assert.IsNotNull(table.Rows[0]);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }
        #endregion

        [TestMethod]
        public void GetColumnDataItems_WhenCalledOnTestTable_ReturnsValues()
        {
            try
            {
                //ARRANGE:
                IEnumerable<ColumnData> columns = _sqliteService.GetColumns(_testTable, _databasePath);
                //ACT:
                if (columns.FirstOrDefault() is ColumnData column)
                {
                    IEnumerable<Item> columnData = _sqliteService.GetItems(new Item
                    {
                        Name = column.Name,
                        Path = String.Concat(_databasePath, ';', _testTable),
                        Type = ItemType.Column
                    });
                    Assert.IsNotNull(columnData.FirstOrDefault());
                }
                else
                {
                    Assert.Fail();
                }

            }
            catch (Exception ex)
            {
                Assert.Fail();
            }
        }
        #region GetRowData
        [TestMethod]
        public void GetRowData_WhenTableHasData_ReturnsFilledTable()
        {
            //ARRANGE:
            //ACT:

        }
        #endregion
        #region GetTables
        [TestMethod]
        public void GetTables_ReturnsTables()
        {
            try
            {
                //ACT:
                IEnumerable<string> tables = _sqliteService.GetTables(_databasePath);
                //Assert:
                Assert.IsNotNull(tables.First());
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }
        #endregion
        #region GetColumns
        [TestMethod]
        public void GetColumns_WhenCalledOnTestTable_ReturnsValues()
        {
            try
            {
                //ACT:
                IEnumerable<ColumnData> columnData = _sqliteService.GetColumns(_testTable, _databasePath);
                //ASSERT:
                Assert.IsNotNull(columnData.FirstOrDefault());
            }
            catch (Exception ex)
            {
                Assert.Fail();
            }

        }
        #endregion
    }
}
