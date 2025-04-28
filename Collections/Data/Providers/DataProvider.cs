using System.Collections.Concurrent;

namespace Collections;

public class DataProvider
{
    public List<ClassJobAdapter> SupportedClassJobs = new();
    public List<StainAdapter> SupportedStains { get; set; }
    public readonly List<EquipSlot> SupportedEquipSlots = new()
    {
        EquipSlot.MainHand,
        EquipSlot.OffHand,
        EquipSlot.Head,
        EquipSlot.Body,
        EquipSlot.Gloves,
        EquipSlot.Legs,
        EquipSlot.Feet,
    };

    public ConcurrentDictionary<Type, (string name, uint orderKey, List<ICollectible> collection)> collections = new();

    public DataProvider()
    {
        Dev.Start();
        PopulateData();
        // Task.Run(() =>
        //     {
        //         Services.XivCollectClient.FetchXivCollectData().Wait();
        //     });
        Dev.Stop();
    }

    public List<ICollectible> GetCollection<T>()
    {
        return collections[typeof(T)].collection;
    }

    public List<ICollectible> GetCollection(Type T)
    {
        return collections[T].collection;
    }

    public Dictionary<string, List<ICollectible>> GetCollections()
    {
        return collections.ToDictionary(kv => kv.Value.name, kv => kv.Value.collection);
    }

    private void PopulateData()
    {
        // Class jobs
        SupportedClassJobs = ExcelCache<ClassJobAdapter>.GetSheet().AsParallel().Where(entry => ClassJobAdapter.ClassJobConfig.ContainsKey(entry.RowId)).ToList();

        // Stains
        SupportedStains = ExcelCache<StainAdapter>.GetSheet().Where(s => s.Color != 0).ToList();

        // Collections
        InitializeGlamourCollection();
        InitializeMountCollection();
        InitializeMinionCollection();
        InitializeEmoteCollection();
        InitializeHairstyleCollection();
        InitializeTripleTriadCollection();
        InitializeBlueMageCollection();
        InitializeBardingCollection();
        InitializeOrchestrionRollCollection();
        InitializeOutfitsCollection();
        InitializeFramerKitCollection();
        InitializeFashionAccessoriesCollection();
        InitializeGlassesCollection();
    }

    private void InitializeGlamourCollection()
    {
        collections[typeof(GlamourCollectible)] = (
            GlamourCollectible.CollectionName,
            0,
            ExcelCache<ItemAdapter>.GetSheet().AsParallel()
            .Where(entry => entry.LevelEquip >= 1)
            .Where(entry => SupportedEquipSlots.Contains(entry.EquipSlot))
            .Where(entry => !entry.Name.ToString().StartsWith("Dated ")) // TODO filter only works in English
            .Select(entry => (ICollectible)CollectibleCache<GlamourCollectible, ItemAdapter>.Instance.GetObject(entry))
            .OrderByDescending(c => c.IsFavorite())
            .ThenByDescending(c => ((ItemKey)c.CollectibleKey).Input.Item1.LevelEquip)
            .ThenByDescending(c => c.PatchAdded)
            .ToList()
            );
    }

    private void InitializeMountCollection()
    {
        collections[typeof(MountCollectible)] = (
            MountCollectible.CollectionName,
            1,
            ExcelCache<Mount>.GetSheet().AsParallel()
            .Where(entry => entry.Singular != "" && entry.Order != -1)
            .Select(entry => (ICollectible)CollectibleCache<MountCollectible, Mount>.Instance.GetObject(entry))
            .OrderByDescending(c => c.IsFavorite())
            .ThenByDescending(c => c.PatchAdded)
            .ToList()
            );
    }

    
    private void InitializeMinionCollection()
    {
        collections[typeof(MinionCollectible)] = (
            MinionCollectible.CollectionName,
            2,
            ExcelCache<Companion>.GetSheet().AsParallel()
            .Where(entry => entry.Singular != "" && !DataOverrides.IgnoreMinionId.Contains(entry.RowId))
            .Select(entry => (ICollectible)CollectibleCache<MinionCollectible, Companion>.Instance.GetObject(entry))
            .OrderByDescending(c => c.IsFavorite())
            .ThenByDescending(c => c.PatchAdded)
            .ToList()
            );
    }

    private void InitializeEmoteCollection()
    {
        collections[typeof(EmoteCollectible)] = (
            EmoteCollectible.CollectionName,
            3,
            ExcelCache<Emote>.GetSheet().AsParallel()
            .Where(entry => entry.Name != "" && entry.Icon != 0 && !DataOverrides.IgnoreEmoteId.Contains(entry.RowId) && entry.UnlockLink != 0)
            .Select(entry => (ICollectible)CollectibleCache<EmoteCollectible, Emote>.Instance.GetObject(entry))
            .OrderByDescending(c => c.IsFavorite())
            .ThenByDescending(c => c.PatchAdded)
            .ToList()
            );
    }

    private void InitializeHairstyleCollection()
    {
        collections[typeof(HairstyleCollectible)] = (
            HairstyleCollectible.CollectionName,
            4,
            ExcelCache<CharaMakeCustomize>.GetSheet().AsParallel()
            .Where(entry => entry.IsPurchasable && (entry.RowId < 100 || (entry.RowId >= 2050 && entry.RowId < 2100)))
            .Select(entry => (ICollectible)CollectibleCache<HairstyleCollectible, CharaMakeCustomize>.Instance.GetObject(entry))
            .OrderByDescending(c => c.IsFavorite())
            .ThenByDescending(c => c.PatchAdded)
            .ToList()
            );
    }

