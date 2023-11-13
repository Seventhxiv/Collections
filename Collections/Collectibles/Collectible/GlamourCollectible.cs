using FFXIVClientStructs.FFXIV.Client.Game;

namespace Collections;

public class GlamourCollectible : Collectible<ItemAdapter>
{
    protected override ItemAdapter excelRow { get; set; }
    public override string Name { get; init; }
    public GlamourCollectible(ItemAdapter excelRow) : base(excelRow)
    {
        CollectibleKey = new CollectibleKey(excelRow);
        this.excelRow = excelRow;
        Name = excelRow.Name;
    }

    public override string GetName()
    {
        return excelRow.Name;
    }

    public override void UpdateObtainedState()
    {
        isObtained = GetIsItemObtained(excelRow.RowId);
    }
    protected override int GetIconId()
    {
        return excelRow.Icon;
    }

    public override int SortKey()
    {
        return Convert.ToInt32(string.Format("{0}{1}", excelRow.LevelEquip, excelRow.LevelItem.Value.RowId));
    }

    private unsafe bool GetIsItemObtained(uint itemId)
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

        // Look up glamour dresser
        var inGlamDresser = Services.Configuration.DresserContentIds.Any(id => id == itemId);

        return false;
    }

    //public new static List<ICollectible> GetCollection()
    //{
    //    return Services.ItemManager.items
    //}

    public new static string GetCollectionName()
    {
        return "Glamour";
    }
}
