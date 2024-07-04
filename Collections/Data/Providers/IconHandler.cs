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
        var icon = Services.TextureProvider.GetFromFile(iconPath);
        var tex = Services.DataManager.GetFile<TexFile>(iconPath)!;
        return Services.TextureProvider.CreateFromTexFile(tex);
    }

    private static string getIconPath(int iconId, bool hq)
    {
        var lang = "";
        return string.Format("ui/icon/{0:D3}000/{1}{2:D6}_hr1.tex",
            iconId / 1000, (hq ? "hq/" : "") + lang, iconId);
    }
}
