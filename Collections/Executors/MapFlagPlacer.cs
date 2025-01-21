using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace Collections;

public static class MapFlagPlacer
{
    private static uint FlagIconId = 60561U;

    private static int MapCordToInternal(double coord, double scale)
        => (int)(coord - 100 - (2048 / scale)) / 2;

    public static unsafe void Place(TerritoryType territory, float xCord, float yCord)
    {
        PlaceFromInternalCoords(territory.RowId, territory.Map.RowId, xCord, yCord);
    }

    public static unsafe void PlaceFromMapCoords(TerritoryType territory, float xCord, float yCord)
    {
        var sizeFactor = territory.Map.Value.SizeFactor / 100f;
        var x = MapCordToInternal(xCord * 100, sizeFactor);
        var y = MapCordToInternal(yCord * 100, sizeFactor);
        PlaceFromInternalCoords(territory.RowId, territory.Map.RowId, x, y);
    }

    private static unsafe void PlaceFromInternalCoords(uint territoryId, uint mapId, float xCord, float yCord)
    {
        var territory = ExcelCache<TerritoryType>.GetSheet().GetRow(territoryId);
        var sizeFactor = (territory?.Map.Value.SizeFactor ?? 100f) / 100f;
        var x = MapCordToInternal(xCord, sizeFactor);
        var y = MapCordToInternal(yCord, sizeFactor);

        Dev.Log($"TerritoryId: {territoryId} at ({xCord},{yCord}) coords, sizeFactor: {sizeFactor}, adjusted coords ({x},{y})");
        var agentMap = AgentMap.Instance();
        agentMap->IsFlagMarkerSet = 0;

        agentMap->SetFlagMapMarker(territoryId, mapId, xCord, yCord, FlagIconId);
        agentMap->OpenMap(mapId, territoryId);
    }
}
