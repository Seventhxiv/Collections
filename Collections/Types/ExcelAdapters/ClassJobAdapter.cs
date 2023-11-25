namespace Collections;

[Sheet("ClassJob")]
public class ClassJobAdapter : ClassJob
{
    public Job? Job { get; set; }
    public ClassRole ClassRole { get; set; }
    public int IconId { get; set; }

    public static readonly Dictionary<uint, (Job job, int iconId, ClassRole classRole)> ClassJobConfig = new()
    {
        { 19, (Collections.Job.PLD, 62401, ClassRole.Tank)},
        { 21, (Collections.Job.WAR, 62403, ClassRole.Tank)},
        { 32, (Collections.Job.DRK, 62412, ClassRole.Tank)},
        { 37, (Collections.Job.GNB, 62417, ClassRole.Tank)},

        { 24, (Collections.Job.WHM, 62406, ClassRole.Healer)},
        { 33, (Collections.Job.AST, 62413, ClassRole.Healer)},
        { 28, (Collections.Job.SCH, 62409, ClassRole.Healer)},
        { 40, (Collections.Job.SGE, 62420, ClassRole.Healer)},

        { 30, (Collections.Job.NIN, 62410, ClassRole.Melee)},
        { 22, (Collections.Job.DRG, 62404, ClassRole.Melee)},
        { 39, (Collections.Job.RPR, 62419, ClassRole.Melee)},
        { 20, (Collections.Job.MNK, 62402, ClassRole.Melee)},
        { 34, (Collections.Job.SAM, 62414, ClassRole.Melee)},

        { 23, (Collections.Job.BRD, 62405, ClassRole.Ranged)},
        { 31, (Collections.Job.MCH, 62411, ClassRole.Ranged)},
        { 38, (Collections.Job.DNC, 62418, ClassRole.Ranged)},

        { 25, (Collections.Job.BLM, 62407, ClassRole.Caster)},
        { 27, (Collections.Job.SMN, 62408, ClassRole.Caster)},
        { 35, (Collections.Job.RDM, 62415, ClassRole.Caster)},
        { 36, (Collections.Job.BLU, 62416, ClassRole.Caster)},
    };

    private IconHandler iconHandler { get; set; }
    public override void PopulateData(RowParser parser, Lumina.GameData lumina, Language language)
    {
        base.PopulateData(parser, lumina, language);
        if (ClassJobConfig.ContainsKey(RowId))
        {
            IconId = ClassJobConfig[RowId].iconId;
            Job = ClassJobConfig[RowId].job;
            ClassRole = ClassJobConfig[RowId].classRole;
        }
    }

    public IDalamudTextureWrap GetIconLazy()
    {
        iconHandler ??= new IconHandler(IconId);
        return iconHandler.GetIconLazy();
    }
}

public enum ClassRole
{
    Tank,
    Healer,
    Melee,
    Ranged,
    Caster,
}
