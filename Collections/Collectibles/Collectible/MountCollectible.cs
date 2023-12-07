using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace Collections;

public class MountCollectible : Collectible<Mount>, ICreateable<MountCollectible, Mount>
{
    public new static string CollectionName => "Mounts";

    public MountCollectible(Mount excelRow) : base(excelRow)
    {
    }

    public static MountCollectible Create(Mount excelRow)
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

    public override unsafe void UpdateObtainedState()
    {
        isObtained = PlayerState.Instance()->IsMountUnlocked(ExcelRow.RowId);
    }

    protected override int GetIconId()
    {
        return ExcelRow.Icon;
    }

    public override unsafe void Interact()
    {
        if (isObtained)
            ActionManager.Instance()->UseAction(ActionType.Mount, ExcelRow.RowId);
    }
}
