namespace Collections;

public class CollectibleCache<TCollectible, TExcel> : ObjectCache<CollectibleCache<TCollectible, TExcel>, TCollectible, TExcel, uint>
    where TExcel : struct, IExcelRow<TExcel>
    where TCollectible : ICreateable<TCollectible, TExcel>
{
    protected override uint GetKey(TExcel input)
    {
        return input.RowId;
    }

    protected override TExcel GetInput(uint key)
    {
        return (TExcel)ExcelCache<TExcel>.GetSheet().GetRow(key)!;
    }
}

