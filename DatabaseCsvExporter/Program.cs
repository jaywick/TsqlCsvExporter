using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DatabaseCsvExporter
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            var databaseName = "yourdatabasenamehere";
            var outputPath = @"C:\path\to\output\";

            var outputFolder = new DirectoryInfo(outputPath);
            var exporter = new Exporter();
            exporter.OnWarning += (message) => Console.WriteLine(message);

            Console.WriteLine("Starting export");
            exporter.Start(databaseName, outputFolder);
            Console.WriteLine("Export complete");
        }
    }
}
