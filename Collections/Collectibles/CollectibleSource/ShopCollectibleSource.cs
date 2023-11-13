namespace Collections;

public class ShopCollectibleSource : CollectibleSource
{
    public List<(CollectibleKey collectibleKey, int amount)> costItems = new();
    public ENpcResident ENpcResident { get; init; }
    public ShopCollectibleSource(ShopEntry shopEntry)
    {
        if (shopEntry.ENpcResidentId != null)
        {
            ENpcResident = Excel.GetExcelSheet<ENpcResident>().GetRow((uint)shopEntry.ENpcResidentId);
        }
        foreach (var cost in shopEntry.Cost)
        {
            // Recursively create collectibleKey (only allow 1 degree) to allow for the currency to determine the source category
            costItems.Add((new CollectibleKey(cost.Item, false), cost.Amount));
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
            name += cost.collectibleKey.item.Name + " x" + cost.amount + ", ";
        }

        // Location
        var location = GetLocationEntry();
        if (location != null)
        {
            name += "at " + location.TerritoryType.PlaceName.Value.Name.ToString();
        }

        return name;
    }

    private List<CollectibleSourceCategory> sourceCategories;
    public override List<CollectibleSourceCategory> GetSourceCategories()
    {
        if (sourceCategories != null)
        {
            return sourceCategories;
        }

        // Derive source type from all cost items
        sourceCategories = costItems.Select(cost => cost.collectibleKey).SelectMany(source => source.GetSourceTypes()).ToList();

        // Add source type of beast tribe currencies
        if (ENpcResident != null)
        {
            var npcName = ENpcResident.Singular.ToString();
            var beastTribeNpcNames = new List<string>() { "Vath Stickpeddler" };
            if (beastTribeNpcNames.Any(name => npcName.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                sourceCategories.Add(CollectibleSourceCategory.BeastTribe);
            }
        }

        return sourceCategories;
    }

    public override bool GetIslocatable()
    {
        return GetLocationEntry() is not null;
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

        if (Services.DataGenerator.NpcLocationDataGenerator.npcToLocation.ContainsKey(ENpcResident.RowId))
        {
            locationEntry = Services.DataGenerator.NpcLocationDataGenerator.npcToLocation[ENpcResident.RowId];
        }

        return locationEntry;
    }

    protected override int GetIconId()
    {
        return 60550;
    }
}
