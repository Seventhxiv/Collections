using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Collections;

public class ShopCollectibleSource : CollectibleSource
{
    public List<(CollectibleUnlockItem CollectibleUnlockItem, int amount)> costItems = new();
    public ENpcResident ENpcResident { get; init; }
    public ShopCollectibleSource(ShopEntry shopEntry)
    {
        if (shopEntry.ENpcResidentId != null)
        {
            ENpcResident = Excel.GetExcelSheet<ENpcResident>().GetRow((uint)shopEntry.ENpcResidentId);
        }
        foreach (var cost in shopEntry.Cost)
        {
            // Recursively create CollectibleUnlockItem (only allow 1 degree)
            costItems.Add((new CollectibleUnlockItem(cost.Item, false), cost.Amount));
        }
    }

    private string name;
    public override string GetName()
    {
        if (name != null)
        {
            return name;
        }

        name = "";

        // NPC name
        if (ENpcResident != null)
        {
            name += ENpcResident.Singular.ToString();
        }
        else
        {
            name += "Unknown Shop";
        }

        // Costs
        name += ": ";
        foreach (var cost in costItems)
        {
            name += cost.CollectibleUnlockItem.item.Name + " x" + cost.amount + ", ";
        }

        // Location
        var location = GetLocationEntry();
        if (location != null)
        {
            name += "at " + location.TerritoryType.PlaceName.Value.Name.ToString();
        }

        return name;
    }

    private List<CollectibleSourceType> sourceTypes;
    public override List<CollectibleSourceType> GetSourceType()
    {
        if (sourceTypes != null)
        {
            return sourceTypes;
        }

        // Derive source type from all cost items
        sourceTypes = costItems.Select(cost => cost.CollectibleUnlockItem).SelectMany(source => source.GetSourceTypes()).ToList();

        // Add source type of beast tribe currencies
        if (ENpcResident != null)
        {
            var npcName = ENpcResident.Singular.ToString();
            var beastTribeNpcNames = new List<string>() { "Vath Stickpeddler" };
            if (beastTribeNpcNames.Any(name => npcName.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                sourceTypes.Add(CollectibleSourceType.BeastTribe);
            }
        }

        return sourceTypes;
    }

    public override bool GetIslocatable()
    {
        return true;
    }

    private bool locationChecked = false;
    private LocationEntry locationEntry;
    public override LocationEntry GetLocationEntry()
    {
        if (locationChecked)
        {
            return locationEntry;
        }

        locationChecked = true;

        if (ENpcResident == null)
        {
            return null;
        }

        if (Services.DataGenerator.NpcLocationDataParser.npcToLocation.ContainsKey(ENpcResident.RowId))
        {
            locationEntry = Services.DataGenerator.NpcLocationDataParser.npcToLocation[ENpcResident.RowId];
        }

        return locationEntry;
    }

    protected override int GetIconId()
    {
        return 60550;
    }
}
