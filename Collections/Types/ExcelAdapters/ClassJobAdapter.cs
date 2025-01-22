namespace Collections;
using Lumina.Excel.Sheets;

[Sheet("ClassJob")]
public struct ClassJobAdapter(ExcelPage page, uint offset, uint row) : IExcelRow<ClassJobAdapter>
{
    static ClassJobAdapter IExcelRow<ClassJobAdapter>.Create(ExcelPage page, uint offset, uint row)
    {
        var obj = new ClassJobAdapter(page, offset, row);
        obj.PopulateData();
        return obj;
    }

    public void PopulateData()
    {
        if (ClassJobConfig.ContainsKey(row))
        {
            IconId = ClassJobConfig[row].iconId;
            Job = ClassJobConfig[row].job;
            ClassRole = ClassJobConfig[row].classRole;
            iconHandler = new IconHandler(IconId);
        }
    }

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
        { 41, (Collections.Job.VPR, 62043, ClassRole.Melee)},

        { 23, (Collections.Job.BRD, 62405, ClassRole.Ranged)},
        { 31, (Collections.Job.MCH, 62411, ClassRole.Ranged)},
        { 38, (Collections.Job.DNC, 62418, ClassRole.Ranged)},

        { 25, (Collections.Job.BLM, 62407, ClassRole.Caster)},
        { 27, (Collections.Job.SMN, 62408, ClassRole.Caster)},
        { 35, (Collections.Job.RDM, 62415, ClassRole.Caster)},
        { 42, (Collections.Job.PCT, 62044, ClassRole.Caster)},
        { 36, (Collections.Job.BLU, 62416, ClassRole.Caster)},
    };

    private IconHandler? iconHandler { get; set; }
    public ISharedImmediateTexture? GetIconLazy()
    {
        iconHandler ??= new IconHandler(IconId);
        return iconHandler.GetIconLazy();
    }

    // Original
    public uint RowId => row;

    public readonly ReadOnlySeString Name => page.ReadString(offset, offset);
    public readonly ReadOnlySeString Abbreviation => page.ReadString(offset + 4, offset);
    public readonly ReadOnlySeString Unknown0 => page.ReadString(offset + 8, offset);
    public readonly bool CanQueueForDuty => page.ReadPackedBool(offset + 12, 0);
    public readonly ReadOnlySeString NameEnglish => page.ReadString(offset + 16, offset);
    public readonly RowRef<Item> ItemSoulCrystal => new(page.Module, page.ReadUInt32(offset + 20), page.Language);
    public readonly RowRef<Quest> UnlockQuest => new(page.Module, page.ReadUInt32(offset + 24), page.Language);
    public readonly RowRef<Quest> RelicQuest => new(page.Module, page.ReadUInt32(offset + 28), page.Language);
    public readonly RowRef<Quest> Prerequisite => new(page.Module, page.ReadUInt32(offset + 32), page.Language);
    public readonly int Unknown_70_1 => page.ReadInt32(offset + 36);
    public readonly int Unknown_70_2 => page.ReadInt32(offset + 40);
    public readonly RowRef<Item> ItemStartingWeapon => new(page.Module, (uint)page.ReadInt32(offset + 44), page.Language);
    public readonly int Unknown1 => page.ReadInt32(offset + 48);
    public readonly ushort ModifierHitPoints => page.ReadUInt16(offset + 52);
    public readonly ushort ModifierManaPoints => page.ReadUInt16(offset + 54);
    public readonly ushort ModifierStrength => page.ReadUInt16(offset + 56);
    public readonly ushort ModifierVitality => page.ReadUInt16(offset + 58);
    public readonly ushort ModifierDexterity => page.ReadUInt16(offset + 60);
    public readonly ushort ModifierIntelligence => page.ReadUInt16(offset + 62);
    public readonly ushort ModifierMind => page.ReadUInt16(offset + 64);
    public readonly ushort ModifierPiety => page.ReadUInt16(offset + 66);
    public readonly ushort Unknown2 => page.ReadUInt16(offset + 68);
    public readonly ushort Unknown3 => page.ReadUInt16(offset + 70);
    public readonly ushort Unknown4 => page.ReadUInt16(offset + 72);
    public readonly ushort Unknown5 => page.ReadUInt16(offset + 74);
    public readonly ushort Unknown6 => page.ReadUInt16(offset + 76);
    public readonly ushort Unknown7 => page.ReadUInt16(offset + 78);
    public readonly RowRef<Action> LimitBreak1 => new(page.Module, (uint)page.ReadUInt16(offset + 80), page.Language);
    public readonly RowRef<Action> LimitBreak2 => new(page.Module, (uint)page.ReadUInt16(offset + 82), page.Language);
    public readonly RowRef<Action> LimitBreak3 => new(page.Module, (uint)page.ReadUInt16(offset + 84), page.Language);
    public readonly RowRef<ClassJobCategory> ClassJobCategory => new(page.Module, (uint)page.ReadUInt8(offset + 86), page.Language);
    public readonly byte Unknown8 => page.ReadUInt8(offset + 87);
    public readonly byte JobIndex => page.ReadUInt8(offset + 88);
    public readonly byte Unknown9 => page.ReadUInt8(offset + 89);
    public readonly byte PvPActionSortRow => page.ReadUInt8(offset + 90);
    public readonly byte Unknown10 => page.ReadUInt8(offset + 91);
    public readonly RowRef<ClassJob> ClassJobParent => new(page.Module, (uint)page.ReadUInt8(offset + 92), page.Language);
    public readonly byte Role => page.ReadUInt8(offset + 93);
    public readonly RowRef<Town> StartingTown => new(page.Module, (uint)page.ReadUInt8(offset + 94), page.Language);
    public readonly byte PrimaryStat => page.ReadUInt8(offset + 95);
    public readonly byte UIPriority => page.ReadUInt8(offset + 96);
    public readonly byte StartingLevel => page.ReadUInt8(offset + 97);
    public readonly byte PartyBonus => page.ReadUInt8(offset + 98);
    public readonly byte Unknown11 => page.ReadUInt8(offset + 99);
    public readonly sbyte ExpArrayIndex => page.ReadInt8(offset + 100);
    public readonly sbyte BattleClassIndex => page.ReadInt8(offset + 101);
    public readonly sbyte DohDolJobIndex => page.ReadInt8(offset + 102);
    public readonly RowRef<MonsterNote> MonsterNote => new(page.Module, (uint)page.ReadInt8(offset + 103), page.Language);
    public readonly bool IsLimitedJob => page.ReadPackedBool(offset + 104, 0);
}

public enum ClassRole
{
    Tank,
    Healer,
    Melee,
    Ranged,
    Caster,
}
