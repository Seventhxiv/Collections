namespace Collections;

public class QuestKey : CollectibleKey<(Quest, int)>, ICreateable<QuestKey, (Quest, int)>
{
    public QuestKey((Quest, int) input) : base(input)
    {
    }

    public static QuestKey Create((Quest, int) input)
    {
        return new(input);
    }

    protected override string GetName((Quest, int) input)
    {
        return input.Item1.Name.ToString();
    }

    protected override uint GetId((Quest, int) input)
    {
        return input.Item1.RowId;
    }

    protected override List<ICollectibleSource> GetCollectibleSources((Quest, int) input)
    {
        return new List<ICollectibleSource>() { new QuestSource(input.Item1) };
    }

    protected override HashSet<SourceCategory> GetBaseSourceCategories()
    {
        return new HashSet<SourceCategory>() { SourceCategory.Quest };
    }

    public override Tradeability GetIsTradeable()
    {
        return Tradeability.Untradeable;
    }
}

