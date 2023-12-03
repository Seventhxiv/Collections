using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

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
        var cabinetItem = ExcelCache<Lumina.Excel.GeneratedSheets.Cabinet>.GetSheet().Where(entry => entry.Item.Row == itemId).FirstOrDefault();
        return cabinetItem is not null ? cabinetItem.RowId : null;
    }

    public uint? ItemIdFromCabinetId(uint cabinetId)
    {
        var cabinetItem = ExcelCache<Lumina.Excel.GeneratedSheets.Cabinet>.GetSheet().GetRow(cabinetId);
        return cabinetItem is not null ? cabinetItem.Item.Row : null;
    }

    public bool IsItemInDresser(uint itemId)
    {
        var pureItemId = GetPureItemId(itemId);
        return Services.DresserObserver.DresserItemIds.Contains(pureItemId);
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
