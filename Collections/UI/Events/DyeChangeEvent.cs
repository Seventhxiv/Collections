namespace Collections;

public class DyeChangeEventArgs : EventArgs
{
    public StainAdapter StainEntity0 { get; init; }
    public StainAdapter StainEntity1 { get; init; }
    public EquipSlot EquipSlot { get; init; }
    public DyeChangeEventArgs(EquipSlot equipSlot)
    {
        EquipSlot = equipSlot;
    }
}

public class DyeChangeEvent : Event<DyeChangeEventArgs>
{
}


