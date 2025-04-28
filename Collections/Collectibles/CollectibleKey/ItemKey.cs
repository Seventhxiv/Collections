using System.Linq.Expressions;

namespace Collections;

public class ItemKey : CollectibleKey<(ItemAdapter, int)>, ICreateable<ItemKey, (ItemAdapter, int)>
{
    private IconHandler iconHandler { get; init; }

    public ItemKey((ItemAdapter, int) input) : base(input)
    {
        iconHandler = new IconHandler(input.Item1.Icon);
    }

    public static ItemKey Create((ItemAdapter, int) input)
    {
        return new(input);
    }

    protected override string GetName((ItemAdapter, int) input)
    {
        return input.Item1.Name.ToString();
    }

    protected override uint GetId((ItemAdapter, int) input)
    {
        return input.Item1.RowId;
    }

    protected override List<ICollectibleSource> GetCollectibleSources((ItemAdapter, int) input)
    {
        var excelRow = input.Item1;
        var collectibleSources = new List<ICollectibleSource>();
        var dataGenerator = Services.DataGenerator.SourcesDataGenerator;

        // Stop recursion depth at 10 at most
        if (input.Item2 >= 10)
        {
            return collectibleSources;
        }

        // For currencies dont bother looking at another level of shops
        if (input.Item2 == 0)
        {
            if (dataGenerator.ShopsDataGenerator.data.TryGetValue(excelRow.RowId, out var shopEntries))
            {
                collectibleSources.AddRange(shopEntries.Select(shopEntry => new ShopSource(shopEntry, input.Item2 + 1)));
            }
        }

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

        if (dataGenerator.ContainersDataGenerator.data.TryGetValue(excelRow.RowId, out var containers))
        {
            collectibleSources.AddRange(containers.Select(itemId => new ContainerSource(itemId, input.Item2 + 1)));
        }

        if (dataGenerator.AchievementsDataGenerator.data.TryGetValue(excelRow.RowId, out var achievements))
        {
            collectibleSources.AddRange(achievements.Select(entry => new AchievementSource(entry)));
        }
        
        if (dataGenerator.PvPDataGenerator.data.TryGetValue(excelRow.RowId, out var pvpSeries))
        {
            collectibleSources.AddRange(pvpSeries.Select(entry => new PvPSeriesSource(entry.Item1, entry.Item2)));
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

    private World? homeWorld = null;
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
            var world = Services.ClientState.LocalPlayer?.CurrentWorld.Value;
            if (world != null)
            {
                homeWorld = (World)world;
            }
            Task.Run(async () =>
            {
                await Services.UniversalisClient.populateMarketBoardData(Input.Item1.RowId, homeWorld);
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