    private void InitializeTripleTriadCollection()
    {
        collections[typeof(TripleTriadCollectible)] = (
            TripleTriadCollectible.CollectionName,
            5,
            ExcelCache<TripleTriadCard>.GetSheet().AsParallel()
            .Where(entry => entry.Name != "" && entry.Name != "0")
            .Select(entry => (ICollectible)CollectibleCache<TripleTriadCollectible, TripleTriadCard>.Instance.GetObject(entry))
            .OrderByDescending(c => c.IsFavorite())
            .ThenByDescending(c => c.PatchAdded)
            .ToList()
            );
    }

    private void InitializeBardingCollection()
    {
        collections[typeof(BardingCollectible)] = (
            BardingCollectible.CollectionName,
            7,
            ExcelCache<BuddyEquip>.GetSheet().AsParallel()
            .Where(entry => entry.Name != "" && !DataOverrides.IgnoreBardingId.Contains(entry.RowId))
            .Select(entry => (ICollectible)CollectibleCache<BardingCollectible, BuddyEquip>.Instance.GetObject(entry))
            .OrderByDescending(c => c.IsFavorite())
            .ThenByDescending(c => c.PatchAdded)
            .ToList()
            );
    }

    private void InitializeBlueMageCollection()
    {
        collections[typeof(BlueMageCollectible)] = (
            BlueMageCollectible.CollectionName,
            6,
            ExcelCache<Lumina.Excel.Sheets.Action>.GetSheet().AsParallel()
            .Where(entry => entry.ClassJob.RowId == 36 && entry.Name != "")
            .Select(entry => (ICollectible)CollectibleCache<BlueMageCollectible, Lumina.Excel.Sheets.Action>.Instance.GetObject(entry))
            .OrderByDescending(c => c.IsFavorite())
            .ThenByDescending(c => c.PatchAdded)
            .ToList()
            );
    }

    private void InitializeOrchestrionRollCollection()
    {
        collections[typeof(OrchestrionCollectible)] = (
            OrchestrionCollectible.CollectionName,
            8,
            ExcelCache<Orchestrion>.GetSheet().AsParallel()
            .Where(entry => entry.Name != "" && entry.Name != "0")
            .Select(entry => (ICollectible)CollectibleCache<OrchestrionCollectible, Orchestrion>.Instance.GetObject(entry))
            .OrderByDescending(c => c.IsFavorite())
            .ThenByDescending(c => c.Name)
            .ToList()
            );
    }

    private void InitializeOutfitsCollection()
    {
        collections[typeof(OutfitsCollectible)] = (
            OutfitsCollectible.CollectionName,
            9,
            ExcelCache<ItemAdapter>.GetSheet().AsParallel()
            .Where(entry => entry.LevelEquip >= 1)
            .Where(entry => entry.ItemUICategory.Value.Name == "Outfits")
            .Select(entry => (ICollectible)CollectibleCache<OutfitsCollectible, ItemAdapter>.Instance.GetObject(entry))
            .OrderByDescending(c => c.IsFavorite())
            .ThenByDescending(c => c.PatchAdded)
            .ToList()
            );
    }

    private void InitializeFramerKitCollection()
    {
        collections[typeof(FramerKitCollectible)] = (
            FramerKitCollectible.CollectionName,
            10,
            ExcelCache<ItemAdapter>.GetSheet().AsParallel()
            .Where(entry => entry.ItemAction.Value.Type == 29459)
            .Select(entry => (ICollectible)CollectibleCache<FramerKitCollectible, ItemAdapter>.Instance.GetObject(entry))
            .OrderByDescending(c => c.IsFavorite())
            .ThenByDescending(c => c.Name)
            .ToList()
            );
    }

    private void InitializeFashionAccessoriesCollection()
    {
        collections[typeof(FashionAccessoriesCollectible)] = (
            FashionAccessoriesCollectible.CollectionName,
            11,
            ExcelCache<Ornament>.GetSheet().AsParallel()
            .Where(entry => entry.Icon != 0 && !DataOverrides.IgnoreFashionAccessoryId.Contains(entry.RowId))
            .Select(entry => (ICollectible)CollectibleCache<FashionAccessoriesCollectible, Ornament>.Instance.GetObject(entry))
            .OrderByDescending(c => c.IsFavorite())
            .ThenByDescending(c => c.Name)
            .ToList()
            );
    }
    private void InitializeGlassesCollection()
    {
        collections[typeof(GlassesCollectible)] = (
            GlassesCollectible.CollectionName,
            12,
            ExcelCache<Glasses>.GetSheet().AsParallel()
            .Where(entry => entry.Icon != 0 && entry.Name == entry.Style.Value.Name)
            .Select(entry => (ICollectible)CollectibleCache<GlassesCollectible, Glasses>.Instance.GetObject(entry))
            .OrderByDescending(c => c.IsFavorite())
            .ThenByDescending(c => c.Name)
            .ToList()
            );
    }
}
