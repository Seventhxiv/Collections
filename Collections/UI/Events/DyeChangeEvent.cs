using System;

namespace Collections;

public class DyeChangeEventArgs : EventArgs
{
    public StainAdapter StainEntity { get; init; }
    public EquipSlot EquipSlot { get; init; }
    public DyeChangeEventArgs(EquipSlot equipSlot, StainAdapter stainEntity)
    {
        EquipSlot = equipSlot;
        StainEntity = stainEntity;
    }
}

public class DyeChangeEvent : Event<DyeChangeEventArgs>
{
}


