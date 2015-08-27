using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCsvExporter
{
    internal class Database
    {
        private readonly static string ConnectionStringFormat = "server={0};Initial Catalog={1};{2}";

        private string ConnectionString { get; set; }

        public Database(string initialCatalog, string server = ".", string additionalConnectionStringDetails = "Integrated Security=True")
        {
            ConnectionString = String.Format(ConnectionStringFormat, server, initialCatalog, additionalConnectionStringDetails);
        }

        public DataTable Query(string query, Dictionary<string, string> parameters = null)
        {
            var table = new DataTable();

            var formattedQuery = query;
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    formattedQuery = formattedQuery.Replace(parameter.Key, String.Format("'{0}'", parameter.Value));
                }
            }

            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(formattedQuery, connection))
            {
                connection.Open();
                var reader = command.ExecuteReader();

                var columns = Enumerable
                    .Range(0, reader.FieldCount)
                    .Select(reader.GetName).ToList();

                columns.ForEach(x => table.Columns.Add(x, typeof(string)));

                while (reader.Read())
                {
                    var row = new List<string>();

                    foreach (var column in columns)
                        row.Add(reader[column].ToString());

                    table.Rows.Add(row.ToArray());
                }

                connection.Close();
            }

            return table;
        }
    }
}
