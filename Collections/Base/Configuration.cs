using Dalamud.Configuration;

namespace Collections;

public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 1;
    public Dictionary<string, HashSet<uint>> Favorites = new();
    public Dictionary<string, HashSet<uint>> Wishlist = new();
    public HashSet<uint> WishListed = new();
    public List<uint> DresserItemIds = new();
    public List<uint> ArmoireItemIds = new();
    public GlamourTree GlamourTree = new();
    public bool AutoOpenInstanceTab = true;
    public bool ForceTryOn = false;

    public void Save()
    {
        Services.PluginInterface.SavePluginConfig(this);
    }

    public static Configuration GetConfig()
    {
        var config = Services.PluginInterface.GetPluginConfig() as Configuration;
        if (config is not null)
        {
            Dev.Log($"Loaded Configuration: V{config.Version}");
            return config;
        }

        Dev.Log("Initializing a new Configuration");
        return new();
    }
}
