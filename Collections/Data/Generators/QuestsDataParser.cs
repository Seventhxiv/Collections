using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using System.Linq;

namespace Collections;

public class QuestsDataParser
{
    public readonly Dictionary<uint, List<Quest>> itemToQuest = new();
    public readonly Dictionary<uint, Quest> emoteToQuest = new();

    public QuestsDataParser()
    {
        Dev.StartStopwatch();
        PopulateData();
        Dev.EndStopwatch();
    }

    private void PopulateData()
    {
        var questSheet = Excel.GetExcelSheet<Quest>();
        var itemSheet = Excel.GetExcelSheet<Item>();
        foreach (var quest in questSheet)
        {
            var items = quest.ItemReward.ToList();
            items.AddRange(quest.OptionalItemReward.Select(entry => entry.Row).ToList());
            foreach (var itemId in items)
            {
                if (itemId == 0) continue;
                if (!itemToQuest.ContainsKey(itemId))
                {
                    itemToQuest[itemId] = new List<Quest>();
                }
                itemToQuest[itemId].Add(quest);
            }

            var emoteId = quest.EmoteReward.Row;
            if (emoteId != 0)
            {
                emoteToQuest[emoteId] = quest;
            }
        }
    }
}
