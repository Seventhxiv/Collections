using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace Collections;

public class FramerKitCollectible: Collectible<ItemAdapter>, ICreateable<FramerKitCollectible, ItemAdapter>
{
    public new static string CollectionName => "Framer Kits";

    public FramerKitCollectible(ItemAdapter excelRow) : base(excelRow)
    {
    }

    public static FramerKitCollectible Create(ItemAdapter excelRow)
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
        isObtained = PlayerState.Instance()->IsFramersKitUnlocked(ExcelRow.AdditionalData.RowId);
    }

    protected override int GetIconId()
    {
        return ExcelRow.Icon;
    }

    public override unsafe void Interact()
    {
        // Do nothing
    }

}