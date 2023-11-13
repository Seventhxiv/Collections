namespace Collections;

public class GlamourSet
{
    public Dictionary<EquipSlot, GlamourItem?> set = new();
    public string name;
}

public class GlamourItem
{
    public GlamourCollectible glamourCollectible;
    public StainAdapter stain;
}