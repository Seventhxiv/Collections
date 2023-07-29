using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;

namespace Collections;

public class ShopEntry
{
    public List<(Item Item, int Amount)> Cost { get; set; }
    public uint? ENpcResidentId { get; set; }
    public ShopEntry(List<(Item Item, int Amount)> cost, uint? eNpcResidentId)
    {
        Cost = cost;
        ENpcResidentId = eNpcResidentId;
    }
}
