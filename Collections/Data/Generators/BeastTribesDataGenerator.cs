namespace Collections;

public class BeastTribesDataGenerator
{
    public Dictionary<uint, BeastTribe> itemToBeastTribe = new();

    public BeastTribesDataGenerator()
    {
        //Dev.Start();
        populateData();
        //Dev.Stop();
    }

    private void populateData()
    {
        var beastTribeSheet = ExcelCache<BeastTribe>.GetSheet();
        foreach (var beastTribe in beastTribeSheet)
        {
            itemToBeastTribe[beastTribe.CurrencyItem.Row] = beastTribe;
        }
    }
}
