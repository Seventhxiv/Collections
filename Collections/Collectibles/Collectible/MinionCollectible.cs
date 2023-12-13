using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace Collections;

public class MinionCollectible : Collectible<Companion>, ICreateable<MinionCollectible, Companion>
{
    public new static string CollectionName => "Minions";

    public MinionCollectible(Companion excelRow) : base(excelRow)
    {
    }

    public static MinionCollectible Create(Companion excelRow)
    {
        return new(excelRow);
    }

    protected override string GetCollectionName()
    {
        return CollectionName;
    }

    protected override string GetName()
    {
        return ExcelRow.Singular;
    }

    protected override uint GetId()
    {
        return ExcelRow.RowId;
    }

    protected override string GetPrimaryDescription()
    {
        return "";
    }

    protected override string GetSecondaryDescription()
    {
        return "";
    }

    public override unsafe void UpdateObtainedState()
    {
        isObtained = UIState.Instance()->IsCompanionUnlocked(ExcelRow.RowId);
    }

    protected override int GetIconId()
    {
        return ExcelRow.Icon;
    }

    public override unsafe void Interact()
    {
        if (isObtained)
            ActionManager.Instance()->UseAction(ActionType.Companion, ExcelRow.RowId);
    }
}
