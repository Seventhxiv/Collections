//namespace Collections;

//public abstract class CollectionProvider
//{
//    public CollectionProvider()
//    {
//    }

//    public abstract string CollectionName { get; }
//    public abstract List<ICollectible> GetCollection();
//}

//public class GlamourCollectionProvider : CollectionProvider
//{
//    public override string CollectionName => "Glamour";

//    public GlamourCollectionProvider()
//    {
//    }

    
//    public override List<ICollectible> GetCollection()
//    {
//        return ExcelCache<ItemAdapter>.GetSheet().AsParallel()
//            .Where(entry => entry.LevelEquip >= 1)
//            .Where(entry => Services.DataProvider.SupportedEquipSlots.Contains(entry.EquipSlot))
//            .Where(entry => !entry.Name.ToString().StartsWith("Dated ")) // Filter Dated items TODO probably doesnt work on other languages
//            .Select(entry => (ICollectible)CollectibleCache<GlamourCollectible, ItemAdapter>.Instance.GetObject(entry))
//            .OrderByDescending(c => c.IsFavorite())
//            .ThenByDescending(c => ((ItemCollectibleKey)c.CollectibleKey).excelRow.LevelEquip)
//            .ThenByDescending(c => c.Name)
//            .ToList();
//    }
//}


//public class Glamour2CollectionProvider : CollectionProvider
//{
//    public override string CollectionName => "Glamour2";

//    public Glamour2CollectionProvider()
//    {
//    }


//    public override List<ICollectible> GetCollection()
//    {
//        return ExcelCache<ItemAdapter>.GetSheet().AsParallel()
//            .Where(entry => entry.LevelEquip >= 1)
//            .Where(entry => Services.DataProvider.SupportedEquipSlots.Contains(entry.EquipSlot))
//            .Where(entry => !entry.Name.ToString().StartsWith("Dated ")) // Filter Dated items TODO probably doesnt work on other languages
//            .Select(entry => (ICollectible)CollectibleCache<GlamourCollectible, ItemAdapter>.Instance.GetObject(entry))
//            .OrderByDescending(c => c.IsFavorite())
//            .ThenByDescending(c => ((ItemCollectibleKey)c.CollectibleKey).excelRow.LevelEquip)
//            .ThenByDescending(c => c.Name)
//            .ToList();
//    }
//}

