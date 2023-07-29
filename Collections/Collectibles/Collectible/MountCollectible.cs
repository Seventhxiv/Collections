using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using System.Linq;

namespace Collections;

public class MountCollectible : Collectible<Mount>
{
    protected override Mount excelRow { get; set; }
    public MountCollectible(Mount excelRow) : base(excelRow)
    {
        // CollectibleUnlockItem
        if (Services.DataGenerator.CollectibleUnlockItemDataParser.mountUnlockItem.ContainsKey(excelRow.RowId))
        {
            var item = Services.DataGenerator.CollectibleUnlockItemDataParser.mountUnlockItem[excelRow.RowId];
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
        isObtained = PlayerState.Instance()->IsMountUnlocked(excelRow.RowId);
    }

    protected override int GetIconId()
    {
        return excelRow.Icon;
    }

    public new static List<ICollectible> GetCollection()
    {
        var mountSheet = Excel.GetExcelSheet<Mount>()!;
        return mountSheet.AsParallel()
            .Where(entry => entry.Singular != null && entry.Singular != "")
            .Select(entry => (ICollectible)new MountCollectible(entry))
            .ToList();
    }

    public new static string GetCollectionName()
    {
        return "Mounts";
    }
}
