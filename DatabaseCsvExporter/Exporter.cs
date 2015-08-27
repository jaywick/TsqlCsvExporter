using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;

namespace DatabaseCsvExporter
{
    internal class Exporter
    {
        private readonly static string SelectAllDatabaseTablesQuery = "SELECT * FROM information_schema.tables where TABLE_TYPE = 'BASE TABLE' order by TABLE_NAME";
        private readonly static string SelectAllFromTableQueryFormat = "SELECT * FROM {0}";
        private readonly static string TableSchemaColumnName = "TABLE_SCHEMA";
        private readonly static string TableNameColumnName = "TABLE_NAME";
        private readonly static string TableNameFormat = "[{0}].[{1}]";

        private Database _database;

        public delegate void OnWarningHandler(string message);
        public event OnWarningHandler OnWarning;

        public void Start(string databaseName, DirectoryInfo outputFolder)
        {
            _database = new Database(databaseName);

            if (!outputFolder.Exists)
                outputFolder.Create();

            var tableNames = GetTableNames();

            foreach (var tableName in tableNames)
            {
                var tableData = GetTable(tableName);
                var outputPath = GetSafeOutputFilePath(outputFolder, tableName);

                var writer = new Writer();
                writer.Save(tableData, outputPath);

                if (writer.OverflowedColumns.Any() && OnWarning != null)
                {
                    foreach (var columnName in writer.OverflowedColumns)
                        OnWarning(String.Format("Warning! Overflow in table {0} column {1}", tableData, columnName));
                }
            }
        }

        private string GetSafeOutputFilePath(DirectoryInfo outputFolder, string tableName, string extension = ".csv")
        {
            var safeTableName = tableName.Replace("[", "").Replace("]", "");

            foreach (char illegalCharacter in Path.GetInvalidFileNameChars().Concat(Path.GetInvalidPathChars()))
                safeTableName = safeTableName.Replace(illegalCharacter.ToString(), "_");

            return Path.Combine(outputFolder.FullName, safeTableName + extension);
        }

        private DataTable GetTable(string tableName)
        {
            var query = String.Format(SelectAllFromTableQueryFormat, tableName);
            return _database.Query(query);
        }

        private IEnumerable<string> GetTableNames()
        {
            var query = SelectAllDatabaseTablesQuery;

            var results = _database
                .Query(query)
                .Rows
                .Cast<DataRow>()
                .ToList();

            foreach (DataRow row in results)
            {
                var schema = row[TableSchemaColumnName];
                var name = row[TableNameColumnName];

                yield return String.Format(TableNameFormat, schema, name);
            }
        }
    }
}
