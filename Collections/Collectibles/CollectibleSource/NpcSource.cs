
namespace Collections;

public class NpcSource : CollectibleSource
{
    private ENpcResident? eNpcResident { get; init; }
    public NpcSource(ENpcResident npc)
    {
        eNpcResident = npc;
    }

    public override string GetName()
    {
        return eNpcResident.Value.Singular.ToString();
    }

    private List<SourceCategory> sourceType = new();
    public override List<SourceCategory> GetSourceCategories()
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
    private Location locationEntry;
    public Location GetLocationEntry()
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

        if (Services.DataGenerator.NpcLocationDataGenerator.npcToLocation.ContainsKey(eNpcResident.Value.RowId))
        {
            locationEntry = Services.DataGenerator.NpcLocationDataGenerator.npcToLocation[eNpcResident.Value.RowId];
        }

        return locationEntry;
    }

    protected override int GetIconId()
    {
        return 061104;
    }

    public override NpcSource Clone()
    {

        return new NpcSource(eNpcResident.Value);
    }
}
