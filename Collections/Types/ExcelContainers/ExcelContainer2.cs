// namespace Collections;

// public struct ExcelContainer2<T>(ExcelPage page, uint offset, uint row) : IExcelRow<T> where T : struct, IExcelRow<T>
// {
//     public uint RowId => row;

//     static T IExcelRow<T>.Create(ExcelPage page, uint offset, uint row)
//     {
//         var obj = (T?)Activator.CreateInstance(typeof(T), [page, offset, row]);
//         if (obj != null)
//         {
//             return (T)obj;
//         }
//         else
//         {
//             throw new Exception("Could not create object");
//         }
//     }

// }