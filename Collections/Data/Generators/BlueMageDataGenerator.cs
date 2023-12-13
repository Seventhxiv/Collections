namespace Collections;

public class BlueSpell
{
    public uint ActionId { get; set; }
    public string MobDescription { get; set; }
    public string LocationDescription { get; set; }
    public uint? DutyId { get; set; }
    public uint? TerritoryId { get; set; }
    public float? X { get; set; }
    public float? Y { get; set; }
}

public class BlueMageDataGenerator
{
    public Dictionary<uint, Monster> ActionIdToBlueSpell { get; set; } = new();
    public BlueMageDataGenerator()
    {
        PopulateData();
    }

    private static readonly string FileName = "BlueSpells.csv";
    private void PopulateData()
    {
        var data = CSVHandler.Load<BlueSpell>(FileName);
        ActionIdToBlueSpell = data
            .GroupBy(entry => entry.ActionId)
            .ToDictionary(kv => kv.Key, kv => {
                var blueSpell = kv.First();
                return new Monster()
                    {
                        name = blueSpell.MobDescription,
                        LocationDescription = blueSpell.LocationDescription,
                        dutyId = blueSpell.DutyId,
                        territoryId = blueSpell.TerritoryId,
                        X = blueSpell.X,
                        Y = blueSpell.Y,
                    };
                });
    }
}

