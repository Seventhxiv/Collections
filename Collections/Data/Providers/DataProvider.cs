namespace Collections;

public class DataProvider
{
    public Dictionary<EquipSlot, List<ICollectible>> GlamourCollection = new();
    public Dictionary<EquipSlot, IDalamudTextureWrap> defaultEquipSlotIcons = new();
    public List<ClassJobAdapter> classJobs = new(); // TODO Move this out of here
    public ExcelSheet<ClassJobCategoryAdapter> classJobCategorySheet;
    public List<StainAdapter> stains = new();

    private ExcelSheet<ItemAdapter> itemSheet { get; set; }
    public DataProvider()
    {
        Dev.Start();
        itemSheet = Excel.GetExcelSheet<ItemAdapter>()!;
        PopulateData();
        BuildGlamourCollection();
        BuildMountCollection();
        BuildMinionCollection();
        Dev.Stop();
    }

    private Dictionary<Type, List<ICollectible>> collections = new();
    public List<ICollectible> GetCollection<T>()
    {
        return collections[typeof(T)];
    }


    private void PopulateData()
    {
        // Cache Equip slot icons
        var equipSlotList = GetEnumValues<EquipSlot>();
        defaultEquipSlotIcons = equipSlotList.AsParallel().ToDictionary(entry => entry, entry =>
        {
            var iconHandler = new IconHandler(GetEquipSlotIcon(entry));
            return iconHandler.GetIcon();
        });

        // Class jobs
        var classJobSheet = Excel.GetExcelSheet<ClassJobAdapter>()!;
        // Filter by interesting jobs based on ClassJobEntity.ClassJobConfig
        classJobs = classJobSheet.AsParallel().Where(entry => ClassJobAdapter.ClassJobConfig.ContainsKey(entry.Abbreviation)).ToList();

        // Stains
        stains = Excel.GetExcelSheet<StainAdapter>().Where(s => s.Color != 0).ToList();
    }

    private void BuildGlamourCollection()
    {
        GlamourCollection = itemSheet.AsParallel()
            .Where(entry => entry.LevelEquip >= 1)
            .Where(entry => entry.IsEquipment) // Filter Equipment
            .Where(entry => !entry.Name.ToString().StartsWith("Dated ")) // Filter Dated items TODO probably doesnt work on other languages
            .Select(entry => (ICollectible)new GlamourCollectible(entry))
            .GroupBy(entry => entry.CollectibleKey.item.EquipSlot)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(e => e.SortKey()).ToList());
    }

    private void BuildMountCollection()
    {
        var mountSheet = Excel.GetExcelSheet<Mount>()!;
        collections[typeof(MountCollectible)] = mountSheet.AsParallel()
            .Where(entry => entry.Singular != null && entry.Singular != "")
            .Select(entry => (ICollectible)new MountCollectible(entry))
            .ToList();
    }

    private void BuildMinionCollection()
    {
        var MinionSheet = Excel.GetExcelSheet<Companion>()!;
        collections[typeof(MinionCollectible)] = MinionSheet.AsParallel()
            .Where(entry => entry.Singular != null && entry.Singular != "")
            .Select(entry => (ICollectible)new MinionCollectible(entry))
            .ToList();
    }

    private int GetEquipSlotIcon(EquipSlot equipSlot)
    {
        switch (equipSlot)
        {
            case EquipSlot.Head: return 60124;
            case EquipSlot.Body: return 60125;
            case EquipSlot.Gloves: return 60126;
            case EquipSlot.Legs: return 60129;
            case EquipSlot.Feet: return 60130;
            case EquipSlot.MainHand: return 60102;
            case EquipSlot.OffHand: return 60110;
        }
        return 60135;
    }

}
