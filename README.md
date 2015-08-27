# TsqlCsvExporter
Exports Microsoft SQL Server Database to CSV

If you've ever tried to use export tools from MS SQL Server, you're guaranteed to have run into the same frustrations that I had.

## Usage
In `Program.Main()` update `databaseName` and `outputPath` to save CSV file outputs of tables from the database of the local server.

If you want to customise your database connection string, check out the call to `Database` constructor.

    public Database(string initialCatalog, string server = ".", string additionalConnectionStringDetails = "Integrated Security=True")

## How it works 
Just check out the `Exporter.Start()` method! It should be self-explainatory.

```csharp
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
```
