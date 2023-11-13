using Dalamud.Configuration;

namespace Collections;

public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;
    public List<bool> newPropertyList { get; set; } = new();

    public List<uint> DresserContentIds = new();

    public bool autoOpenInstanceTab = true;

    public void Save()
    {
        Services.PluginInterface.SavePluginConfig(this);
    }

    public void updateDresserContentIds(List<uint> itemIds)
    {
        var initialSize = DresserContentIds.Count;
        DresserContentIds.Clear();
        foreach (var itemId in itemIds)
        {
            DresserContentIds.Add(itemId);
        }
        Save();
        Dev.Log($"Config Dresser list updated: {initialSize} -> {DresserContentIds.Count}");
    }
}
