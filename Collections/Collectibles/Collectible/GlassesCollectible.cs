using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Component.Excel;

namespace Collections;

public class GlassesCollectible: Collectible<Glasses>, ICreateable<GlassesCollectible, Glasses>
{
    public new static string CollectionName => "Glasses";

    public GlassesCollectible(Glasses excelRow) : base(excelRow)
    {
    }

    public static GlassesCollectible Create(Glasses excelRow)
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
        // trying to use the underlying unlock item for glasses, as the unlock description is cooler.
        if(CollectibleKey != null)
            return ExcelCache<ItemAdapter>.GetSheet().GetRow(CollectibleKey.Id).Value.Description.ToString();
        
        return ExcelRow.Description.ToString();
    }

    public override unsafe void UpdateObtainedState()
    {
        isObtained = PlayerState.Instance()->IsGlassesUnlocked((ushort)ExcelRow.RowId);
    }

    protected override int GetIconId()
    {
        return ExcelRow.Icon;
    }

    public override unsafe void Interact()
    {
        if(isObtained)
            ActionManager.Instance()->UseAction(ActionType.Unk_10, ExcelRow.RowId);
    }

    public override void OpenGamerEscape()
    {
        WikiOpener.OpenGamerEscape(GetDisplayName() + "_(Facewear)");
    }
}