using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace Collections;

public class BlueMageCollectible : Collectible<Lumina.Excel.GeneratedSheets.Action>, ICreateable<BlueMageCollectible, Lumina.Excel.GeneratedSheets.Action>
{
    public new static string CollectionName => "Blue Mage";

    public BlueMageCollectible(Lumina.Excel.GeneratedSheets.Action excelRow) : base(excelRow)
    {
    }

    public static BlueMageCollectible Create(Lumina.Excel.GeneratedSheets.Action excelRow)
    {
        return new(excelRow);
    }

    protected override string GetCollectionName()
    {
        return CollectionName;
    }

    protected override string GetName()
    {
        return ExcelRow.Name;
    }

    protected override uint GetId()
    {
        return ExcelRow.RowId;
    }

    protected override string GetPrimaryDescription()
    {
        return $"Cast: {ExcelRow.Cast100ms / 100f}s\nRecast: {ExcelRow.Recast100ms / 100f}s";
    }

    protected override string GetSecondaryDescription()
    {
        return ExcelCache<ActionTransient>.GetSheet().GetRow(ExcelRow.RowId).Description.ToString();
    }

    public override unsafe void UpdateObtainedState()
    {
        isObtained = UIState.Instance()->IsUnlockLinkUnlocked(ExcelRow.UnlockLink);
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
