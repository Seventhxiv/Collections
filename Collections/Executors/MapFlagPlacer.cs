using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace Collections;

public static class MapFlagPlacer
{
    private static uint FlagIconId = 60561U;

    private static int MapCordToInternal(double coord, double scale)
        => (int)(coord - 100 - (2048 / scale)) / 2;

    public static unsafe void Place(TerritoryType territory, float xCord, float yCord)
    {
        Place(territory.RowId, territory.Map.Row, xCord, yCord, (territory.Map.Value?.SizeFactor ?? 100f) / 100f);
    }

    public static unsafe void Place(uint territoryId, uint mapId, float xCord, float yCord, double sizeFactor)
    {
        Dev.Log($"{territoryId},{mapId},{xCord},{yCord},{sizeFactor}");
        var agentMap = AgentMap.Instance();
        agentMap->IsFlagMarkerSet = 0;
        var x = MapCordToInternal(xCord, sizeFactor);
        var y = MapCordToInternal(yCord, sizeFactor);
        agentMap->SetFlagMapMarker(territoryId, mapId, xCord, yCord, FlagIconId);
        agentMap->OpenMap(mapId, territoryId);
    }
}
