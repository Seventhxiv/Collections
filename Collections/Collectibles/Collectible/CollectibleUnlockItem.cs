using Dalamud.Interface.Internal;
using ImGuiScene;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Collections;

// Represents items that are used to unlock collectibles
// This layer is used to connect between a Collectible and CollectibleSources
public class CollectibleUnlockItem
{
    // Properties
    public List<CollectibleSource> CollectibleSources { get; set; } = new();
    public Item item { get; init; }
    protected IconHandler IconHandler { get; init; }

    // Constructor
    public CollectibleUnlockItem(Item item, bool includeShops = true)
    {
        this.item = item;
        IconHandler = new IconHandler(item.Icon);

        // For currencies dont bother looking at another level of shops
        if (includeShops)
        {
            if (Services.DataGenerator.ShopsDataParser.itemToShopEntry.ContainsKey(item.RowId))
            {
                CollectibleSources.AddRange(Services.DataGenerator.ShopsDataParser.itemToShopEntry[item.RowId].Select(shopEntry => new ShopCollectibleSource(shopEntry)));
            }
        }
        if (Services.DataGenerator.InstancesDataParser.itemToContentFinderCondition.ContainsKey(item.RowId))
        {
            CollectibleSources.AddRange(Services.DataGenerator.InstancesDataParser.itemToContentFinderCondition[item.RowId].Select(instance => new InstanceCollectibleSource(instance)));
        }
        if (Services.DataGenerator.EventDataParser.itemsToEvents.ContainsKey(item.RowId))
        {
            CollectibleSources.AddRange(Services.DataGenerator.EventDataParser.itemsToEvents[item.RowId].Select(eventName => new EventCollectibleSource(eventName)));
        }
        if (Services.DataGenerator.MogStationDataParser.items.Contains(item.RowId))
        {
            CollectibleSources.Add(new MogStationCollectibleSource());
        }
        if (Services.DataGenerator.ContainersDataParser.itemsToContainers.ContainsKey(item.RowId))
        {
            CollectibleSources.AddRange(Services.DataGenerator.ContainersDataParser.itemsToContainers[item.RowId].Select(itemId => new ContainerCollectibleSource(itemId)));
        }
        if (Services.DataGenerator.AchievementsDataParser.itemToAchievement.ContainsKey(item.RowId))
        {
            CollectibleSources.Add(new AchievementCollectibleSource(Services.DataGenerator.AchievementsDataParser.itemToAchievement[item.RowId]));
        }
        if (Services.DataGenerator.QuestsDataParser.itemToQuest.ContainsKey(item.RowId))
        {
            CollectibleSources.AddRange(Services.DataGenerator.QuestsDataParser.itemToQuest[item.RowId].Select(entry => new QuestCollectibleSource(entry)));
        }
    }

    // Implementation
    private List<CollectibleSourceType> sourceTypes;
    public List<CollectibleSourceType> GetSourceTypes()
    {
        if (sourceTypes != null)
        {
            return sourceTypes;
        }

        sourceTypes = CollectibleSources.SelectMany(source => source.GetSourceType()).ToList();

        // Add any custom mappings from ContentTypeResolver
        sourceTypes.Add(Services.ContentTypeResolver.getSourceType(item));

        // Adding beast tribe currencies manually
        if (Services.DataGenerator.BeastTribesDataParser.itemToBeastTribe.ContainsKey(item.RowId))
        {
            sourceTypes.Add(CollectibleSourceType.BeastTribe);
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

    public void OpenGamerEscape()
    {
        GamerEscapeLink.OpenItem(item.Name);
    }

    public IDalamudTextureWrap GetIconLazy()
    {
        return IconHandler.GetIconLazy();
    }
}

