using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace Collections;

public class MountCollectible : Collectible<Mount>, ICreateable<MountCollectible, Mount>
{
    protected override Mount excelRow { get; set; }
    public override string Name { get; init; }
    public MountCollectible(Mount excelRow) : base(excelRow)
    {
        Name = excelRow.Singular;

        if (Services.DataGenerator.CollectibleKeyDataGenerator.mountUnlockItem.ContainsKey(excelRow.RowId))
        {
            var item = Services.DataGenerator.CollectibleKeyDataGenerator.mountUnlockItem[excelRow.RowId];
            CollectibleKey = CollectibleKeyCache.Instance.GetObject((item, true));
        }
        else
        {
            CollectibleKey = null;
        }
    }

    public new static string GetCollectionName()
    {
        return "Mounts";
    }

    public static MountCollectible Create(Mount excelRow)
    {
        return new(excelRow);
    }

    public override unsafe void UpdateObtainedState()
    {
        isObtained = PlayerState.Instance()->IsMountUnlocked(excelRow.RowId);
    }

    protected override int GetIconId()
    {
        return excelRow.Icon;
    }

    public override void Interact()
    {
        if (isObtained)
            SummonActionExecutor.SummonMount(excelRow.RowId);
    }
}
