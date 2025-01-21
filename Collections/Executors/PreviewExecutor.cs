using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace Collections;

public unsafe class PreviewExecutor
{
    private HashSet<EquipSlot> previewHistory = new();
    private static Character* Character = (Character*)Services.ClientState.LocalPlayer.Address;

    public static bool IsInGPose()
    {
        return GameMain.IsInGPose();
    }

    public void PreviewWithTryOnRestrictions(GlamourCollectible collectible, uint stainId, bool tryOn)
    {
        var tryOnOverride = tryOn || collectible.CollectibleKey.SourceCategories.Contains(SourceCategory.MogStation);
        if (tryOnOverride)
        {
            TryOn(collectible.ExcelRow.RowId, (byte)stainId);
        }
        else
        {
            Preview(collectible.ExcelRow, (byte)stainId);
        }
    }

    private static void TryOn(uint item, byte stain = 0)
    {
        // Will need to implement second dye layer
        AgentTryon.TryOn(0xFF, item, stain, 0, item, false);
    }

    private void Preview(ItemAdapter item, byte stainId = 0, bool storePreviewHistory = true)
    {
        Dev.Log($"Previewing {item.Name}");

        if (storePreviewHistory)
            previewHistory.Add(item.EquipSlot);

        if (item.EquipSlot == EquipSlot.MainHand || item.EquipSlot == EquipSlot.OffHand)
        {
            PreviewWeapon(item, stainId);
        }
        else
        {
            PreviewEquipment(item, stainId);
        }
    }

    public unsafe void ResetAllPreview()
    {
        if (previewHistory.Count == 0)
        {
            return;
        }

        var itemSheet = ExcelCache<ItemAdapter>.GetSheet()!;
        var container = InventoryManager.Instance()->GetInventoryContainer(InventoryType.EquippedItems);

        // Reset slots
        foreach (var equipSlot in previewHistory)
        {
            ResetSlotPreview(equipSlot);
        }

        // Reapply current equipped gear
        for (var i = 0; i < 13; i++)
        {
            var invSlot = container->GetInventorySlot(i);
            var item = itemSheet.GetRow(invSlot->GlamourId != 0 ? invSlot->GlamourId : invSlot->ItemId);
            if (previewHistory.Contains(item.Value.EquipSlot))
                Preview(item.Value, invSlot->Stains[0], false);
        }
        previewHistory.Clear();
    }

    public void ResetSlotPreview(EquipSlot equipSlot)
    {
        if (equipSlot == EquipSlot.MainHand || equipSlot == EquipSlot.OffHand)
        {
            var weaponModelId = new WeaponModelId()
            {
                Id = 0,
                Type = 0,
                Stain0 = 0,
                Variant = 0,
            };
            PreviewWeapon(equipSlot, weaponModelId);
        }
        else
        {
            var equipmentModelId = new EquipmentModelId()
            {
                Id = 0,
                Stain0 = 0,
                Variant = 0,
            };
            PreviewEquipment(equipSlot, equipmentModelId);
        }
    }

    private unsafe void PreviewEquipment(ItemAdapter item, byte stainId)
    {
        var equipmentModelId = GetEquipmentModelId(item, stainId);
        PreviewEquipment(item.EquipSlot, equipmentModelId);
    }

    private unsafe void PreviewEquipment(EquipSlot equipSlot, EquipmentModelId equipmentModelId)
    {
        var equipmentSlot = EquipSlotConverter.EquipSlotToEquipmentSlot(equipSlot);
        Character->DrawData.LoadEquipment(equipmentSlot, &equipmentModelId, true);
    }

    private void PreviewWeapon(ItemAdapter item, byte stainId)
    {
        var weaponModelId = GetWeaponModelId(item, stainId);
        PreviewWeapon(item.EquipSlot, weaponModelId);
    }

    private void PreviewWeapon(EquipSlot equipSlot, WeaponModelId weaponModelId)
    {
        var weaponSlot = EquipSlotConverter.EquipSlotToWeaponSlot(equipSlot);
        Character->DrawData.LoadWeapon(weaponSlot, weaponModelId, 0, 0, 0, 0);
    }

    private EquipmentModelId GetEquipmentModelId(ItemAdapter item, byte stainId)
    {
        return new EquipmentModelId()
        {
            Id = (ushort)item.ModelMain,
            Stain0 = stainId,
            Variant = (byte)(item.ModelMain >> 16),
        };
    }

    private WeaponModelId GetWeaponModelId(ItemAdapter item, byte stainId)
    {
        return new WeaponModelId()
        {
            Id = (ushort)item.ModelMain,
            Type = (byte)(item.ModelMain >> 16),
            Stain0 = stainId,
            Variant = (byte)(item.ModelMain >> 32),
        };
    }
}
