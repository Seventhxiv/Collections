using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace Collections;

public unsafe class DresserObserver
{
    public List<uint> DresserItemIds;
    public List<uint> ArmoireItemIds;

    private static MirageManager* MirageManager = FFXIVClientStructs.FFXIV.Client.Game.MirageManager.Instance();
    private static FFXIVClientStructs.FFXIV.Client.Game.UI.Cabinet Cabinet = UIState.Instance()->Cabinet;

    private bool firstTimeLoaded = true;
    private DateTime lastLoadTime;
    private const int RELOAD_THRESHOLD_IN_SECONDS = 5;
    private const int DRESSER_ITEM_LIMIT = 800;

    public DresserObserver()
    {
        InitializeContentsFromConfiguration();
    }

    public unsafe void OnFrameworkTick(IFramework framework)
    {
        if (IsDresserLoaded())
        {
            if (SecondsSinceLastLoad() > RELOAD_THRESHOLD_IN_SECONDS)
            {
                LoadDresserContents();

                // Only load Armoire when loading Dresser
                // Dresser loaded is good indication that we are at an Inn, and interacted with either Dresser or Armoire
                if (IsArmoireLoaded())
                {
                    LoadArmoireContents();
                }

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

    public bool IsArmoireLoaded()
    {
        var cabinet = UIState.Instance()->Cabinet;
        return cabinet.IsCabinetLoaded();
    }

    private unsafe double SecondsSinceLastLoad()
    {
        return (DateTime.Now - lastLoadTime).TotalSeconds;
    }


    private void LoadDresserContents()
    {
        lastLoadTime = DateTime.Now;
        var mirageManagerItemIds = new Span<uint>(MirageManager->PrismBoxItemIds, DRESSER_ITEM_LIMIT);

        var initialItemCount = DresserItemIds.Count;
        DresserItemIds.Clear();

        foreach (var itemId in mirageManagerItemIds)
        {
            if (itemId == 0)
                continue;

            var pureItemId = Services.ItemFinder.GetPureItemId(itemId);
            DresserItemIds.Add(pureItemId);
        }

        if (firstTimeLoaded)
        {
            Dev.Log($"New Dresser load state detected, reloading every {RELOAD_THRESHOLD_IN_SECONDS} seconds");
            Dev.Log($"Dresser contents count: {initialItemCount} -> {DresserItemIds.Count}");
        } else
        {
            Dev.Log($"Refreshing Dresser contents count: {initialItemCount} -> {DresserItemIds.Count}");
        }

        SaveDresserContentsInConfiguration();
    }

    private void LoadArmoireContents()
    {
        lastLoadTime = DateTime.Now;

        var initialItemCount = ArmoireItemIds.Count;
        ArmoireItemIds.Clear();

        var cabinetSheet = ExcelCache<Lumina.Excel.GeneratedSheets.Cabinet>.GetSheet();
        foreach (var cabinet in cabinetSheet)
        {
            if (Cabinet.IsItemInCabinet((int)cabinet.RowId))
            {
                var itemId = (uint)Services.ItemFinder.ItemIdFromCabinetId(cabinet.RowId);
                ArmoireItemIds.Add(itemId);
            }
        }

        if (firstTimeLoaded)
        {
            Dev.Log($"New Armoire load state detected, reloading every {RELOAD_THRESHOLD_IN_SECONDS} seconds");
            Dev.Log($"Armoire contents count: {initialItemCount} -> {ArmoireItemIds.Count}");
        }
        else
        {
            Dev.Log($"Refreshing Armoire contents count: {initialItemCount} -> {ArmoireItemIds.Count}");
        }

        SaveArmoireContentsInConfiguration();
    }

    private void InitializeContentsFromConfiguration()
    {
        DresserItemIds = Services.Configuration.DresserItemIds;
        ArmoireItemIds = Services.Configuration.ArmoireItemIds;
    }

    private void SaveDresserContentsInConfiguration()
    { 
        Services.Configuration.DresserItemIds = DresserItemIds;
        Services.Configuration.Save();
    }

    private void SaveArmoireContentsInConfiguration()
    {
        Services.Configuration.ArmoireItemIds = ArmoireItemIds;
        Services.Configuration.Save();
    }
}
