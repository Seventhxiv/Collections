namespace Collections;

public class DataDebugExporter
{
    public static void LogDataReport()
    {
        var collections = Services.DataProvider.collections;
        foreach (var (type, (name, orderKey, collection)) in collections)
        {
            PrintCollectionReport(collection, name);
        }
    }

    private static void PrintCollectionReport(List<ICollectible> collection, string name)
    {
        var size = collection.Count;
        var keysLocated = collection.Where(c => c.CollectibleKey is not null).Count();
        var sourcesLocated = collection.Where(c => c.CollectibleKey is not null && c.CollectibleKey.CollectibleSources.Any()).Count();
        var sourcesCount = collection.Where(c => c.CollectibleKey is not null).Select(c => c.CollectibleKey.CollectibleSources.Count()).Sum();
        Dev.Log($"{name}: Keys Missing: {size - keysLocated}/{size}, Sources Missing: {size - sourcesLocated}/{size}, Avg Sources: {(float)sourcesCount / size}");
    }

    public static void ExportCollectionsData(List<Type> collectionFilters = null)
    {
        var collections = Services.DataProvider.collections;
        var filteredCollections = collectionFilters is not null ? collections.Where(k => collectionFilters.Contains(k.Key)) : collections;

        var data = new List<CollectionData2>();
        foreach (var (type, (name, orderKey, collection)) in filteredCollections)
        {
            data.Add(new CollectionData2()
            {
                CollectionName = name,
                CollectiblesData = GetCollectiblesData(collection),
            });
        }

        var flattenedData = new List<CollectibleFlattenedData>();
        foreach (var collection in data)
        {
            foreach (var entry in collection.CollectiblesData)
            {
                if (entry.CollectibleKeyData is null)
                {
                    flattenedData.Add(new CollectibleFlattenedData()
                    {
                        CollectionName = collection.CollectionName,
                        Id = entry.Id,
                        Name = entry.Name,
                    });
                }
                else if (entry.CollectibleSourcesData is null || !entry.CollectibleSourcesData.Any())
                {
                    flattenedData.Add(new CollectibleFlattenedData()
                    {
                        CollectionName = collection.CollectionName,
                        Id = entry.Id,
                        Name = entry.Name,
                        KeyId = entry.CollectibleKeyData.Id,
                        KeyName = entry.CollectibleKeyData.Name,
                        KeyType = entry.CollectibleKeyData.Type,
                    });
                }
                else
                {
                    foreach (var source in entry.CollectibleSourcesData)
                    {
                        flattenedData.Add(new CollectibleFlattenedData()
                        {
                            CollectionName = collection.CollectionName,
                            Id = entry.Id,
                            Name = entry.Name,
                            KeyId = entry.CollectibleKeyData.Id,
                            KeyName = entry.CollectibleKeyData.Name,
                            KeyType = entry.CollectibleKeyData.Type,
                            SourceName = source.Name,
                            SourceType = source.Type,
                            ShopId = source.ShopId,
                            NpcId = source.NpcId,
                            NpcName = source.NpcName,
                            NpcNotFound = source.NpcNotFound,
                            LocationNotFound = source.LocationNotFound,
                        });
                    }
                }
            }
        }
        CSVHandler.Write(flattenedData, "DataDebug.csv");
    }

    public static List<CollectibleData> GetCollectiblesData(List<ICollectible> collection)
    {
        var CollectiblesData = new List<CollectibleData>();
        foreach (var collectible in collection)
        {
            CollectiblesData.Add(GetCollectibleData(collectible));
        }
        return CollectiblesData;
    }

    public static CollectibleData GetCollectibleData(ICollectible collectible)
    {
        return new CollectibleData()
        {
            Id = collectible.Id,
            Name = collectible.Name,
            CollectibleKeyData = GetCollectibleKeyData(collectible.CollectibleKey),
            CollectibleSourcesData = GetCollectibleSourcesData(collectible.CollectibleKey),
        };
    }

    public static CollectibleKeyData GetCollectibleKeyData(ICollectibleKey collectibleKey)
    {
        if (collectibleKey is null)
            return null;

        return new CollectibleKeyData()
        {
            Id = collectibleKey.Id,
            Name = collectibleKey.Name,
            Type = collectibleKey.GetType().Name,
        };
    }

