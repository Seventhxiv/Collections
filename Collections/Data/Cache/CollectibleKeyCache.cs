namespace Collections;

public class CollectibleKeyCache<TCollectibleKey, TExcel> : ObjectCache<CollectibleKeyCache<TCollectibleKey, TExcel>, TCollectibleKey, (TExcel, bool), (uint, bool)>
    where TExcel : struct, IExcelRow<TExcel>
    where TCollectibleKey : ICreateable<TCollectibleKey, (TExcel, bool)>
{
    protected override (uint, bool) GetKey((TExcel, bool) input)
    {
        return (input.Item1.RowId, input.Item2);
    }

    protected override (TExcel, bool) GetInput((uint, bool) key)
    {
        return ((TExcel)ExcelCache<TExcel>.GetSheet().GetRow(key.Item1)!, key.Item2);
    }
}
