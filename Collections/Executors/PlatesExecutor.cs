using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace Collections;

public unsafe class PlatesExecutor
{
    private static unsafe AgentMiragePrismMiragePlate* plateAgent => AgentMiragePrismMiragePlate.Instance();

    public static unsafe bool IsInPlateWindow()
    {
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
                SetPlateItem(PlateItemSource.Dresser, item.RowId, stainId);
            }

            // Look up in Armoire
            else if (Services.ItemFinder.IsItemInArmoire(item.RowId))
            {
                Dev.Log($"Found {item.Name} ({item.RowId}) in Armoire, Adding to plate with stain: {stainId}");
                //var cabinetId = (uint)Services.ItemFinder.CabinetIdFromItemId(item.RowId);
                SetPlateItem(PlateItemSource.Armoire, item.RowId, stainId);
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

    private static unsafe void SetPlateItem(PlateItemSource plateItemSource, uint itemId, byte stainId = 0)
    {
        Services.AddressResolver.setGlamourPlateSlot((IntPtr)plateAgent, plateItemSource, 0, itemId, stainId);
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
