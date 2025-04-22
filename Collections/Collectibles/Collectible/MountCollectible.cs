using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace Collections;

public class MountCollectible : Collectible<Mount>, ICreateable<MountCollectible, Mount>
{
    public new static string CollectionName => "Mounts";

    public MountCollectible(Mount excelRow) : base(excelRow)
    {
        SortOptions.Add(new CollectibleSortOption("Seats", Comparer<ICollectible>.Create((c1, c2) => ((MountCollectible)c1).ExcelRow.ExtraSeats.CompareTo(((MountCollectible)c2).ExcelRow.ExtraSeats)), false, null));
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

    protected override HintModule GetPrimaryHint()
    {
        return new HintModule((ExcelRow.ExtraSeats + 1).ToString(), FontAwesomeIcon.PeopleGroup);
    }

    protected override HintModule GetSecondaryHint()
    {
        return new HintModule("", null);
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
