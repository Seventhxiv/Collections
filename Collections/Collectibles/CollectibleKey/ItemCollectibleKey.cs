namespace Collections;

public class ItemCollectibleKey : CollectibleKey<ItemAdapter>, ICreateable<ItemCollectibleKey, (ItemAdapter, bool)>
{
    public override string Name { get; init; }
    private IconHandler iconHandler { get; init; }

    public ItemCollectibleKey((ItemAdapter, bool) input) : base(input)
    {
        Name = excelRow.Name;
        iconHandler = new IconHandler(excelRow.Icon);

        // For currencies dont bother looking at another level of shops
        if (input.Item2)
        {
            if (Services.DataGenerator.ShopsDataGenerator.itemToShopEntries.TryGetValue(excelRow.RowId, out var shopEntries))
            {
                CollectibleSources.AddRange(shopEntries.Select(shopEntry => new ShopCollectibleSource(shopEntry)));
            }
        }
        if (Services.DataGenerator.InstancesDataGenerator.itemToContentFinderCondition.TryGetValue(excelRow.RowId, out var duty))
        {
            CollectibleSources.AddRange(duty.Select(instance => new InstanceCollectibleSource(instance)));
        }
        if (Services.DataGenerator.EventDataGenerator.itemsToEvents.TryGetValue(excelRow.RowId, out var events))
        {
            CollectibleSources.AddRange(events.Select(eventName => new EventCollectibleSource(eventName)));
        }
        if (Services.DataGenerator.MogStationDataGenerator.items.Contains(excelRow.RowId))
        {
            CollectibleSources.Add(new MogStationCollectibleSource());
        }
        if (Services.DataGenerator.ContainersDataGenerator.itemsToContainers.TryGetValue(excelRow.RowId, out var containers))
        {
            CollectibleSources.AddRange(containers.Select(itemId => new ContainerCollectibleSource(itemId)));
        }
        if (Services.DataGenerator.AchievementsDataGenerator.itemToAchievement.TryGetValue(excelRow.RowId, out var achievement))
        {
            CollectibleSources.Add(new AchievementCollectibleSource(achievement));
        }
        if (Services.DataGenerator.QuestsDataGenerator.itemToQuests.TryGetValue(excelRow.RowId, out var quests))
        {
            CollectibleSources.AddRange(quests.Select(entry => new QuestCollectibleSource(entry)));
        }
        if (Services.DataGenerator.CraftingDataGenerator.itemsToRecipes.TryGetValue(excelRow.RowId, out var recipes))
        {
            CollectibleSources.AddRange(recipes.Select(entry => new CraftingCollectibleSource(entry)));
        }
        if (Services.DataGenerator.CollectibleKeyDataGenerator.cardItemIdToNpc.TryGetValue(excelRow.RowId, out var Npc))
        {
            CollectibleSources.Add(new NpcCollectibleSource(Npc));
        }
    }

    public static ItemCollectibleKey Create((ItemAdapter, bool) input)
    {
        return new(input);
    }

    public IDalamudTextureWrap GetIconLazy()
    {
        return iconHandler.GetIconLazy();
    }

    public override bool GetIsTradeable()
    {
        return !excelRow.IsUntradable;
    }

    private List<CollectibleSourceCategory> sourceTypes;
    public override List<CollectibleSourceCategory> GetSourceCategories()
    {
        if (sourceTypes != null)
        {
            return sourceTypes;
        }

        // Inherit categories from sources
        sourceTypes = CollectibleSources.SelectMany(source => source.GetSourceCategories()).ToList();

        // Add category if item is a currency
        if (Services.DataGenerator.CurrencyDataGenerator.ItemIdToSourceCategory.TryGetValue(excelRow.RowId, out var category))
        {
            sourceTypes.Add(category);
        }

        return sourceTypes;
    }

    private int? marketBoardPrice = null;
    private bool marketBoardPriceScheduled = false;
    public override int? GetMarketBoardPriceLazy()
    {
        if (marketBoardPrice != null)
        {
            return marketBoardPrice;
        }

        if (!marketBoardPriceScheduled)
        {
            marketBoardPriceScheduled = true;
            Task.Run(async () =>
            {
                await Services.UniversalisClient.populateMarketBoardData(excelRow.RowId);
                var price = Services.UniversalisClient.itemToMarketplaceData[excelRow.RowId].minPriceWorld;
                if (price != null)
                {
                    marketBoardPrice = (int)price;
                }
            });
        }

        return null;
    }
}

