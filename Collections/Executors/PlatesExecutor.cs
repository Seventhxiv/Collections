using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace Collections;

public unsafe class PlatesExecutor
{
    private static unsafe AgentMiragePrismMiragePlate* plateAgent => AgentMiragePrismMiragePlate.Instance();

    public static unsafe bool IsInPlateWindow()
    {
        AgentCharaCard.Instance();
        return plateAgent->AgentInterface.IsAgentActive();
    }

    public static unsafe void SetPlateItem(ItemAdapter item, byte stain0Id = 0, byte stain1Id = 0)
    {
        try
        {
            if (!IsInPlateWindow())
                throw new ApplicationException("Attempt to edit Glamour Plate when plate agent is inactive");

            SetPlateAgentToEquipSlot(item.EquipSlot);

            // Look up in Dresser
            if (Services.ItemFinder.IsItemInDresser(item.RowId))
            {
                Dev.Log($"Found {item.Name} ({item.RowId}) in Dresser, Adding to plate with stain: {stain0Id}, {stain1Id}");
                var index = Services.DresserObserver.DresserItemIds.IndexOf(item.RowId);
                SetPlateItem(PlateItemSource.Dresser, index, item.RowId, stain0Id, stain1Id);
            }

            // Look up in Armoire
            else if (Services.ItemFinder.IsItemInArmoireCache(item.RowId))
            {
                Dev.Log($"Found {item.Name} ({item.RowId}) in Armoire, Adding to plate with stain: {stain0Id}, {stain1Id}");

                // Checking Armoire Loaded since it's not always loaded when in plates window
                if (!Services.DresserObserver.IsArmoireLoaded())
                {
                    Dev.Log($"Armoire not loaded, not applying {item.Name} ({item.RowId}) to plate");
                }
                var cabinetId = (int)Services.ItemFinder.CabinetIdFromItemId(item.RowId);
                SetPlateItem(PlateItemSource.Armoire, cabinetId, item.RowId, stain0Id, stain1Id);
            }

            else
            {
                Dev.Log($"Couldn't find {item.Name} in Dresser or Armoire");
            }
        }
        catch (Exception e)
        {
            Dev.Log(e.ToString());
        }
    }

    private static unsafe void SetPlateItem(PlateItemSource plateItemSource, int index, uint itemId, byte stain0Id = 0, byte stain1Id = 0)
    {
        plateAgent->SetSelectedItemData((AgentMiragePrismMiragePlateData.ItemSource)plateItemSource, (uint)index, itemId, stain0Id, stain1Id);
    }

    private static unsafe void SetPlateAgentToEquipSlot(EquipSlot equipSlot)
    {
        var editorInfo = *(IntPtr*)((IntPtr)plateAgent + 0x28);
        var slotPtr = (EquipSlot*)(editorInfo + 0x18);
        *slotPtr = equipSlot;
    }
}

public enum PlateItemSource
{
    Dresser = AgentMiragePrismMiragePlateData.ItemSource.PrismBox,
    Armoire = AgentMiragePrismMiragePlateData.ItemSource.Cabinet,
}
