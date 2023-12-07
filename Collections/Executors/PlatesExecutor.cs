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

    public static unsafe void SetPlateItem(ItemAdapter item, byte stainId = 0)
    {
        try
        {
            if (!IsInPlateWindow())
                throw new ApplicationException("Attempt to edit Glamour Plate when plate agent is inactive");

            SetPlateAgentToEquipSlot(item.EquipSlot);

            // Look up in Dresser
            if (Services.ItemFinder.IsItemInDresser(item.RowId))
            {
                Dev.Log($"Found {item.Name} ({item.RowId}) in Dresser, Adding to plate with stain: {stainId}");
                var index = Services.DresserObserver.DresserItemIds.IndexOf(item.RowId);
                SetPlateItem(PlateItemSource.Dresser, index, item.RowId, stainId);
            }

            // Look up in Armoire
            else if (Services.ItemFinder.IsItemInArmoireCache(item.RowId))
            {
                Dev.Log($"Found {item.Name} ({item.RowId}) in Armoire, Adding to plate with stain: {stainId}");

                // Checking Armoire Loaded since it's not always loaded when in plates window
                if (!Services.DresserObserver.IsArmoireLoaded())
                {
                    Dev.Log($"Armoire not loaded, not applying {item.Name} ({item.RowId}) to plate");
                }
                var cabinetId = (int)Services.ItemFinder.CabinetIdFromItemId(item.RowId);
                SetPlateItem(PlateItemSource.Armoire, cabinetId, item.RowId, stainId);
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

    private static unsafe void SetPlateItem(PlateItemSource plateItemSource, int index, uint itemId, byte stainId = 0)
    {
        Services.AddressResolver.setGlamourPlateSlot((IntPtr)plateAgent, plateItemSource, index, itemId, stainId);
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
    Dresser = 1,
    Armoire = 2,
}