    public static List<CollectibleSourceData> GetCollectibleSourcesData(ICollectibleKey collectibleKey)
    {
        if (collectibleKey is null)
            return null;

        var sources = collectibleKey.CollectibleSources;
        var data = new List<CollectibleSourceData>();
        foreach (var source in sources)
        {
            var isShop = source is ShopSource;
            ShopSource shopSource = null;
            ENpcResident npc = null;
            if (isShop)
            {
                shopSource = (ShopSource)source;
                npc = shopSource.ENpcResident;
            }

            var Name = source.GetName();
            var Type = source.GetType().Name;
            uint? NpcId = npc is not null ? npc.RowId : null;
            var NpcName = npc is not null ? npc.Singular : "";
            uint? ShopId = isShop ? shopSource.ShopId : null;
            var LocationNotFound = isShop && !source.GetIslocatable();
            var NpcNotFound = isShop && npc is null;

            data.Add(new CollectibleSourceData()
            {
                Name = Name,
                Type = Type,
                NpcId = NpcId,
                NpcName = NpcName,
                ShopId = ShopId,
                LocationNotFound = LocationNotFound,
                NpcNotFound = NpcNotFound,
            });
        }
        return data;
    }

    public static void ShopsDataDebugger()
    {
        var data = new List<ShopData>();
        foreach (var collectible in Services.DataProvider.GetCollection<GlamourCollectible>())
        {
            var sources = collectible.CollectibleKey.CollectibleSources;
            var badObservedCost = new List<uint>();
            var goodObservedCost = new List<uint>();
            foreach (var source in sources)
            {
                if (source is not ShopSource)
                {
                    continue;
                }

                var shopSource = (ShopSource)source;

                var name = source.GetName();
                var locatable = shopSource.GetIslocatable();
                var npcResident = shopSource.ENpcResident;
                var NpcId = npcResident is not null ? npcResident.RowId : 0;
                var validNpc = npcResident is not null;
                var npcName = npcResident is not null ? npcResident.Singular.ToString() : "";
                var shopId = shopSource.ShopId;

                var costIndicator = shopSource.costItems.First().collectibleKey.Id;
                if (validNpc && locatable)
                    goodObservedCost.Add(costIndicator);
                else
                    badObservedCost.Add(costIndicator);
            }

            if (badObservedCost.Any() && !badObservedCost.Intersect(goodObservedCost).Any())
            {
                foreach (var source in sources)
                {
                    if (source is not ShopSource)
                    {
                        continue;
                    }

                    var shopSource = (ShopSource)source;

                    var name = source.GetName();
                    var locatable = shopSource.GetIslocatable();
                    var npcResident = shopSource.ENpcResident;
                    var NpcId = npcResident is not null ? npcResident.RowId : 0;
                    var validNpc = npcResident is not null;
                    var npcName = npcResident is not null ? npcResident.Singular.ToString() : "";
                    var shopId = shopSource.ShopId;
                    var dataObj = new ShopData()
                    {
                        itemId = collectible.CollectibleKey.Id,
                        itemName = collectible.CollectibleKey.Name,
                        npcId = NpcId,
                        npcName = npcName,
                        validNpc = validNpc,
                        locatableNpc = locatable,
                        shopId = shopId,
                        costDescription = name,
                    };
                    data.Add(dataObj);
                }
            }
        }

        CSVHandler.Write(data, "DataDebug.csv");
    }

    public class CollectibleFlattenedData
    {
        public string CollectionName { get; set; }
        public uint Id { get; set; }
        public string Name { get; set; }
        public uint KeyId { get; set; }
        public string KeyName { get; set; }
        public string KeyType { get; set; }
        public string SourceName { get; set; }
        public string SourceType { get; set; }
        public uint? ShopId { get; set; }
        public uint? NpcId { get; set; }
        public string NpcName { get; set; }
        public bool NpcNotFound { get; set; }
        public bool LocationNotFound { get; set; }
    }

    public class CollectionData2
    {
        public string CollectionName { get; set; }
        public List<CollectibleData> CollectiblesData { get; set; }
    }

    public class CollectibleData
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public CollectibleKeyData CollectibleKeyData { get; set; }
        public List<CollectibleSourceData> CollectibleSourcesData { get; set; }
    }

    public class CollectibleKeyData
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class CollectibleSourceData
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public uint? ShopId { get; set; }
        public uint? NpcId { get; set; }
        public string NpcName { get; set; }
        public bool NpcNotFound { get; set; }
        public bool LocationNotFound { get; set; }
    }

    public class ShopData
    {
        public uint npcId { get; set; }
        public string npcName { get; set; }
        public bool validNpc { get; set; }
        public bool locatableNpc { get; set; }
        public uint shopId { get; set; }
        public uint itemId { get; set; }
        public string itemName { get; set; }
        public string costDescription { get; set; }
    }
}

