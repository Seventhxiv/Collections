using Lumina.Data.Files;
using Lumina.Data.Parsing.Layer;
using LuminaSupplemental.Excel.Model;

namespace Collections;

public class NpcLocationDataGenerator
{
    public Dictionary<uint, Location> npcToLocation = new();

    public NpcLocationDataGenerator()
    {
        Task.Run(PopulateData);
    }

    private void PopulateData()
    {
        Dev.Start();

        // Build NPC locations from levels
        var levels = ExcelCache<Level>.GetSheet()!;
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

            npcToLocation[npcRowId] = new Location(level.Territory.Value, level.X, level.Z);

        }

        // Build NPC locations from territory sheet
        var territorySheet = ExcelCache<TerritoryType>.GetSheet()!;
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

                    npcToLocation[npcRowId] = new Location(sTerritoryType, instanceObject.Transform.Translation.X, instanceObject.Transform.Translation.Z);
                }
            }
        }

        // Inject from CSV
        var eNpcPlaces = CsvLoader.LoadResource<ENpcPlace>(CsvLoader.ENpcPlaceResourceName, out var failedLines);
        foreach (var entry in eNpcPlaces)
        {
            if (npcToLocation.ContainsKey(entry.ENpcResidentId))
            {
                continue;
            }

            var territoryType = ExcelCache<TerritoryType>.GetSheet().GetRow(entry.TerritoryTypeId);
            if (territoryType == null)
            {
                continue;
            }

            npcToLocation[entry.ENpcResidentId] = new Location(territoryType, entry.Position.X, entry.Position.Y);
        }

        // Inject from manual overrides
        foreach (var (npcId, (territoryId, X, Y)) in DataOverrides.NpcBaseIdToLocation)
        {
            if (npcToLocation.ContainsKey(npcId))
            {
                continue;
            }

            var territoryType = ExcelCache<TerritoryType>.GetSheet().GetRow(territoryId);
            if (territoryType == null)
            {
                continue;
            }

            npcToLocation[npcId] = new Location(territoryType, (float)X, (float)Y);
        }
        Dev.Stop();
    }
}
