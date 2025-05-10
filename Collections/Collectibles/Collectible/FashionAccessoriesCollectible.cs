using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace Collections;

public class FashionAccessoriesCollectible: Collectible<Ornament>, ICreateable<FashionAccessoriesCollectible, Ornament>
{
    public new static string CollectionName => "Fashion Accessories";

    public FashionAccessoriesCollectible(Ornament excelRow) : base(excelRow)
    {
    }

    public static FashionAccessoriesCollectible Create(Ornament excelRow)
    {
        return new(excelRow);
    }

    protected override string GetCollectionName()
    {
        return CollectionName;
    }

    protected override string GetName()
    {
        return ExcelRow.Singular.ToString();
    }

    protected override uint GetId()
    {
        return ExcelRow.RowId;
    }

    protected override string GetDescription()
    {
        // trying to use the underlying unlock item for fashion accessories, as the unlock description is cooler.
        if(CollectibleKey != null)
            return ExcelCache<ItemAdapter>.GetSheet().GetRow(CollectibleKey.Id).Value.Description.ToString();
        
        return ExcelCache<OrnamentTransient>.GetSheet().GetRow(ExcelRow.RowId).Value.Text.ToString() ?? "";
    }

    public override unsafe void UpdateObtainedState()
    {
        isObtained = PlayerState.Instance()->IsOrnamentUnlocked(ExcelRow.RowId);
    }

    protected override int GetIconId()
    {
        return ExcelRow.Icon;
    }

    public override unsafe void Interact()
    {
        if (isObtained)
            ActionManager.Instance()->UseAction(ActionType.Ornament, ExcelRow.RowId);
    }
}