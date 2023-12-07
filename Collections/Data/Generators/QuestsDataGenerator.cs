namespace Collections;

public class QuestsDataGenerator
{
    public readonly Dictionary<uint, List<Quest>> itemToQuests = new();
    public readonly Dictionary<uint, Quest> emoteToQuest = new();

    public QuestsDataGenerator()
    {
        //Dev.Start();
        PopulateData();
        //Dev.Stop();
    }

    private void PopulateData()
    {
        var questSheet = ExcelCache<Quest>.GetSheet();
        foreach (var quest in questSheet)
        {
            var items = quest.ItemReward.ToList();
            items.AddRange(quest.OptionalItemReward.Select(entry => entry.Row).ToList());
            foreach (var itemId in items)
            {
                if (itemId == 0) continue;
                if (!itemToQuests.ContainsKey(itemId))
                {
                    itemToQuests[itemId] = new List<Quest>();
                }
                itemToQuests[itemId].Add(quest);
            }

            var emoteId = quest.EmoteReward.Row;
            if (emoteId != 0)
            {
                emoteToQuest[emoteId] = quest;
            }
        }
    }
}
