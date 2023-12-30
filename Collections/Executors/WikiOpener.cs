using System.Diagnostics;

namespace Collections;

public class WikiOpener
{
    public static void OpenGamerEscape(string itemName)
    {
        var formattedName = itemName.Replace(' ', '_').Replace("'", "%27");

        Dev.Log($"Opening Gamer Escape with {formattedName} (originally {itemName})");

        Process.Start(new ProcessStartInfo()
        {
            UseShellExecute = true,
            FileName = GamerEscapeUrl(formattedName)
        }); ;
    }

    private static string GamerEscapeUrl(string formattedName) => $"https://ffxiv.gamerescape.com/wiki/{formattedName}";
}
