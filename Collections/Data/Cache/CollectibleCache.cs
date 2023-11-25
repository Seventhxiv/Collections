namespace Collections;

public class CollectibleCache<TCollectible, TExcel> : ObjectCache<CollectibleCache<TCollectible, TExcel>, TCollectible, TExcel, uint>
    where TExcel : ExcelRow
    where TCollectible : ICreateable<TCollectible, TExcel>
{
    protected override uint GetKey(TExcel input)
    {
        return input.RowId;
    }

    protected override TExcel GetInput(uint key)
    {
        return ExcelCache<TExcel>.GetSheet().GetRow(key);
    }
}

