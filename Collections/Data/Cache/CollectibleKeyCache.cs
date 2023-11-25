namespace Collections;

public class CollectibleKeyCache : ObjectCache<CollectibleKeyCache, CollectibleKey, (ItemAdapter, bool), (uint, bool)>
{
    protected override (uint, bool) GetKey((ItemAdapter, bool) input)
    {
        return (input.Item1.RowId, input.Item2);
    }

    protected override (ItemAdapter, bool) GetInput((uint, bool) key)
    {
        return (ExcelCache<ItemAdapter>.GetSheet().GetRow(key.Item1), key.Item2);
    }
}
