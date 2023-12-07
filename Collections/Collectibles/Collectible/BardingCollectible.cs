using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace Collections;

public class BardingCollectible : Collectible<BuddyEquip>, ICreateable<BardingCollectible, BuddyEquip>
{
    public new static string CollectionName => "Bardings";

    public BardingCollectible(BuddyEquip excelRow) : base(excelRow)
    {
    }

    public static BardingCollectible Create(BuddyEquip excelRow)
    {
        return new(excelRow);
    }

    protected override string GetCollectionName()
    {
        return CollectionName;
    }

    protected override string GetName()
    {
        return ExcelRow.Name;
    }

    protected override uint GetId()
    {
        return ExcelRow.RowId;
    }

    public override unsafe void UpdateObtainedState()
    {
        isObtained = UIState.Instance()->Buddy.IsBuddyEquipUnlocked(ExcelRow.RowId);
    }

    protected override int GetIconId()
    {
        return ExcelRow.IconBody != 0 ? ExcelRow.IconBody : ExcelRow.IconHead;
    }

    public override void Interact()
    {
        // Do nothing
    }
}
