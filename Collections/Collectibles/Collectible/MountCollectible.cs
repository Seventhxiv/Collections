using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace Collections;

public class MountCollectible : Collectible<Mount>
{
    protected override Mount excelRow { get; set; }
    public override string Name { get; init; }
    public MountCollectible(Mount excelRow) : base(excelRow)
    {
        Name = excelRow.Singular;
        // Collectible key
        if (Services.DataGenerator.CollectibleKeyDataGenerator.mountUnlockItem.ContainsKey(excelRow.RowId))
        {
            var item = Services.DataGenerator.CollectibleKeyDataGenerator.mountUnlockItem[excelRow.RowId];
            CollectibleKey = new CollectibleKey(item);
        }
        else
        {
            CollectibleKey = null;
        }
    }

    public override string GetName()
    {
        return excelRow.Singular;
    }

    public override unsafe void UpdateObtainedState()
    {
        isObtained = PlayerState.Instance()->IsMountUnlocked(excelRow.RowId);
    }

    protected override int GetIconId()
    {
        return excelRow.Icon;
    }

    public new static string GetCollectionName()
    {
        return "Mounts";
    }
}
