using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace Collections;

public class MountCollectible : Collectible<Mount>, ICreateable<MountCollectible, Mount>
{
    public static string CollectionName => "Mounts";

    public MountCollectible(Mount excelRow) : base(excelRow)
    {
        SortOptions.Add(new CollectibleSortOption("Seats", (c) => c is MountCollectible ? ((MountCollectible)c).ExcelRow.ExtraSeats : -1));
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
        return ExcelRow.Singular.ToString();
    }

    protected override uint GetId()
    {
        return ExcelRow.RowId;
    }

    protected override string GetDescription()
    {
        return ExcelCache<MountTransient>.GetSheet().GetRow(ExcelRow.RowId)?.Description.ToString() ?? "";
    }

    protected override HintModule GetSecondaryHint()
    {
        return new HintModule((ExcelRow.ExtraSeats + 1).ToString(), FontAwesomeIcon.PeopleGroup);
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

    public override void OpenGamerEscape()
    {
        WikiOpener.OpenGamerEscape(GetDisplayName() + "_(Mount)");
    }
}
