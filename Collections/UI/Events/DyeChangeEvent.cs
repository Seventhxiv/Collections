namespace Collections;

public class DyeChangeEventArgs : EventArgs
{
    public StainAdapter StainEntity { get; init; }
    public EquipSlot EquipSlot { get; init; }
    public DyeChangeEventArgs(EquipSlot equipSlot)
    {
        EquipSlot = equipSlot;
    }
}

public class DyeChangeEvent : Event<DyeChangeEventArgs>
{
}


