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
        // unless we have a better idea, 
        return ExcelCache<ItemAdapter>.GetSheet().GetRow(CollectibleKey.Id).Value.Icon;
    }

    public override unsafe void Interact()
    {
        // Do nothing
    }

    public ItemAdapter? GetUnlockItem()
    {
        if(CollectibleKey != null)
        {
            return ExcelCache<ItemAdapter>.GetSheet().GetRow(CollectibleKey.Id);
        }
        return null;
    }

}