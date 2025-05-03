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
        isObtained = Services.ItemFinder.IsItemInInventory(ExcelRow.RowId)
                    || Services.ItemFinder.IsItemInDresser(ExcelRow.RowId)
                    || Services.ItemFinder.IsItemInArmoireCache(ExcelRow.RowId);
    }

    protected override int GetIconId()
    {
        return ExcelRow.Icon;
    }

    public override void Interact()
    {
        // Do nothing - taken care of by event service (should probably unify this in some way)
    }
    
    // Internally, outfits and their associated items are stored as 'MirageStoreSetItem'
    // We can use this to get the items required to create the outfit in the first place.
    // reason the collection isn't a MirageStoreSetItem is because that class is only a LookupTable,
    // and it's more convenient to store it internally like a glamourItem.
    public List<uint> GetAssociatedItemIds()
    {
        List<uint> associatedItems = [];
        var outfitSet = ExcelCache<MirageStoreSetItem>.GetSheet().GetRow(ExcelRow.RowId);
        if(outfitSet is not null)
        {
            var related = outfitSet.Value;
            // TODO: iterate over, not hardcode
            associatedItems = [
                related.MainHand.RowId,
                related.OffHand.RowId,
                related.Head.RowId,
                related.Body.RowId,
                related.Hands.RowId,
                related.Legs.RowId,
                related.Feet.RowId,
                related.Earrings.RowId,
                related.Necklace.RowId,
                related.Bracelets.RowId,
                related.Ring.RowId,
            ];
            associatedItems = associatedItems.Where(id => id != 0).ToList();
        }

        return associatedItems; 
    }
}
