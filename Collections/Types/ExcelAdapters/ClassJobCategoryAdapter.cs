namespace Collections;

[Sheet("ClassJobCategory")]
public class ClassJobCategoryAdapter : ClassJobCategory
{
    public List<Job> Jobs { get; set; }

    public override void PopulateData(RowParser parser, Lumina.GameData lumina, Language language)
    {
        base.PopulateData(parser, lumina, language);
        InitializeJobs();
    }

    private void InitializeJobs()
    {
        Jobs = new List<Job>();
        foreach (var job in GetEnumValues<Job>())
        {
            if (this.GetProperty<bool>(job.GetEnumName()))
            {
                Jobs.Add(job);
            }
        }
    }
}
