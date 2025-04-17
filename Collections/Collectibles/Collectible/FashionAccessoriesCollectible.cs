using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Component.Excel;

namespace Collections;

public class FashionAccessoriesCollectible: Collectible<ItemAdapter>, ICreateable<FashionAccessoriesCollectible, ItemAdapter>
{
    public new static string CollectionName => "Fashion Accessories";

    public FashionAccessoriesCollectible(ItemAdapter excelRow) : base(excelRow)
    {

    }

    public static FashionAccessoriesCollectible Create(ItemAdapter excelRow)
    {
        return new(excelRow);
    }

    protected override string GetCollectionName()
    {
        return CollectionName;
    }

    protected override string GetName()
    {
        if(IsGlasses(ExcelRow))
        {
            var temp = ExcelCache<Glasses>.GetSheet().GetRow(GetAccessorySheetId());
            if(temp != null) return temp.Value.Name.ToString();
        }
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

    protected override HintModule GetPrimaryHint()
    {
        return new HintModule($"Collection ID: {ExcelRow.RowId.ToString()}", null);
    }

    protected override HintModule GetSecondaryHint()
    {
        return new HintModule("", null);
    }

    public override unsafe void UpdateObtainedState()
    {
        if(IsGlasses(ExcelRow)) isObtained = PlayerState.Instance()->IsGlassesUnlocked((ushort)GetAccessorySheetId());
        else isObtained = PlayerState.Instance()->IsOrnamentUnlocked(GetAccessorySheetId());
    }

    protected override int GetIconId()
    {
        if(IsGlasses(ExcelRow))
        {
            Glasses? temp = ExcelCache<Glasses>.GetSheet().GetRow(GetAccessorySheetId());
            if(temp != null) return temp.Value.Icon;
        }
        return ExcelRow.Icon;
    }

    public override unsafe void Interact()
    {
        // Do nothing
    }

    public uint GetAccessorySheetId()
    {
        // facewear
        if(IsGlasses(ExcelRow)) return ExcelRow.AdditionalData.RowId;
        // fashion accessories
        if((uint)ExcelRow.ItemAction.Value.Data.ElementAt(0) != 0) return (uint)ExcelRow.ItemAction.Value.Data.ElementAt(0);
        // no link found.
        return 0;
    }

    public static bool IsGlasses(ItemAdapter item)
    {
        return (item.AdditionalData.RowId != 0) && item.Unknown4 == 31000 && item.ItemUICategory.Value.Name == "Miscellany";
    }

    public static bool IsFashionAccessory(ItemAdapter item)
    {
        return item.ItemAction.Value.Type == 20086 && item.ItemUICategory.Value.Name == "Miscellany";
    }
}