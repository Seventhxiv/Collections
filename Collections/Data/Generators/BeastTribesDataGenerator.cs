using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;

namespace Collections;

public class BeastTribesDataGenerator
{
    public Dictionary<uint, BeastTribe> itemToBeastTribe = new();

    public BeastTribesDataGenerator()
    {
        Dev.StartStopwatch();
        populateData();
        Dev.EndStopwatch();
    }

    private void populateData()
    {
        var beastTribeSheet = Excel.GetExcelSheet<BeastTribe>();
        foreach (var beastTribe in beastTribeSheet)
        {
            itemToBeastTribe[beastTribe.CurrencyItem.Row] = beastTribe;
        }
    }
}
