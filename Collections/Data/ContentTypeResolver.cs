using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;

namespace Collections;

public class ContentTypeResolver
{
    private Dictionary<uint, CollectibleSourceType> itemIdToContentType = new()
    {
        //{ 25, CollectibleSourceType.WolfMarks },
        //{ 25, CollectibleSourceType.PvP },
    };
    private Dictionary<string, CollectibleSourceType> itemNameToContentType = new(StringComparer.InvariantCultureIgnoreCase)
    {
        { "Gil", CollectibleSourceType.Gil },
        { "Storm Seal", CollectibleSourceType.CompanySeals }, // designated company seals
        { "Wolf Mark", CollectibleSourceType.PvP },
        { "Gatherers' Scrip", CollectibleSourceType.Scrips },
        { "Crafters' Scrip", CollectibleSourceType.Scrips },
        { "Centurio Seal", CollectibleSourceType.HuntSeals },
        { "Allied Seal", CollectibleSourceType.HuntSeals },
        { "Sack Of Nuts", CollectibleSourceType.HuntSeals },
        { "MGP", CollectibleSourceType.MGP },
        { "Seafarer's Cowrie", CollectibleSourceType.IslandSanctuary },
        { "Gelmorran potsherd", CollectibleSourceType.DeepDungeon },
    };

    public ContentTypeResolver()
    {
        Dev.StartStopwatch();
        PopulateData();
        Dev.EndStopwatch();
    }

    // Does it's best to infer the content type from an item
    public CollectibleSourceType getSourceType(Item item)
    {
        if (itemIdToContentType.ContainsKey(item.RowId))
        {
            return itemIdToContentType[item.RowId];
        }
        return CollectibleSourceType.Other;
    }

    private void PopulateData()
    {
        // Looks up tomestone sheet
        var TomestonesItemSheet = Excel.GetExcelSheet<TomestonesItem>();
        foreach (var tomestone in TomestonesItemSheet)
        {
            itemIdToContentType[tomestone.Item.Row] = CollectibleSourceType.Tomestones;
        }

        // Populate based on manual data in itemNameToContentType
        var ItemSheet = Excel.GetExcelSheet<Item>();
        foreach (var item in ItemSheet)
        {
            if (itemNameToContentType.ContainsKey(item.Name))
            {
                var contentType = itemNameToContentType[item.Name];
                itemIdToContentType[item.RowId] = contentType;
            }
            else if (item.Name.ToString().EndsWith("Gatherers' Scrip") || item.Name.ToString().EndsWith("Crafters' Scrip"))
            {
                itemIdToContentType[item.RowId] = CollectibleSourceType.Scrips;
            }
        }
    }
}
