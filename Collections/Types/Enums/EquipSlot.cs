using static FFXIVClientStructs.FFXIV.Client.Game.Character.DrawDataContainer;

namespace Collections;

// Maintain order to set glamour plate pointer based on this index
public enum EquipSlot : uint
{
    MainHand,
    OffHand,
    Head,
    Body,
    Gloves,
    Waist, // Here to keep equipSlot Row ID = order
    Legs,
    Feet,
    Ears,
    Neck,
    Wrists,
    FingerR,
    FingerL,
    None,
}

class EquipSlotConverter
{
    public static WeaponSlot EquipSlotToWeaponSlot(EquipSlot equipSlot)
    {
        switch(equipSlot)
        {
            case EquipSlot.MainHand: return WeaponSlot.MainHand;
            case EquipSlot.OffHand: return WeaponSlot.OffHand;
            default: return WeaponSlot.Unk;
        }
    }

    public static EquipmentSlot EquipSlotToEquipmentSlot(EquipSlot equipSlot)
    {
        switch (equipSlot)
        {
            case EquipSlot.Head: return EquipmentSlot.Head;
            case EquipSlot.Body: return EquipmentSlot.Body;
            case EquipSlot.Gloves: return EquipmentSlot.Hands;
            case EquipSlot.Legs: return EquipmentSlot.Legs;
            case EquipSlot.Feet: return EquipmentSlot.Feet;
            case EquipSlot.Ears: return EquipmentSlot.Ears;
            case EquipSlot.Neck: return EquipmentSlot.Neck;
            case EquipSlot.Wrists: return EquipmentSlot.Wrists;
            case EquipSlot.FingerR: return EquipmentSlot.RFinger;
            case EquipSlot.FingerL: return EquipmentSlot.LFinger;
            default: throw new ArgumentException("Expected EquipSlot with possible EquipmentSlot match");
        }
    }
}