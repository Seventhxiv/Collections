using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace Collections;

public class MinionCollectible : Collectible<ItemAdapter>, ICreateable<MinionCollectible, ItemAdapter>
{
    public new static string CollectionName => "Minions";

    public MinionCollectible(ItemAdapter excelRow) : base(excelRow)
    {
    }

    public static MinionCollectible Create(ItemAdapter excelRow)
    {
        return new(excelRow);
    }

    protected override string GetCollectionName()
    {
        return CollectionName;
    }

    protected override string GetName()
    {
        return ExcelRow.Singular.ToString();
    }

    protected override uint GetId()
    {
        return ExcelRow.RowId;
    }

    protected override string GetDescription()
    {
        return ExcelCache<CompanionTransient>.GetSheet().GetRow(getCompanionFromUnlock().RowId)?.Description.ToString() ?? "";
    }

    public override unsafe void UpdateObtainedState()
    {
        isObtained = UIState.Instance()->IsCompanionUnlocked(getCompanionFromUnlock().RowId);
    }

    protected override int GetIconId()
    {
        return ExcelRow.Icon;
    }

    public override unsafe void Interact()
    {
        if (isObtained)
            ActionManager.Instance()->UseAction(ActionType.Companion, getCompanionFromUnlock().RowId);
    }

    public override string GetDisplayName()
    {
        return Name
                .UpperCaseAfterSpaces()
                .LowerCaseWords(new List<string>() { "Of", "Up" });
    }

    private Companion getCompanionFromUnlock()
    {
        return ExcelCache<Companion>.GetSheet().GetRow(ExcelRow.ItemAction.Value.Data.ElementAt(0)).Value;
    }
}
