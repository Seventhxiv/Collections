using System.Collections.Generic;

namespace Collections;

public class MogStationCollectibleSource : CollectibleSource
{
    public MogStationCollectibleSource()
    {
    }

    public override string GetName()
    {
        return "Mog Station Item";
    }

    private readonly List<CollectibleSourceType> sourceType = new() { CollectibleSourceType.MogStation };
    public override List<CollectibleSourceType> GetSourceType()
    {
        return sourceType;
    }

    public override bool GetIslocatable()
    {
        return false;
    }

    public override LocationEntry GetLocationEntry()
    {
        return null;
    }

    public static int iconId = 61831;
    protected override int GetIconId()
    {
        return iconId;
    }
}
