namespace Collections;

public class CollectibleKeyFactory
{
    public static ICollectibleKey Get<T>(Collectible<T> collectible) where T : struct, IExcelRow<T>
    {
        var type = typeof(T);
        var keysDataGenerator = Services.DataGenerator.KeysDataGenerator;
        var id = collectible.Id;

        if (type == typeof(ItemAdapter))
        {
            var item = (ItemAdapter)ExcelCache<ItemAdapter>.GetSheet().GetRow(id)!;
            return CollectibleKeyCache<ItemKey, ItemAdapter>.Instance.GetObject((item, true));
        }

        if (keysDataGenerator.collectibleIdToItem.TryGetValue(type, out var itemDict))
        {
            if (itemDict.TryGetValue(id, out var item))
            {
                return CollectibleKeyCache<ItemKey, ItemAdapter>.Instance.GetObject((item, true));
            }
        }
        if (keysDataGenerator.collectibleIdToQuest.TryGetValue(type, out var questDict))
        {
            if (questDict.TryGetValue(id, out var quest))
            {
                return CollectibleKeyCache<QuestKey, Quest>.Instance.GetObject((quest, false));
            }
        }
        if (keysDataGenerator.collectibleIdToInstance.TryGetValue(type, out var instanceDict))
        {
            if (instanceDict.TryGetValue(id, out var instance))
            {
                return CollectibleKeyCache<InstanceKey, ContentFinderCondition>.Instance.GetObject((instance, true));
            }
        }
        if (keysDataGenerator.collectibleIdToAchievement.TryGetValue(type, out var achievementDict))
        {
            if (achievementDict.TryGetValue(id, out var achievement))
            {
                return CollectibleKeyCache<AchievementKey, Achievement>.Instance.GetObject((achievement, true));
            }
        }
        if (keysDataGenerator.collectibleIdToMisc.TryGetValue(type, out var miscDict))
        {
            if (miscDict.TryGetValue(id, out var misc))
        {
                return new MiscKey((misc, false));
            }
        }
        if (type == typeof(Lumina.Excel.Sheets.Action))
        {
            if (Services.DataGenerator.KeysDataGenerator.ActionIdToBlueSpell.TryGetValue(id, out var monster))
            {
                return new MonsterKey((monster, false));
            }
        }
        return null;
    }
}

