namespace Collections;

public class CollectibleKeyFactory
{
    public static ICollectibleKey Get<T>(Collectible<T> collectible) where T : ExcelRow
    {
        var type = typeof(T);
        var collectibleKeyDataGenerator = Services.DataGenerator.CollectibleKeyDataGenerator;
        var id = collectible.Id;

        if (type == typeof(ItemAdapter))
        {
            var item = ExcelCache<ItemAdapter>.GetSheet().GetRow(id);
            return CollectibleKeyCache<ItemCollectibleKey, ItemAdapter>.Instance.GetObject((item, true));
        }

        if (collectibleKeyDataGenerator.collectibleIdToItem.TryGetValue(type, out var itemDict))
        {
            if (itemDict.TryGetValue(id, out var item))
            {
                return CollectibleKeyCache<ItemCollectibleKey, ItemAdapter>.Instance.GetObject((item, true));
            }
        }
        if (collectibleKeyDataGenerator.collectibleIdToQuest.TryGetValue(type, out var questDict))
        {
            if (questDict.TryGetValue(id, out var quest))
            {
                return CollectibleKeyCache<QuestCollectibleKey, Quest>.Instance.GetObject((quest, true));
            }
        }
        if (collectibleKeyDataGenerator.collectibleIdToInstance.TryGetValue(type, out var instanceDict))
        {
            if (instanceDict.TryGetValue(id, out var instance))
            {
                return CollectibleKeyCache<InstanceCollectibleKey, ContentFinderCondition>.Instance.GetObject((instance, true));
            }
        }
        if (collectibleKeyDataGenerator.collectibleIdToAchievement.TryGetValue(type, out var achievementDict))
        {
            if (achievementDict.TryGetValue(id, out var achievement))
            {
                return CollectibleKeyCache<AchievementCollectibleKey, Achievement>.Instance.GetObject((achievement, true));
            }
        }
        if (collectibleKeyDataGenerator.collectibleIdToMisc.TryGetValue(type, out var miscDict))
        {
            if (miscDict.TryGetValue(id, out var misc))
            {
                return new MiscCollectibleKey(misc);
            }
        }
        if (type == typeof(Lumina.Excel.GeneratedSheets.Action))
        {
            if (Services.DataGenerator.BlueMageDataGenerator.ActionIdToBlueSpell.TryGetValue(id, out var monster))
            {
                return new MonsterCollectibleKey(monster);
            }
        }
        return null;
    }
}

