using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace Collections;

public class TripleTriadCollectible : Collectible<TripleTriadCard>, ICreateable<TripleTriadCollectible, TripleTriadCard>
{
    public new static string CollectionName => "Triple Triad";

    public TripleTriadCollectible(TripleTriadCard excelRow) : base(excelRow)
    {
    }

    public static TripleTriadCollectible Create(TripleTriadCard excelRow)
    {
        return new(excelRow);
    }

    protected override string GetCollectionName()
    {
        return CollectionName;
    }

    protected override string GetName()
    {
        return ExcelRow.Name.ToString();
    }

    protected override uint GetId()
    {
        return ExcelRow.RowId;
    }

    protected override string GetDescription()
    {
        return "";
    }

    protected override HintModule GetPrimaryHint()
    {
        return new HintModule("", null);
    }

    protected override HintModule GetSecondaryHint()
    {
        return new HintModule("", null);
    }

    public override unsafe void UpdateObtainedState()
    {
        isObtained = UIState.Instance()->IsTripleTriadCardUnlocked((ushort)ExcelRow.RowId);
    }

    protected override int GetIconId()
    {
        return (int)ExcelRow.RowId + 87000;
    }

    public override unsafe void Interact()
    {
        if (isObtained)
            ActionManager.Instance()->UseAction(ActionType.Companion, ExcelRow.RowId);
    }

    public override void OpenGamerEscape()
    {
        WikiOpener.OpenGamerEscape(GetDisplayName() + "_(Triple_Triad_Card)");
    }
}
