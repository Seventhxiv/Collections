using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using System.Linq;

namespace Collections;

public class MinionCollectible : Collectible<Companion>
{
    protected override Companion excelRow { get; set; }
    public MinionCollectible(Companion excelRow) : base(excelRow)
    {
        // CollectibleUnlockItem
        if (Services.DataGenerator.CollectibleUnlockItemDataParser.minionUnlockItem.ContainsKey(excelRow.RowId))
        {
            var item = Services.DataGenerator.CollectibleUnlockItemDataParser.minionUnlockItem[excelRow.RowId];
            CollectibleUnlockItem = new CollectibleUnlockItem(item);
        }
        else
        {
            CollectibleUnlockItem = null;
        }
    }

    public override string GetName()
    {
        return excelRow.Singular;
    }

    public override unsafe void UpdateObtainedState()
    {
        isObtained = UIState.Instance()->IsCompanionUnlocked(excelRow.RowId);
    }

    protected override int GetIconId()
    {
        return excelRow.Icon;
    }

    public new static List<ICollectible> GetCollection()
    {
        var MinionSheet = Excel.GetExcelSheet<Companion>()!;
        return MinionSheet.AsParallel()
            .Where(entry => entry.Singular != null && entry.Singular != "")
            .Select(entry => (ICollectible)new MinionCollectible(entry))
            .ToList();
    }

    public new static string GetCollectionName()
    {
        return "Minions";
    }
}
