using CsvHelper;
using ImGuiScene;
using System.Globalization;
using System.IO;

namespace Collections;

public class Helpers
{
    public static TextureWrap loadImagefromDirectory(string fileName)
    {
        var imagePath = Path.Combine(Services.PluginInterface.AssemblyLocation.Directory?.FullName!, fileName);
        return (TextureWrap)Services.PluginInterface.UiBuilder.LoadImage(imagePath);
    }

    public static List<T> LoadCSV<T>(string path)
    {
        using (var reader = new StreamReader(path))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            List<T> result = csv.GetRecords<T>().ToList();
            return result;
        }
    }
}
