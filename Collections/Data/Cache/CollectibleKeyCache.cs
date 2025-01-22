namespace Collections;

public class CollectibleKeyCache<TCollectibleKey, TExcel> : ObjectCache<CollectibleKeyCache<TCollectibleKey, TExcel>, TCollectibleKey, (TExcel, int), (uint, int)>
    where TExcel : struct, IExcelRow<TExcel>
    where TCollectibleKey : ICreateable<TCollectibleKey, (TExcel, int)>
{

    protected override (uint, int) GetKey((TExcel, int) input)
    {
        return (input.Item1.RowId, input.Item2);
    }

    protected override (TExcel, int) GetInput((uint, int) key)
    {
        return ((TExcel)ExcelCache<TExcel>.GetSheet().GetRow(key.Item1)!, key.Item2);
    }
}
