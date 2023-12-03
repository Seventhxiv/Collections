namespace Collections;

// Represents an item that is used to unlock a collectible
public interface ICollectibleKey
{
    public List<CollectibleSource> CollectibleSources { get; set; }
    public ItemAdapter item { get; init; }
}

public class CollectibleKey : ICollectibleKey, ICreateable<CollectibleKey, (ItemAdapter, bool)>
{
    public List<CollectibleSource> CollectibleSources { get; set; } = new();
    public ItemAdapter item { get; init; }
    protected IconHandler IconHandler { get; init; }

    public CollectibleKey((ItemAdapter, bool) input)
    {
        this.item = input.Item1;
        IconHandler = new IconHandler(item.Icon);

        // For currencies dont bother looking at another level of shops
        if (input.Item2)
        {
            if (Services.DataGenerator.ShopsDataGenerator.itemToShopEntry.ContainsKey(item.RowId))
            {
                CollectibleSources.AddRange(Services.DataGenerator.ShopsDataGenerator.itemToShopEntry[item.RowId].Select(shopEntry => new ShopCollectibleSource(shopEntry)));
            }
        }
        if (Services.DataGenerator.InstancesDataGenerator.itemToContentFinderCondition.ContainsKey(item.RowId))
        {
            CollectibleSources.AddRange(Services.DataGenerator.InstancesDataGenerator.itemToContentFinderCondition[item.RowId].Select(instance => new InstanceCollectibleSource(instance)));
        }
        if (Services.DataGenerator.EventDataGenerator.itemsToEvents.ContainsKey(item.RowId))
        {
            CollectibleSources.AddRange(Services.DataGenerator.EventDataGenerator.itemsToEvents[item.RowId].Select(eventName => new EventCollectibleSource(eventName)));
        }
        if (Services.DataGenerator.MogStationDataGenerator.items.Contains(item.RowId))
        {
            CollectibleSources.Add(new MogStationCollectibleSource());
        }
        if (Services.DataGenerator.ContainersDataGenerator.itemsToContainers.ContainsKey(item.RowId))
        {
            CollectibleSources.AddRange(Services.DataGenerator.ContainersDataGenerator.itemsToContainers[item.RowId].Select(itemId => new ContainerCollectibleSource(itemId)));
        }
        if (Services.DataGenerator.AchievementsDataGenerator.itemToAchievement.ContainsKey(item.RowId))
        {
            CollectibleSources.Add(new AchievementCollectibleSource(Services.DataGenerator.AchievementsDataGenerator.itemToAchievement[item.RowId]));
        }
        if (Services.DataGenerator.QuestsDataGenerator.itemToQuest.ContainsKey(item.RowId))
        {
            CollectibleSources.AddRange(Services.DataGenerator.QuestsDataGenerator.itemToQuest[item.RowId].Select(entry => new QuestCollectibleSource(entry)));
        }
        if (Services.DataGenerator.CraftingDataGenerator.itemsToRecipes.ContainsKey(item.RowId))
        {
            CollectibleSources.AddRange(Services.DataGenerator.CraftingDataGenerator.itemsToRecipes[item.RowId].Select(entry => new CraftingCollectibleSource(entry)));
        }
    }

    public static CollectibleKey Create((ItemAdapter, bool) input)
    {
        return new(input);
    }

    private List<CollectibleSourceCategory> sourceTypes;
    public List<CollectibleSourceCategory> GetSourceTypes()
    {
        if (sourceTypes != null)
        {
            return sourceTypes;
        }

        sourceTypes = CollectibleSources.SelectMany(source => source.GetSourceCategories()).ToList();

        // Add any custom mappings from ContentTypeResolver
        sourceTypes.Add(Services.ContentTypeResolver.CurrencyItemCollectionMethodType(item));

        // Adding beast tribe currencies manually
        if (Services.DataGenerator.BeastTribesDataGenerator.itemToBeastTribe.ContainsKey(item.RowId))
        {
            sourceTypes.Add(CollectibleSourceCategory.BeastTribes);
        }

        return sourceTypes;
    }

    private int? marketBoardPrice = null;
    private bool marketBoardPriceScheduled = false;
    public int? GetMarketBoardPriceLazy()
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
                await Services.UniversalisClient.populateMarketBoardData(item.RowId);
                var price = Services.UniversalisClient.itemToMarketplaceData[item.RowId].minPriceWorld;
                if (price != null)
                {
                    marketBoardPrice = (int)price;
                }
            });
        }

        return null;
    }

    public bool GetIsTradeable()
    {
        return !item.IsUntradable;
    }

    // TODO move this out of here
    public void OpenGamerEscape()
    {
        WikiOpener.OpenGamerEscape(item.Name);
    }

    public IDalamudTextureWrap GetIconLazy()
    {
        return IconHandler.GetIconLazy();
    }
}

