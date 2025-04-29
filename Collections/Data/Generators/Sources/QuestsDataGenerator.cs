namespace Collections;

public class QuestsDataGenerator : BaseDataGenerator<Quest>
{
    public readonly Dictionary<uint, Quest> EmoteToQuest = new(); // TODO connect

    private static readonly string FileName = "ItemIdToQuest.csv";
    protected override void InitializeData()
    {
        // Based on sheet
        var questSheet = ExcelCache<Quest>.GetSheet();
        foreach (var quest in questSheet)
        {
            var rewards = quest.Reward;
            var items = new List<uint>();

            foreach (var reward in rewards)
            {
                if (reward.GetType() == typeof(Item))
                {
                    items.Add(reward.RowId);
                }
            }
            items.AddRange(quest.OptionalItemReward.Select(entry => entry.RowId).ToList());
            foreach (var itemId in items)
            {
                if (itemId == 0)
                    continue;
                AddEntry(itemId, quest);
            }

            var emoteId = quest.EmoteReward.RowId;
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

            var quest = questSheet.GetRow(entry.SourceId);
            if (quest != null)
            {
                AddEntry(entry.ItemId, quest.Value);
            }
        }
    }
}
