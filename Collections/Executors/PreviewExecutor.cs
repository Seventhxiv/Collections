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

    public void PreviewWithTryOnRestrictions(GlamourCollectible collectible, uint stain0Id, uint stain1Id, bool tryOn)
    {
        var tryOnOverride = tryOn || collectible.CollectibleKey.SourceCategories.Contains(SourceCategory.MogStation);
        if (tryOnOverride)
        {
            TryOn(collectible.ExcelRow.RowId, (byte)stain0Id, (byte)stain1Id);
        }
        else
        {
            Preview(collectible.ExcelRow, (byte)stain0Id, (byte)stain1Id);
        }
    }

    // Use when dye event is applied to a slot without a preview.
    // more expensive than through using the collectible.
    public void PreviewWithTryOnRestrictions(EquipSlot equipSlot, uint stain0Id, uint stain1Id, bool tryOn)
    {

        var itemSheet = ExcelCache<ItemAdapter>.GetSheet()!;
        var invSlot = InventoryManager.Instance()->GetInventorySlot(InventoryType.EquippedItems, EquipSlotConverter.EquipSlotToInventorySlot(equipSlot));
        var item = itemSheet.GetRow(invSlot->GlamourId != 0 ? invSlot->GlamourId : invSlot->ItemId);
        // if(stain0Id < 0) stain0Id = invSlot->Stains[0];
        // if(stain1Id < 0) stain0Id = invSlot->Stains[1];
        var tryOnOverride = tryOn; // no need to worry about already equipped mog items
        if (tryOnOverride)
        {
            TryOn(item.Value.RowId, (byte)stain0Id, (byte)stain1Id);
        }
        else
        {
            Preview(item.Value, (byte)stain0Id, (byte)stain1Id);
        }
    }

    private static void TryOn(uint item, byte stain0 = 0, byte stain1 = 0)
    {
        AgentTryon.TryOn(0xFF, item, stain0, stain1, item, false);
    }

    private void Preview(ItemAdapter item, byte stain0Id = 0, byte stain1Id = 0, bool storePreviewHistory = true)
    {
        Dev.Log($"Previewing {item.Name}");

        if (storePreviewHistory)
            previewHistory.Add(item.EquipSlot);

        if (item.EquipSlot == EquipSlot.MainHand || item.EquipSlot == EquipSlot.OffHand)
        {
            PreviewWeapon(item, stain0Id, stain1Id);
        }
        else
        {
            PreviewEquipment(item, stain0Id, stain1Id);
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
                Preview(item.Value, invSlot->Stains[0], invSlot->Stains[1], false);
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
                Stain1 = 0,
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
                Stain1 = 0,
                Variant = 0,
            };
            PreviewEquipment(equipSlot, equipmentModelId);
        }
        // Treat reset events as preview events so that reset catches and resets this slot.
        previewHistory.Add(equipSlot);
    }

    private unsafe void PreviewEquipment(ItemAdapter item, byte stain0Id, byte? stain1Id)
    {
        var equipmentModelId = GetEquipmentModelId(item, stain0Id, stain1Id);
        PreviewEquipment(item.EquipSlot, equipmentModelId);
    }

    private unsafe void PreviewEquipment(EquipSlot equipSlot, EquipmentModelId equipmentModelId)
    {
        var equipmentSlot = EquipSlotConverter.EquipSlotToEquipmentSlot(equipSlot);
        Character->DrawData.LoadEquipment(equipmentSlot, &equipmentModelId, true);
    }

    private void PreviewWeapon(ItemAdapter item, byte stain0Id, byte? stain1Id)
    {
        var weaponModelId = GetWeaponModelId(item, stain0Id, stain1Id);
        PreviewWeapon(item.EquipSlot, weaponModelId);
    }

    private void PreviewWeapon(EquipSlot equipSlot, WeaponModelId weaponModelId)
    {
        var weaponSlot = EquipSlotConverter.EquipSlotToWeaponSlot(equipSlot);
        Character->DrawData.LoadWeapon(weaponSlot, weaponModelId, 0, 0, 0, 0);
    }

    private EquipmentModelId GetEquipmentModelId(ItemAdapter item, byte stain0Id, byte? stain1Id)
    {
        return new EquipmentModelId()
        {
            Id = (ushort)item.ModelMain,
            Stain0 = stain0Id,
            Stain1 = stain1Id ?? 0,
            Variant = (byte)(item.ModelMain >> 16),
        };
    }

    private WeaponModelId GetWeaponModelId(ItemAdapter item, byte stain0Id, byte? stain1Id)
    {
        return new WeaponModelId()
        {
            Id = (ushort)item.ModelMain,
            Type = (byte)(item.ModelMain >> 16),
            Stain0 = stain0Id,
            Stain1 = stain1Id ?? 0,
            Variant = (byte)(item.ModelMain >> 32),
        };
    }
}
