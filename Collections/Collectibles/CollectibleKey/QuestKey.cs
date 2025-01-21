namespace Collections;

public class QuestKey : CollectibleKey<(Quest, bool)>, ICreateable<QuestKey, (Quest, bool)>
{
    public QuestKey((Quest, bool) input) : base(input)
    {
    }

    public static QuestKey Create((Quest, bool) input)
    {
        return new(input);
    }

    protected override string GetName((Quest, bool) input)
    {
        return input.Item1.Name.ToString();
    }

    protected override uint GetId((Quest, bool) input)
    {
        return input.Item1.RowId;
    }

    protected override List<ICollectibleSource> GetCollectibleSources((Quest, bool) input)
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

