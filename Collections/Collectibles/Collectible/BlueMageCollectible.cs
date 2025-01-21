using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace Collections;

public class BlueMageCollectible : Collectible<Lumina.Excel.Sheets.Action>, ICreateable<BlueMageCollectible, Lumina.Excel.Sheets.Action>
{
    public new static string CollectionName => "Blue Mage";

    public BlueMageCollectible(Lumina.Excel.Sheets.Action excelRow) : base(excelRow)
    {
    }

    public static BlueMageCollectible Create(Lumina.Excel.Sheets.Action excelRow)
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
        return ExcelCache<ActionTransient>.GetSheet().GetRow(ExcelRow.RowId)?.Description.ToString() ?? "Could not find description";
    }

    protected override HintModule GetPrimaryHint()
    {
        return new HintModule($"Cast: {ExcelRow.Cast100ms / 100f}s", null);
    }

    protected override HintModule GetSecondaryHint()
    {
        return new HintModule($"Recast: {ExcelRow.Recast100ms / 100f}s", null);
    }

    public override unsafe void UpdateObtainedState()
    {
        isObtained = UIState.Instance()->IsUnlockLinkUnlocked(ExcelRow.UnlockLink.RowId);
    }

    protected override int GetIconId()
    {
        return ExcelRow.Icon;
    }

    public override void Interact()
    {
        // Do nothing
    }
}
