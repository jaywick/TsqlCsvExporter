using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCsvExporter
{
    internal class Writer
    {
        private readonly static int ExcelMaxCharsThreshold = 32767;

        private HashSet<string> _overflowedColumns = new HashSet<string>();
        public IEnumerable<string> OverflowedColumns
        {
            get { return _overflowedColumns; }
        }

        public void Save(DataTable results, string outputPath)
        {
            var config = new CsvConfiguration
            {
                QuoteAllFields = true,
            };

            using (var writer = new StreamWriter(outputPath, false))
            using (var csv = new CsvWriter(writer, config))
            {
                // write header
                foreach (DataColumn header in results.Columns)
                    csv.WriteField<string>(header.ColumnName);

                csv.NextRecord();

                // write data
                foreach (DataRow row in results.Rows)
                {
                    int columnIndex = 0;
                    foreach (var field in row.ItemArray)
                    {
                        var content = field.ToString();

                        if (content.Length > ExcelMaxCharsThreshold)
                        {
                            var columnName = results.Columns[columnIndex].ColumnName;
                            _overflowedColumns.Add(columnName);
                        }

                        ++columnIndex;
                        csv.WriteField<string>(content);
                    }

                    csv.NextRecord();
                }
            }
        }
    }
}
