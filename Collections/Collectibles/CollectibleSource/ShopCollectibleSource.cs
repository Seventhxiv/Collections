namespace Collections;

public class ShopCollectibleSource : CollectibleSource
{
    public List<(ItemCollectibleKey collectibleKey, int amount)> costItems = new();
    public ENpcResident ENpcResident { get; init; }
    public uint ShopId { get; init; }
    public ShopCollectibleSource(ShopEntry shopEntry)
    {
        ShopId = shopEntry.ShopId;
        if (shopEntry.ENpcResidentId != null)
        {
            ENpcResident = ExcelCache<ENpcResident>.GetSheet().GetRow((uint)shopEntry.ENpcResidentId);
        }
        foreach (var cost in shopEntry.Cost)
        {
            // Recursively create collectibleKey (only allow 1 degree) to allow for the currency to determine the source category
            costItems.Add((CollectibleKeyCache<ItemCollectibleKey, ItemAdapter>.Instance.GetObject((cost.Item, false)), cost.Amount));
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
            name += cost.collectibleKey.Name + " x" + cost.amount + ", ";
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
        sourceCategories = costItems.Select(cost => cost.collectibleKey).SelectMany(source => source.GetSourceCategories()).ToList();

        // Add source type of beast tribe currencies TODO
        if (ENpcResident != null)
        {
            var beastTribeNpcIds = new List<uint>() { 1016650, 1016804, 1016838 };
            if (beastTribeNpcIds.Contains(ENpcResident.RowId))
            {
                sourceCategories.Add(CollectibleSourceCategory.BeastTribes);
            }
        }

        return sourceCategories;
    }

    public override bool GetIslocatable()
    {
        return GetLocationEntry() is not null;
    }

    public override void DisplayLocation()
    {
        var locationEntry = GetLocationEntry();
        if (locationEntry is not null)
        {
            MapFlagPlacer.Place(locationEntry.TerritoryType, locationEntry.Xorigin, locationEntry.Yorigin);
        }
    }

    private bool locationChecked = false;
    private LocationEntry locationEntry;
    public LocationEntry GetLocationEntry()
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
