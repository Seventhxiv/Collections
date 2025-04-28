using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace Collections;

public class OrchestrionCollectible : Collectible<Orchestrion>, ICreateable<OrchestrionCollectible, Orchestrion>
{
    public new static string CollectionName => "Orchestrions";

    public OrchestrionCollectible(Orchestrion excelRow) : base(excelRow)
    {
    }

    public static OrchestrionCollectible Create(Orchestrion excelRow)
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
        return ExcelRow.Description.ToString();
    }

    public override unsafe void UpdateObtainedState()
    {
        isObtained = PlayerState.Instance()->IsOrchestrionRollUnlocked(ExcelRow.RowId);
    }

    protected override int GetIconId()
    {
        return 25945; // A Cold Wind orchestrion item icon ID.
    }

    public override unsafe void Interact()
    {
        // Do nothing
    }

    public override void OpenGamerEscape()
    {
        WikiOpener.OpenGamerEscape(GetDisplayName() + "_Orchestrion_Roll");
    }
}