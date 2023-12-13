namespace Collections;

public class QuestsDataGenerator
{
    public readonly Dictionary<uint, HashSet<Quest>> ItemToQuests = new();
    public readonly Dictionary<uint, Quest> EmoteToQuest = new();

    public QuestsDataGenerator()
    {
        PopulateData();
    }

    private static readonly string FileName = "ItemIdToQuest.csv";
    private void PopulateData()
    {
        // Based on sheet
        var questSheet = ExcelCache<Quest>.GetSheet();
        foreach (var quest in questSheet)
        {
            var items = quest.ItemReward.ToList();
            items.AddRange(quest.OptionalItemReward.Select(entry => entry.Row).ToList());
            foreach (var itemId in items)
            {
                if (itemId == 0)
                    continue;
                AddEntry(itemId, quest);
            }

            var emoteId = quest.EmoteReward.Row;
            if (emoteId != 0)
            {
                EmoteToQuest[emoteId] = quest;
            }
        }

        // Based on resource data
        var resourceData = CSVHandler.Load<ItemIdToSource>(FileName);
        foreach (var entry in resourceData)
        {
            if (entry.SourceId == 0)
                continue;

            AddEntry(entry.ItemId, questSheet.GetRow(entry.SourceId));
        }
    }

    private void AddEntry(uint itemId, Quest quest)
    {
        if (!ItemToQuests.ContainsKey(itemId))
        {
            ItemToQuests[itemId] = new HashSet<Quest>();
        }
        ItemToQuests[itemId].Add(quest);
    }
}
