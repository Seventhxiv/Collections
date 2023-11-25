namespace Collections;

[Sheet("Item")]
public class ItemAdapter : Item
{
    public List<Job> Jobs { get; set; }
    public EquipSlot EquipSlot { get; set; }
    public bool IsEquipment { get; set; }

    public override void PopulateData(RowParser parser, Lumina.GameData lumina, Language language)
    {
        base.PopulateData(parser, lumina, language);
        InitializeEquipSlot();
        InitializeJobs();
    }

    public void InitializeEquipSlot()
    {
        var equipSlotCategory = ExcelCache<EquipSlotCategoryAdapter>.GetSheet().GetRow(EquipSlotCategory.Row);
        EquipSlot = equipSlotCategory.EquipSlot;
        IsEquipment = EquipSlot != EquipSlot.None;
    }

    public void InitializeJobs()
    {
        if (IsEquipment)
        {
            var classJobCategory = ExcelCache<ClassJobCategoryAdapter>.GetSheet().GetRow(ClassJobCategory.Row);
            //Services.classJobCategorySheet[(int)ClassJobCategory.Value.RowId];
            Jobs = classJobCategory.Jobs;
        } else
        {
            Jobs = new List<Job>();
        }
    }
}