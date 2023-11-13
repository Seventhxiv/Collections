using Lumina.Data.Files;
using Lumina.Data.Parsing.Layer;
using System.Threading.Tasks;

namespace Collections;

public class LocationEntry
{
    public TerritoryType TerritoryType { get; set; }
    public Map Map { get; set; }
    public double Xmap { get; set; }
    public double Ymap { get; set; }
    public float Xorigin { get; set; }
    public float Yorigin { get; set; }
}

public class NpcLocationDataGenerator
{
    public Dictionary<uint, LocationEntry> npcToLocation = new();

    public NpcLocationDataGenerator()
    {
        Task.Run(() => PopulateData());
    }

    private void PopulateData()
    {
        Dev.StartStopwatch();

        // Build NPC locations from levels
        var levels = Excel.GetExcelSheet<Level>()!;
        foreach (var level in levels)
        {
            var npcRowId = level.Object;

            // NPC type
            if (level.Type != 8)
            {
                continue;
            }

            // Skip already mapped
            if (npcToLocation.ContainsKey(npcRowId))
            {
                continue;
            }

            // No territory specified
            if (level.Territory.Value == null)
            {
                continue;
            }

            var sTerritoryType = level.Territory.Value;
            var map = sTerritoryType.Map.Value;
            var x = ToMapCoordinate(level.X, map.SizeFactor, map.OffsetX);
            var y = ToMapCoordinate(level.Z, map.SizeFactor, map.OffsetY);

            npcToLocation[npcRowId] = new LocationEntry() { TerritoryType = sTerritoryType, Map = map, Xmap = x, Ymap = y, Xorigin = level.X, Yorigin = level.Z };

        }
        Dev.EndStopwatch("NPC Location from levels");

        // Build NPC locations from territory sheet

        Dev.StartStopwatch();

        var territorySheet = Excel.GetExcelSheet<TerritoryType>()!;
        foreach (var sTerritoryType in territorySheet)
        {
            var bg = sTerritoryType.Bg.ToString();
            var lgbFileName = "bg/" + bg[..(bg.IndexOf("/level/", StringComparison.Ordinal) + 1)] + "level/planevent.lgb";
            var sLgbFile = Services.DataManager.GetFile<LgbFile>(lgbFileName);
            if (sLgbFile == null)
            {
                continue;
            }

            foreach (var sLgbGroup in sLgbFile.Layers)
            {
                foreach (var instanceObject in sLgbGroup.InstanceObjects)
                {
                    if (instanceObject.AssetType != LayerEntryType.EventNPC)
                    {
                        continue;
                    }

                    var eventNpc = (LayerCommon.ENPCInstanceObject)instanceObject.Object;
                    var npcRowId = eventNpc.ParentData.ParentData.BaseId;
                    if (npcRowId == 0)
                    {
                        continue;
                    }

                    // Skip already mapped
                    if (npcToLocation.ContainsKey(npcRowId))
                    {
                        continue;
                    }

                    var map = sTerritoryType.Map.Value;
                    var x = ToMapCoordinate(instanceObject.Transform.Translation.X, map.SizeFactor, map.OffsetX);
                    var y = ToMapCoordinate(instanceObject.Transform.Translation.Z, map.SizeFactor, map.OffsetY);
                    npcToLocation[npcRowId] = new LocationEntry() { TerritoryType = sTerritoryType, Map = map, Xmap = x, Ymap = y, Xorigin = instanceObject.Transform.Translation.X, Yorigin = instanceObject.Transform.Translation.Z };
                }
            }
        }
        Dev.EndStopwatch("NPC Location from territory");
    }

    private static float ToMapCoordinate(float val, float scale, short offset)
    {
        var c = scale / 100.0f;

        val = (val + offset) * c;

        return (41.0f / c * ((val + 1024.0f) / 2048.0f)) + 1;
    }
}
