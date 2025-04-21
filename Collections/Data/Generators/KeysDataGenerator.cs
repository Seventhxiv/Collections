namespace Collections;

public class KeysDataGenerator
{
    public readonly Dictionary<Type, Dictionary<uint, ItemAdapter>> collectibleIdToItem = new();
    public readonly Dictionary<Type, Dictionary<uint, Quest>> collectibleIdToQuest = new();
    public readonly Dictionary<Type, Dictionary<uint, ContentFinderCondition>> collectibleIdToInstance = new();
    public readonly Dictionary<Type, Dictionary<uint, Achievement>> collectibleIdToAchievement = new();
    public readonly Dictionary<Type, Dictionary<uint, string>> collectibleIdToMisc = new();
    public Dictionary<uint, Monster> ActionIdToBlueSpell = new();
    public Dictionary<uint, uint> ItemIdToTripleTriadId = new();

    private static readonly int MountItemActionType = 1322;
    private static readonly int MinionItemActionType = 853;
    private static readonly int EmoteHairstyleItemActionType = 2633;
    private static readonly int TripleTriadItemActionType = 3357;
    private static readonly int BardingItemActionType = 1013;
    private static readonly int OrchestrionItemActionType = 25183;
    

    public KeysDataGenerator()
    {
        PopulateItemData();
        PopulateQuestData();
        PopulateInstanceData();
        PopulateAchievementData();
        PopulateMiscData();
        PopulateBlueSpellData();
    }

    private void PopulateItemData()
    {
        foreach (var item in ExcelCache<ItemAdapter>.GetSheet())
        {
            var type = item.ItemAction.Value.Type;
            var collectibleData = item.ItemAction.Value.Data;
            if (type == MountItemActionType)
            {
                AddCollectibleKeyEntry(collectibleIdToItem, typeof(Mount), collectibleData[0], item);
            }
            else if (type == MinionItemActionType)
            {
                AddCollectibleKeyEntry(collectibleIdToItem, typeof(Companion), collectibleData[0], item);
            }
            else if (type == EmoteHairstyleItemActionType)
            {
                AddCollectibleKeyEntry(collectibleIdToItem, typeof(Emote), collectibleData[0], item);
                AddCollectibleKeyEntry(collectibleIdToItem, typeof(CharaMakeCustomize), collectibleData[0], item);
            }
            else if (type == TripleTriadItemActionType)
            {
                AddCollectibleKeyEntry(collectibleIdToItem, typeof(TripleTriadCard), collectibleData[0], item);
                ItemIdToTripleTriadId[item.RowId] = collectibleData[0]; // Maintain reverse look up for triple triad cards
            }
            else if (type == BardingItemActionType)
            {
                AddCollectibleKeyEntry(collectibleIdToItem, typeof(BuddyEquip), collectibleData[0], item);
            }
        }
    }

    private void PopulateQuestData()
    {
        foreach (var quest in ExcelCache<Quest>.GetSheet())
        {
            var emote = quest.EmoteReward.Value;
            if (emote.RowId != 0)
            {
                AddCollectibleKeyEntry(collectibleIdToQuest, typeof(Emote), emote.UnlockLink, quest);
            }
        }

        foreach (var emote in ExcelCache<Emote>.GetSheet())
        {
            if (emote.UnlockLink > ExcelCache<Quest>.GetSheet().First().RowId && emote.UnlockLink < ExcelCache<Quest>.GetSheet().Last().RowId)
            {
                var quest = (Quest)ExcelCache<Quest>.GetSheet().GetRow(emote.UnlockLink)!;
                AddCollectibleKeyEntry(collectibleIdToQuest, typeof(Emote), emote.UnlockLink, quest);
            }
        }

        foreach (var (type, dict) in DataOverrides.collectibleIdToUnlockQuestId)
        {
            foreach (var (collectibleId, questId) in dict)
            {
                var quest = (Quest)ExcelCache<Quest>.GetSheet().GetRow(questId)!;
                AddCollectibleKeyEntry(collectibleIdToQuest, type, collectibleId, quest);
            }
        }
    }

    private void PopulateInstanceData()
    {
        foreach (var (type, dict) in DataOverrides.collectibleIdToUnlockInstanceId)
        {
            foreach (var (collectibleId, instanceId) in dict)
            {
                var instance = (ContentFinderCondition)ExcelCache<ContentFinderCondition>.GetSheet().GetRow(instanceId)!;
                AddCollectibleKeyEntry(collectibleIdToInstance, type, collectibleId, instance);
            }
        }
    }

    private void PopulateAchievementData()
    {
        foreach (var (type, dict) in DataOverrides.collectibleIdToUnlockAchievementId)
        {
            foreach (var (collectibleId, achievementId) in dict)
            {
                var achievement = (Achievement)ExcelCache<Achievement>.GetSheet().GetRow(achievementId)!;
                AddCollectibleKeyEntry(collectibleIdToAchievement, type, collectibleId, achievement);
            }
        }
    }

    private void PopulateMiscData()
    {
        foreach (var (type, dict) in DataOverrides.collectibleIdToUnlockMisc)
        {
            foreach (var (collectibleId, misc) in dict)
            {
                AddCollectibleKeyEntry(collectibleIdToMisc, type, collectibleId, misc);
            }
        }
    }

    private static readonly string BlueSpellsFileName = "BlueSpells.csv";
    private void PopulateBlueSpellData()
    {
        var data = CSVHandler.Load<BlueSpell>(BlueSpellsFileName);
        ActionIdToBlueSpell = data
            .GroupBy(entry => entry.ActionId)
            .ToDictionary(kv => kv.Key, kv =>
            {
                var blueSpell = kv.First();
                return new Monster()
                {
                    name = blueSpell.MobDescription,
                    LocationDescription = blueSpell.LocationDescription,
                    dutyId = blueSpell.DutyId,
                    territoryId = blueSpell.TerritoryId,
                    X = blueSpell.X,
                    Y = blueSpell.Y,
                };
            });
    }

    private void AddCollectibleKeyEntry<T>(Dictionary<Type, Dictionary<uint, T>> dict, Type type, uint id, T entry)
    {
        if (!dict.ContainsKey(type))
        {
            dict[type] = new Dictionary<uint, T>();
        }
        dict[type][id] = entry;
    }
}

public class BlueSpell
{
    public uint ActionId { get; set; }
    public string MobDescription { get; set; }
    public string LocationDescription { get; set; }
    public uint? DutyId { get; set; }
    public uint? TerritoryId { get; set; }
    public float? X { get; set; }
    public float? Y { get; set; }
}