using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;
using System.Text;

namespace Collections;

public static class CSVHandler
{
    public static List<T> Load<T>(string fileName, string path = @"Data\Resources\")
    {
        var fullPath = Path.Combine(Services.PluginInterface.AssemblyLocation.Directory?.FullName!, path, fileName);
        using (var reader = new StreamReader(fullPath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var result = csv.GetRecords<T>().ToList();
            return result;
        }
    }

    public static void Write<T>(List<T> data, string fileName, string path = @"Data\Resources\")
    {
        var fullPath = Path.Combine(Services.PluginInterface.AssemblyLocation.Directory?.FullName!, path, fileName);
        var csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ",",
            Encoding = Encoding.UTF8
        };

        using (var writer = new StreamWriter(fullPath))
        using (var csvWriter = new CsvWriter(writer, csvConfig))
        {
            csvWriter.WriteRecords(data);
        }
    }
}

public class ItemIdToSource
{
    public string Collection { get; set; }
    public uint ItemId { get; set; }
    public uint SourceId { get; set; }
    public string SourceDescription { get; set; }
}

