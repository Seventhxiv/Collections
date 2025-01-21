namespace Collections;

public class ItemKey : CollectibleKey<(ItemAdapter, bool)>, ICreateable<ItemKey, (ItemAdapter, bool)>
{
    private IconHandler iconHandler { get; init; }

    public ItemKey((ItemAdapter, bool) input) : base(input)
    {
        iconHandler = new IconHandler(input.Item1.Icon);
    }

    public static ItemKey Create((ItemAdapter, bool) input)
    {
        return new(input);
    }

    protected override string GetName((ItemAdapter, bool) input)
    {
        return input.Item1.Name.ToString();
    }

    protected override uint GetId((ItemAdapter, bool) input)
    {
        return input.Item1.RowId;
    }

    protected override List<ICollectibleSource> GetCollectibleSources((ItemAdapter, bool) input)
    {
        var excelRow = input.Item1;
        var collectibleSources = new List<ICollectibleSource>();
        var dataGenerator = Services.DataGenerator.SourcesDataGenerator;

        // For currencies dont bother looking at another level of shops
        if (input.Item2)
        {
            if (dataGenerator.ShopsDataGenerator.data.TryGetValue(excelRow.RowId, out var shopEntries))
            {
                collectibleSources.AddRange(shopEntries.Select(shopEntry => new ShopSource(shopEntry)));
            }
        }

        try
        {
            if (dataGenerator.InstancesDataGenerator.data.TryGetValue(excelRow.RowId, out var duty))
            {
                collectibleSources.AddRange(duty.Select(instance => new InstanceSource(instance)));
            }

            if (dataGenerator.EventDataGenerator.data.TryGetValue(excelRow.RowId, out var events))
            {
                collectibleSources.AddRange(events.Select(eventName => new EventSource(eventName)));
            }

            if (dataGenerator.MogStationDataGenerator.data.ContainsKey(excelRow.RowId))
            {
                collectibleSources.Add(new MogStationSource());
            }

            // this causes crash :/
            // if (dataGenerator.ContainersDataGenerator.data.TryGetValue(excelRow.RowId, out var containers))
            // {
            //     collectibleSources.AddRange(containers.Select(itemId => new ContainerSource(itemId)));
            // }

            if (dataGenerator.AchievementsDataGenerator.data.TryGetValue(excelRow.RowId, out var achievements))
            {
                collectibleSources.AddRange(achievements.Select(entry => new AchievementSource(entry)));
            }

            if (dataGenerator.QuestsDataGenerator.data.TryGetValue(excelRow.RowId, out var quests))
            {
                collectibleSources.AddRange(quests.Select(entry => new QuestSource(entry)));
            }

            if (dataGenerator.CraftingDataGenerator.data.TryGetValue(excelRow.RowId, out var recipes))
            {
                collectibleSources.AddRange(recipes.Select(entry => new CraftingSource(entry)));
            }

            if (Services.DataGenerator.KeysDataGenerator.ItemIdToTripleTriadId.TryGetValue(excelRow.RowId, out var tripleTriadId))
            {
                if (dataGenerator.TripleTriadNpcDataGenerator.data.TryGetValue(tripleTriadId, out var npcs))
                {
                    collectibleSources.AddRange(npcs.Select(entry => new NpcSource(entry)));
                }
            }
        }
        catch (Exception e)
        {
            Dev.Log(e.ToString());
        }

        return collectibleSources;
    }

    protected override HashSet<SourceCategory> GetBaseSourceCategories()
    {
        var sourceCategories = new HashSet<SourceCategory>();

        // Add category if item is a currency
        if (Services.DataGenerator.CurrencyDataGenerator.ItemIdToSourceCategory.TryGetValue(Input.Item1.RowId, out var category))
        {
            sourceCategories.Add(category);
        }

        return sourceCategories;
    }

    public ISharedImmediateTexture GetIconLazy()
    {
        return iconHandler.GetIconLazy();
    }

    public override Tradeability GetIsTradeable()
    {
        return !Input.Item1.IsUntradable ? Tradeability.Tradeable : Tradeability.UntradeableSingle;
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
                await Services.UniversalisClient.populateMarketBoardData(Input.Item1.RowId);
                var price = Services.UniversalisClient.itemToMarketplaceData[Input.Item1.RowId].minPriceWorld;
                if (price != null)
                {
                    marketBoardPrice = (int)price;
                }
            });
        }
        return null;
    }
}

