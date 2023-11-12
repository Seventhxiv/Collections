using Dalamud.Interface.Internal;
using Dalamud.Utility;
using ImGuiScene;
using Lumina.Data.Files;
using System.Threading.Tasks;

namespace Collections;

public class IconHandler
{
    private int iconId { get; init; }
    public IconHandler(int iconId)
    {
        this.iconId = iconId;
    }

    private IDalamudTextureWrap iconInternal;
    private bool iconScheduled = false;
    public IDalamudTextureWrap GetIconLazy()
    {
        if (iconInternal != null)
        {
            return iconInternal;
        }

        if (!iconScheduled)
        {
            iconScheduled = true;
            Task.Run(() => iconInternal = getIcon((int)iconId, false));
        }

        return null;
    }
    public IDalamudTextureWrap GetIcon()
    {
        return getIcon((int)iconId, false);
    }
    public static IDalamudTextureWrap getIcon(int iconId, bool hq = false)
    {
        var iconPath = getIconPath(iconId, hq);
        var tex = Services.DataManager.GetFile<TexFile>(iconPath)!;
        return Services.PluginInterface.UiBuilder.LoadImageRaw(
           tex.GetRgbaImageData(), tex.Header.Width, tex.Header.Height, 4);
    }

    private static string getIconPath(int iconId, bool hq)
    {
        var lang = "";
        return string.Format("ui/icon/{0:D3}000/{1}{2:D6}_hr1.tex",
            iconId / 1000, (hq ? "hq/" : "") + lang, iconId);
    }
}
