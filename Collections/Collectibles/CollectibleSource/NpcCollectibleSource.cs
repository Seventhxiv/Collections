
namespace Collections;

public class NpcCollectibleSource : CollectibleSource
{
    private ENpcResident eNpcResident { get; init; }
    public NpcCollectibleSource(ENpcResident npc)
    {
        eNpcResident = npc;
    }

    public override string GetName()
    {
        return eNpcResident.Singular;
    }

    private List<CollectibleSourceCategory> sourceType = new();
    public override List<CollectibleSourceCategory> GetSourceCategories()
    {
        return sourceType;
    }

    public override bool GetIslocatable()
    {
        return true;
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

        if (eNpcResident == null)
        {
            return null;
        }

        if (Services.DataGenerator.NpcLocationDataGenerator.npcToLocation.ContainsKey(eNpcResident.RowId))
        {
            locationEntry = Services.DataGenerator.NpcLocationDataGenerator.npcToLocation[eNpcResident.RowId];
        }

        return locationEntry;
    }

    protected override int GetIconId()
    {
        return 061104;
    }
}
