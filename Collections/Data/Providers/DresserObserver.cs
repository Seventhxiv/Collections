using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace Collections;

public unsafe class DresserObserver
{
    public List<uint> itemIds;

    private static MirageManager* MirageManager = FFXIVClientStructs.FFXIV.Client.Game.MirageManager.Instance();
    private bool firstTimeLoaded = true;
    private DateTime lastLoadTime;
    private const int RELOAD_THRESHOLD_IN_SECONDS = 20;
    private const int DRESSER_ITEM_LIMIT = 800;

    public DresserObserver()
    {
        InitializeDresserContentsFromConfiguration();
    }

    public unsafe void OnFrameworkTick(IFramework framework)
    {
        if (IsDresserLoaded())
        {
            if (SecondsSinceLastLoad() > RELOAD_THRESHOLD_IN_SECONDS)
            {
                LoadDresserContents();
                firstTimeLoaded = false;
            }
        } else
        {
            firstTimeLoaded = true;
        }
    }

    private unsafe bool IsDresserLoaded()
    {
        return MirageManager->PrismBoxLoaded;
    }

    private unsafe double SecondsSinceLastLoad()
    {
        return (DateTime.Now - lastLoadTime).TotalSeconds;
    }


    private void LoadDresserContents()
    {
        lastLoadTime = DateTime.Now;
        var mirageManagerItemIds = new Span<uint>(MirageManager->PrismBoxItemIds, DRESSER_ITEM_LIMIT);

        var initialItemCount = itemIds.Count;
        itemIds.Clear();

        foreach (var itemId in mirageManagerItemIds)
        {
            if (itemId == 0)
                continue;

            var pureItemId = itemId > 1000000 ? itemId - 1000000 : itemId;
            itemIds.Add(pureItemId);
        }

        if (firstTimeLoaded)
        {
            Dev.Log($"New Dresser load state detected, reloading every {RELOAD_THRESHOLD_IN_SECONDS} seconds");
            Dev.Log($"Dresser contents count: {initialItemCount} -> {itemIds.Count}");
        } else
        {
            Dev.Log($"Refreshing Dresser contents count: {initialItemCount} -> {itemIds.Count}");
        }

        SaveDresserContentsInConfiguration();
    }

    private void InitializeDresserContentsFromConfiguration()
    {
        itemIds = Services.Configuration.DresserItemIds;
    }

    private void SaveDresserContentsInConfiguration()
    { 
        Services.Configuration.DresserItemIds = itemIds;
        Services.Configuration.Save();
    }
}
