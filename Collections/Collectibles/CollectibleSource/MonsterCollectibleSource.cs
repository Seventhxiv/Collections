using Collections.Executors;

namespace Collections;

public class MonsterCollectibleSource : CollectibleSource
{
    private Monster monster { get; init; }
    public MonsterCollectibleSource(Monster monster)
    {
        this.monster = monster;
    }

    public override string GetName()
    {
        var name = monster.name;
        if (monster.LocationDescription is not null && monster.LocationDescription != string.Empty)
            name  +=  " - " + monster.LocationDescription;
        return name;
    }

    private List<CollectibleSourceCategory> sourceType = new List<CollectibleSourceCategory>() { };
    public override List<CollectibleSourceCategory> GetSourceCategories()
    {
        return sourceType;
    }

    public override bool GetIslocatable()
    {
        return GetLocationEntry() is not null || (monster.dutyId is not null && monster.dutyId != 0);
    }

    public override void DisplayLocation()
    {
        var locationEntry = GetLocationEntry();
        if (locationEntry is not null)
        {
            MapFlagPlacer.PlaceFromMapCoords(locationEntry.TerritoryType, locationEntry.Xorigin, locationEntry.Yorigin);
        } else if (monster.dutyId is not null && monster.dutyId != 0)
        {
            DutyFinderOpener.OpenRegularDuty((uint)monster.dutyId);
        }
    }

    protected override int GetIconId()
    {
        return 068003;
    }

    private bool locationChecked = false;
    private Location? location;
    public Location GetLocationEntry()
    {
        if (locationChecked)
        {
            return location;
        }

        locationChecked = true;

        return location = monster.GetLocationEntry();
    }
}
