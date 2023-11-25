using Dalamud.Utility;
using Lumina.Data.Files;

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

        // TODO attempt reload after some time
        if (!iconScheduled)
        {
            iconScheduled = true;
            Task.Run(() => iconInternal = getIcon(iconId, false));
        }

        return null;
    }
    public IDalamudTextureWrap GetIcon()
    {
        return getIcon(iconId, false);
    }
    public static IDalamudTextureWrap getIcon(int iconId, bool hq = false)
    {
        var iconPath = getIconPath(iconId, hq);
        var tex = Services.DataManager.GetFile<TexFile>(iconPath)!;
        return Services.PluginInterface.UiBuilder.LoadImageRaw(
           tex.GetRgbaImageData(), tex.Header.Width, tex.Header.Height, 4);

        // Can technically be replaced with official API since v9:
        //return Services.TextureProvider.GetIcon((uint)iconId, Dalamud.Plugin.Services.ITextureProvider.IconFlags.HiRes);
    }

    private static string getIconPath(int iconId, bool hq)
    {
        var lang = "";
        return string.Format("ui/icon/{0:D3}000/{1}{2:D6}_hr1.tex",
            iconId / 1000, (hq ? "hq/" : "") + lang, iconId);
    }
}
