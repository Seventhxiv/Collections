using Dalamud.Configuration;
using System.Collections.Generic;

namespace Collections;

public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;
    public List<bool> newPropertyList { get; set; } = new();

    public List<uint> DresserContentIds = new();

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
