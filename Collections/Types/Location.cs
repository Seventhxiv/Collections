namespace Collections;

public class Location
{
    public TerritoryType TerritoryType { get; set; }
    public double Xmap { get; set; }
    public double Ymap { get; set; }
    public float Xorigin { get; set; }
    public float Yorigin { get; set; }

    public Location(TerritoryType territoryType, float X, float Y)
    {
        TerritoryType = territoryType;
        Xorigin = X;
        Yorigin = Y;
        var map = territoryType.Map.Value;
        Xmap = ToMapCoordinate(X, map.SizeFactor, map.OffsetX);
        Ymap = ToMapCoordinate(Y, map.SizeFactor, map.OffsetY);
    }

    public static float ToMapCoordinate(float val, float scale, short offset)
    {
        var c = scale / 100.0f;

        val = (val + offset) * c;

        return (41.0f / c * ((val + 1024.0f) / 2048.0f)) + 1;
    }
}

