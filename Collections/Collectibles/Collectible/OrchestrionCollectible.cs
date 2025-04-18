using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace Collections;

public class OrchestrionCollectible : Collectible<ItemAdapter>, ICreateable<OrchestrionCollectible, ItemAdapter>
{
    public new static string CollectionName => "Orchestrions";

    public OrchestrionCollectible(ItemAdapter excelRow) : base(excelRow)
    {
    }

    public static OrchestrionCollectible Create(ItemAdapter excelRow)
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
        return GetOrchestrionFromUnlock().Description.ToString();
    }

    protected override HintModule GetPrimaryHint()
    {
        return new HintModule($"Patch {GetPatchAdded()}", null);
    }

    protected override HintModule GetSecondaryHint()
    {
        return new HintModule("", null);
    }

    public override unsafe void UpdateObtainedState()
    {
        isObtained = PlayerState.Instance()->IsOrchestrionRollUnlocked(GetOrchestrionFromUnlock().RowId);
    }

    protected override int GetIconId()
    {
        // lets hope squeenix never remove this orchestrion from the game lol.
        return ExcelRow.Icon;
    }

    public override unsafe void Interact()
    {
        // Do nothing
    }

    public Orchestrion GetOrchestrionFromUnlock()
    {
        return ExcelCache<Orchestrion>.GetSheet().GetRow(ExcelRow.AdditionalData.RowId).Value;
    }

}