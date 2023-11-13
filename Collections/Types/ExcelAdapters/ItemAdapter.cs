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

        // TODO - remove this
        if (RowId == 19178)
        {
            Dev.Log(19178.ToString());
        }
    }

    public void InitializeEquipSlot()
    {
        var equipSlotCategory = Excel.GetExcelSheet<EquipSlotCategoryAdapter>().GetRow(EquipSlotCategory.Row);
        EquipSlot = equipSlotCategory.EquipSlot;
        IsEquipment = EquipSlot != EquipSlot.None;
    }

    public void InitializeJobs()
    {
        if (IsEquipment)
        {
            var classJobCategory = Excel.GetExcelSheet<ClassJobCategoryAdapter>().GetRow(ClassJobCategory.Row);
            //Services.classJobCategorySheet[(int)ClassJobCategory.Value.RowId];
            Jobs = classJobCategory.Jobs;
        } else
        {
            Jobs = new List<Job>();
        }
    }
}