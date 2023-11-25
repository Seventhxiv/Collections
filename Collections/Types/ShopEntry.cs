namespace Collections;

public class ShopEntry
{
    public List<(ItemAdapter Item, int Amount)> Cost { get; init; }
    public uint? ENpcResidentId { get; init; }
    public uint ShopId { get; init; }
    public ShopEntry(List<(ItemAdapter Item, int Amount)> cost, uint? eNpcResidentId, uint shopId)
    {
        Cost = cost;
        ENpcResidentId = eNpcResidentId;
        ShopId = shopId;
    }
}
