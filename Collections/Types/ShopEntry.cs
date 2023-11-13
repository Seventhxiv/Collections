namespace Collections;

public class ShopEntry
{
    public List<(ItemAdapter Item, int Amount)> Cost { get; set; }
    public uint? ENpcResidentId { get; set; }
    public ShopEntry(List<(ItemAdapter Item, int Amount)> cost, uint? eNpcResidentId)
    {
        Cost = cost;
        ENpcResidentId = eNpcResidentId;
    }
}
