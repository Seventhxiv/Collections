namespace Collections;

[Sheet("EquipSlotCategory")]
public class EquipSlotCategoryAdapter : EquipSlotCategory
{
    public EquipSlot EquipSlot { get; set; }

    public override void PopulateData(RowParser parser, Lumina.GameData lumina, Language language)
    {
        base.PopulateData(parser, lumina, language);
        EquipSlot = GetEquipSlot();
    }

    private EquipSlot GetEquipSlot()
    {
        if (MainHand != 0)
            return EquipSlot.MainHand;
        if (OffHand != 0)
            return EquipSlot.OffHand;
        if (Head != 0)
            return EquipSlot.Head;
        if (Body != 0)
            return EquipSlot.Body;
        if (Gloves != 0)
            return EquipSlot.Gloves;
        if (Legs != 0)
            return EquipSlot.Legs;
        if (Feet != 0)
            return EquipSlot.Feet;
        return EquipSlot.None;

        // Can also handle this with reflections, doing it explicitly for now
        //foreach (var equipSlot in GetEnumValues<EquipSlot>())
        //{
        //    if (this.GetProperty<sbyte>(equipSlot.GetEnumName()) != 0)
        //    {
        //        EquipSlot = equipSlot;
        //        return;
        //    }
        //}
    }
}
