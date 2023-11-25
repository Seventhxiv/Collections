using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace Collections;

public class MinionCollectible : Collectible<Companion>, ICreateable<MinionCollectible, Companion>
{
    protected override Companion excelRow { get; set; }
    public override string Name { get; init; }
    public MinionCollectible(Companion excelRow) : base(excelRow)
    {
        Name = excelRow.Singular;

        if (Services.DataGenerator.CollectibleKeyDataGenerator.minionUnlockItem.ContainsKey(excelRow.RowId))
        {
            var item = Services.DataGenerator.CollectibleKeyDataGenerator.minionUnlockItem[excelRow.RowId];
            CollectibleKey = CollectibleKeyCache.Instance.GetObject((item, true));
        }
        else
        {
            CollectibleKey = null;
        }
    }

    public static MinionCollectible Create(Companion excelRow)
    {
        return new(excelRow);
    }

    public new static string GetCollectionName()
    {
        return "Minions";
    }

    public override unsafe void UpdateObtainedState()
    {
        isObtained = UIState.Instance()->IsCompanionUnlocked(excelRow.RowId);
    }

    protected override int GetIconId()
    {
        return excelRow.Icon;
    }

    public override void Interact()
    {
        if (isObtained)
            SummonActionExecutor.SummonMinion(excelRow.RowId);
    }
}
