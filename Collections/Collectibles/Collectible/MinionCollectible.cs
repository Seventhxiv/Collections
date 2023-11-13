using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace Collections;

public class MinionCollectible : Collectible<Companion>
{
    protected override Companion excelRow { get; set; }
    public override string Name { get; init; }
    public MinionCollectible(Companion excelRow) : base(excelRow)
    {
        Name = excelRow.Singular;

        // Collectible key
        if (Services.DataGenerator.CollectibleKeyDataGenerator.minionUnlockItem.ContainsKey(excelRow.RowId))
        {
            var item = Services.DataGenerator.CollectibleKeyDataGenerator.minionUnlockItem[excelRow.RowId];
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
        isObtained = UIState.Instance()->IsCompanionUnlocked(excelRow.RowId);
    }

    protected override int GetIconId()
    {
        return excelRow.Icon;
    }

    public new static string GetCollectionName()
    {
        return "Minions";
    }
}
