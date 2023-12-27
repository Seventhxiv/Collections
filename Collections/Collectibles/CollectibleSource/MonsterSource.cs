using Collections.Executors;

namespace Collections;

public class MonsterSource : CollectibleSource
{
    private Monster monster { get; init; }
    public MonsterSource(Monster monster)
    {
        this.monster = monster;
    }

    public override string GetName()
    {
        var name = monster.name;
        if (monster.LocationDescription is not null && monster.LocationDescription != string.Empty)
            name += " - " + monster.LocationDescription;
        return name;
    }

    private List<SourceCategory> sourceType = new List<SourceCategory>() { };
    public override List<SourceCategory> GetSourceCategories()
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
        }
        else if (monster.dutyId is not null && monster.dutyId != 0)
        {
            DutyFinderOpener.OpenRegularDuty((uint)monster.dutyId);
        }
    }

    public static int defaultIconId = 63003;
    protected override int GetIconId()
    {
        if (monster.dutyId is not null)
        {
            var contentFinderCondition = ExcelCache<ContentFinderCondition>.GetSheet().GetRow((uint)monster.dutyId);
            var contentType = contentFinderCondition.ContentType.Value;
            if (contentType is not null)
            {
                return (int)contentType.Icon;
            }
        }
        return defaultIconId;
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
