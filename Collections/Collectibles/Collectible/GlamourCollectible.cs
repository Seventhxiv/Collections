using FFXIVClientStructs.FFXIV.Component.Excel;

namespace Collections;

public class GlamourCollectible : Collectible<ItemAdapter>, ICreateable<GlamourCollectible, ItemAdapter>
{
    public new static string CollectionName => "Glamour";

    public GlamourCollectible(ItemAdapter excelRow) : base(excelRow)
    {
        SortOptions.Add(new CollectibleSortOption("Dye Channels", Comparer<ICollectible>.Create((c1, c2) => ((GlamourCollectible)c1).ExcelRow.DyeCount.CompareTo(((GlamourCollectible)c2).ExcelRow.DyeCount)), false, null));
        SortOptions.Add(new CollectibleSortOption("Model", Comparer<ICollectible>.Create((c1, c2) => ((GlamourCollectible)c1).ExcelRow.ModelMain.CompareTo(((GlamourCollectible)c2).ExcelRow.ModelMain)), false, null));
    }

    public static GlamourCollectible Create(ItemAdapter excelRow)
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
        return "";
    }

    protected override HintModule GetSecondaryHint()
    {
        return new HintModule($"{ExcelRow.ClassJobCategory.Value.Name}, Lv. {ExcelRow.LevelEquip}", null);
    }

    public override void UpdateObtainedState()
    {
        isObtained = Services.ItemFinder.IsItemInInventory(ExcelRow.RowId)
                    || Services.ItemFinder.IsItemInArmoireCache(ExcelRow.RowId)
                    || Services.ItemFinder.IsItemInDresser(ExcelRow.RowId);
    }

    protected override int GetIconId()
    {
        return ExcelRow.Icon;
    }
    public int GetNumberOfDyeSlots()
    {
        return ExcelRow.DyeCount;
    }

    public override void Interact()
    {
        // Do nothing - taken care of by event service (should probably unify this in some way)
    }
}
