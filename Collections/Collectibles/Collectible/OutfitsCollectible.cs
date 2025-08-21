using FFXIVClientStructs.FFXIV.Component.Excel;

namespace Collections;

public class OutfitsCollectible : Collectible<ItemAdapter>, ICreateable<OutfitsCollectible, ItemAdapter>
{
    public new static string CollectionName => "Outfits";

    public OutfitsCollectible(ItemAdapter excelRow) : base(excelRow)
    {
    }

    public static OutfitsCollectible Create(ItemAdapter excelRow)
    {
        return new(excelRow);
    }

    protected override string GetCollectionName()
    {
        return CollectionName;
    }

    protected override string GetName()
    {
        return ExcelRow.Name.ToString();
    }

    protected override uint GetId()
    {
        return ExcelRow.RowId;
    }

    protected override string GetDescription()
    {
        return ExcelRow.Description.ToString();
    }

    public override void UpdateObtainedState()
    {
        isObtained = Services.ItemFinder.IsItemInDresser(ExcelRow.RowId);
    }

    protected override int GetIconId()
    {
        return ExcelRow.Icon;
    }

    public override void Interact()
    {
        // Do nothing - taken care of by event service (should probably unify this in some way)
    }
}
