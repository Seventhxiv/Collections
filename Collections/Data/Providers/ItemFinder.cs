using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Extensions;

namespace Collections;

public unsafe class ItemFinder
{
    private static FFXIVClientStructs.FFXIV.Client.Game.UI.Cabinet Cabinet = UIState.Instance()->Cabinet;

    public bool IsItemInArmoireCache(uint itemId)
    {
        return Services.DresserObserver.ArmoireItemIds.Contains(itemId);
    }

    public uint? CabinetIdFromItemId(uint itemId)
    {
        var cabinetItem = ExcelCache<Lumina.Excel.Sheets.Cabinet>.GetSheet().Where(entry => entry.Item.RowId == itemId).FirstOrNull();
        return cabinetItem is not null ? cabinetItem.Value.RowId : null;
    }

    public uint? ItemIdFromCabinetId(uint cabinetId)
    {
        var cabinetItem = ExcelCache<Lumina.Excel.Sheets.Cabinet>.GetSheet().GetRow(cabinetId);
        return cabinetItem is not null ? cabinetItem.Value.Item.RowId : null;
    }

    public bool IsItemInDresser(uint itemId, bool checkOutfits = false)
    {
        var pureItemId = GetPureItemId(itemId);
        return Services.DresserObserver.DresserItemIds.Contains(pureItemId) || (checkOutfits && OutfitsContainingItem(pureItemId).Any(Services.DresserObserver.DresserItemIds.Contains));
    }

    public List<uint> OutfitsContainingItem(uint itemId)
    {
        return ExcelCache<MirageStoreSetItem>.GetSheet().Where(outfit =>
            ((List<uint>)[
                outfit.MainHand.RowId,
                outfit.OffHand.RowId,
                outfit.Head.RowId,
                outfit.Body.RowId,
                outfit.Hands.RowId,
                outfit.Legs.RowId,
                outfit.Feet.RowId,
                outfit.Earrings.RowId,
                outfit.Necklace.RowId,
                outfit.Bracelets.RowId,
                outfit.Ring.RowId,
            ]).Contains(itemId)).Select((outfit) => outfit.RowId).ToList();
    }

    // Internally, outfits and their associated items are stored as 'MirageStoreSetItem'
    // We can use this to get the items required to create the outfit in the first place.
    // reason the collection isn't a MirageStoreSetItem is because that class is only a LookupTable,
    // and it's more convenient to store it internally like a GlamourCollectible.
    public List<uint> ItemIdsInOutfit(uint itemId)
    {
        List<uint> associatedItems = [];
        var outfitSet = ExcelCache<MirageStoreSetItem>.GetSheet().GetRow(itemId);
        if (outfitSet is not null)
        {
            var related = outfitSet.Value;
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

    public uint GetPureItemId(uint itemId)
    {
        return itemId > 1000000 ? itemId - 1000000 : itemId;
    }

    public bool IsItemInInventory(uint itemId)
    {
        var inventoryTypes = new List<InventoryType>()
        {
            InventoryType.Inventory1,
            InventoryType.Inventory2,
            InventoryType.Inventory3,
            InventoryType.Inventory4,
            InventoryType.EquippedItems,
            InventoryType.ArmoryOffHand,
            InventoryType.ArmoryHead,
            InventoryType.ArmoryBody,
            InventoryType.ArmoryHands,
            InventoryType.ArmoryWaist,
            InventoryType.ArmoryLegs,
            InventoryType.ArmoryFeets,
            InventoryType.ArmoryEar,
            InventoryType.ArmoryNeck,
            InventoryType.ArmoryWrist,
            InventoryType.ArmoryRings,
            InventoryType.ArmoryMainHand,
            InventoryType.SaddleBag1,
            InventoryType.SaddleBag2,
            InventoryType.PremiumSaddleBag1,
            InventoryType.PremiumSaddleBag2,
            InventoryType.RetainerPage1,
            InventoryType.RetainerPage2,
            InventoryType.RetainerPage3,
            InventoryType.RetainerPage4,
            InventoryType.RetainerPage5,
            InventoryType.RetainerPage6,
            InventoryType.RetainerPage7,
        };
        foreach (var inventoryType in inventoryTypes)
        {
            if (InventoryManager.Instance()->GetItemCountInContainer(itemId, inventoryType, true) > 0)
                return true;
            else if (InventoryManager.Instance()->GetItemCountInContainer(itemId, inventoryType, false) > 0)
                return true;
        }
        return false;
    }
}
