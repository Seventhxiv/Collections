using System.Diagnostics;

namespace Collections;

public class GamerEscapeLink
{

    public static void OpenItem(string itemName)
    {
        var formattedName = itemName.Replace(' ', '_').Replace("'", "%27");
        Process.Start(new ProcessStartInfo()
        {
            UseShellExecute = true,
            FileName = getUrl(formattedName)
        }); ;
    }

    private static string getUrl(string formattedName) => $"https://ffxiv.gamerescape.com/wiki/{formattedName}";
}
