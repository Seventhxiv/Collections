namespace Collections;

public class GlamourCollectible : Collectible<ItemAdapter>, ICreateable<GlamourCollectible, ItemAdapter>
{
    protected override ItemAdapter excelRow { get; set; }
    public override string Name { get; init; }
    public GlamourCollectible(ItemAdapter excelRow) : base(excelRow)
    {
        CollectibleKey = CollectibleKeyCache.Instance.GetObject((excelRow, true));
        this.excelRow = excelRow;
        Name = excelRow.Name;
    }

    public static GlamourCollectible Create(ItemAdapter excelRow)
    {
        return new(excelRow);
    }

    public new static string GetCollectionName()
    {
        return "Glamour";
    }

    public override void UpdateObtainedState()
    {
        isObtained = Services.ItemFinder.IsItemInInventory(excelRow.RowId)
                    || Services.ItemFinder.IsItemInArmoire(excelRow.RowId)
                    || Services.ItemFinder.IsItemInDresser(excelRow.RowId);
    }

    protected override int GetIconId()
    {
        return excelRow.Icon;
    }

    public override void Interact()
    {
        // Do nothing - taken care of by event service (should probably unify this in some way)
    }
}
