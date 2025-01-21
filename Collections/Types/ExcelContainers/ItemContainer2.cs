// namespace Collections;

// public struct ItemContainer2(ExcelPage page, uint offset, uint row) : IExcelRow<Item>
// {
//     static Item IExcelRow<Item>.Create(ExcelPage page, uint offset, uint row) =>
//     new(page, offset, row);

//     public List<Job> Jobs { get; set; }
//     public EquipSlot EquipSlot { get; set; }
//     public bool IsEquipment { get; set; }

//     public ItemContainer(Item ExcelRow) : base(ExcelRow)
//     {
//         EquipSlot = GetEquipSlot();
//         Jobs = GetJobs();
//         IsEquipment = EquipSlot != EquipSlot.None;
//     }

//     public EquipSlot GetEquipSlot()
//     {
//         var equipSlotCategory = ExcelCache<EquipSlotCategoryAdapter>.GetSheet().GetRow(ExcelRow.EquipSlotCategory.RowId);
//         return equipSlotCategory.EquipSlot;
//     }

//     public List<Job> GetJobs()
//     {
//         if (IsEquipment)
//         {
//             var classJobCategory = ExcelCache<ClassJobCategoryAdapter>.GetSheet().GetRow(ExcelRow.ClassJobCategory.RowId);
//             //Services.classJobCategorySheet[(int)ClassJobCategory.Value.RowId];
//             Jobs = classJobCategory.Jobs;
//         }
//         else
//         {
//             Jobs = new List<Job>();
//         }
//         return Jobs;
//     }
// }