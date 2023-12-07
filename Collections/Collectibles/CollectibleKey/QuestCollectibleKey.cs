namespace Collections;

public class QuestCollectibleKey : CollectibleKey<Quest>, ICreateable<QuestCollectibleKey, (Quest, bool)>
{
    public override string Name { get; init; }

    private List<CollectibleSourceCategory> sourceCategories;

    public QuestCollectibleKey((Quest, bool) input) : base(input)
    {
        Name = excelRow.Name;
        CollectibleSources.Add(new QuestCollectibleSource(input.Item1));
        sourceCategories = new List<CollectibleSourceCategory>() { CollectibleSourceCategory.Quest };
    }

    public static QuestCollectibleKey Create((Quest, bool) input)
    {
        return new(input);
    }

    public override bool GetIsTradeable()
    {
        return false;
    }

    public override List<CollectibleSourceCategory> GetSourceCategories()
    {
        return sourceCategories;
    }

    public override int? GetMarketBoardPriceLazy()
    {
        throw new NotImplementedException();
    }
}

