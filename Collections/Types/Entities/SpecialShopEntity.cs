using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;

namespace Collections;

[Sheet("SpecialShop")]
public class SpecialShopEntity : SpecialShop
{
    public override void PopulateData(RowParser parser, Lumina.GameData lumina, Language language)
    {
        base.PopulateData(parser, lumina, language);
        var x = UseCurrencyType;
        for (var i = 0; i < 60; i++)
        {
            Entries.Add(new Entry
            {
                Result = new List<ResultEntry> {
                        new ResultEntry {
                            Item = new LazyRow<Item>(lumina, parser.ReadColumn<int>(1 + i), language),
                            Count = parser.ReadColumn<uint>(61 + i),
                            //SpecialShopItemCategory = new LazyRow<SpecialShopItemCategory>(lumina, parser.ReadColumn<int>(121 + i), language),
                            HQ = parser.ReadColumn<bool>(181 + i)
                        },
                        new ResultEntry {
                            Item = new LazyRow<Item>(lumina, parser.ReadColumn<int>(241 + i), language),
                            Count = parser.ReadColumn<uint>(301 + i),
                            //SpecialShopItemCategory = new LazyRow<SpecialShopItemCategory>(lumina, parser.ReadColumn<int>(361 + i), language),
                            HQ = parser.ReadColumn<bool>(421 + i)
                        }
                    },
                Cost = new List<CostEntry> {
                        new CostEntry {
                            Item = new LazyRow<Item>(lumina, FixItemId(parser.ReadColumn<int>(481 + i)), language),
                            Count = parser.ReadColumn<uint>(541 + i),
                            HQ = parser.ReadColumn<bool>(601 + i),
                            Collectability = parser.ReadColumn<ushort>(661 + i)
                        },
                        new CostEntry {
                            Item = new LazyRow<Item>(lumina, FixItemId(parser.ReadColumn<int>(721 + i)), language),
                            Count = parser.ReadColumn<uint>(781 + i),
                            HQ = parser.ReadColumn<bool>(841 + i),
                            Collectability = parser.ReadColumn<ushort>(901 + i)
                        },
                        new CostEntry {
                            Item = new LazyRow<Item>(lumina, FixItemId(parser.ReadColumn<int>(961 + i)), language),
                            Count = parser.ReadColumn<uint>(1021 + i),
                            HQ = parser.ReadColumn<bool>(1081 + i),
                            Collectability = parser.ReadColumn<ushort>(1141 + i)
                        }
                    },
            }
            );
        }

    }
    public List<Entry> Entries = new();


    // Credit to https://github.com/Critical-Impact/CriticalCommonLib/blob/54b594f459ba1479a3bc67ad18ca65d206c63571/Sheets/SpecialShopListing.cs#L9
    // No idea why this is a thing...
    private int FixItemId(int itemId)
    {
        if (itemId == 0 || itemId > 7)
        {
            return itemId;
        }

        switch (UseCurrencyType)
        {
            // Scrips
            case 16:
                switch (itemId)
                {
                    case 1: return 28;
                    case 2: return 25199;
                    case 4: return 25200;
                    case 6: return 33913;
                    case 7: return 33914;
                    default: return itemId;
                }
            // Gil
            case 8:
                return 1;
            default:
                return itemId;
                // Tomestones
                //case 4:
                //    if (TomeStones.ContainsKey(itemId))
                //    {
                //        return TomeStones[itemId];
                //    }

                //    return itemId;
        }
    }

    public struct Entry
    {
        public List<ResultEntry> Result;
        public List<CostEntry> Cost;
    }

    public struct ResultEntry
    {
        public LazyRow<Item> Item;
        public uint Count;
        //public LazyRow<SpecialShopItemCategory> SpecialShopItemCategory;
        public bool HQ;
    }

    public struct CostEntry
    {
        public LazyRow<Item> Item;
        public uint Count;
        public bool HQ;
        public ushort Collectability;
    }
}
