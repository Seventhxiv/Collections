using Dalamud.Interface.Internal;
using ImGuiScene;
using Lumina.Data;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;

namespace Collections;

[Sheet("ClassJob")]
public class ClassJobEntity : ClassJob
{
    public static Dictionary<string, (int icon, ClassRole classRole)> ClassJobConfig = new()
    {
            { "PLD", (76822, ClassRole.Tank)},
            { "WAR", (76824, ClassRole.Tank)},
            { "DRK", (76835, ClassRole.Tank)},
            { "GNB", (76840, ClassRole.Tank)},

            { "WHM", (76827, ClassRole.Healer)},
            { "AST", (76836, ClassRole.Healer)},
            { "SCH", (76831, ClassRole.Healer)},
            { "SGE", (76843, ClassRole.Healer)},

            { "NIN", (76833, ClassRole.Melee)},
            { "DRG", (76825, ClassRole.Melee)},
            { "RPR", (76842, ClassRole.Melee)},
            { "MNK", (76823, ClassRole.Melee)},
            { "SAM", (76837, ClassRole.Melee)},

            { "BRD", (76826, ClassRole.Ranged)},
            { "MCH", (76834, ClassRole.Ranged)},
            { "DNC", (76841, ClassRole.Ranged)},

            { "BLM", (76828, ClassRole.Caster)},
            { "SMN", (76830, ClassRole.Caster)},
            { "RDM", (76838, ClassRole.Caster)},
            { "BLU", (76839, ClassRole.Caster)},
    };

    private IconHandler iconHandler { get; set; }
    public override void PopulateData(RowParser parser, Lumina.GameData lumina, Language language)
    {
        base.PopulateData(parser, lumina, language);
    }

    public IDalamudTextureWrap GetIconLazy()
    {
        if (iconHandler == null)
        {
            iconHandler = new IconHandler(GetIconIdFromClassJob());
        }
        return iconHandler.GetIconLazy();
    }

    private int GetIconIdFromClassJob()
    {
        return ClassJobConfig[Abbreviation].icon;
    }

    public ClassRole GetClassRole()
    {
        return ClassJobConfig[Abbreviation].classRole;
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
