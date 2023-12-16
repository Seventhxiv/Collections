using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace Collections;

public class HairstyleCollectible : Collectible<CharaMakeCustomize>, ICreateable<HairstyleCollectible, CharaMakeCustomize>
{
    public new static string CollectionName => "Hairstyles";

    public HairstyleCollectible(CharaMakeCustomize excelRow) : base(excelRow)
    {
    }

    public static HairstyleCollectible Create(CharaMakeCustomize excelRow)
    {
        return new(excelRow);
    }

    protected override string GetCollectionName()
    {
        return CollectionName;
    }

    protected override string GetName()
    {
        if (CollectibleKey is ItemKey)
        {
            var item = ExcelRow.HintItem.Value;
            return item.Name.ToString().RemovePrefix("Modern Cosmetics - ").RemovePrefix("Modern Aesthetics - "); // TODO only works in English
        }
        else if (ExcelRow.RowId == 23) // Ceremony hairstyle
        {
            return "Eternal Bonding";
        }
        return "";
    }

    protected override uint GetId()
    {
        return ExcelRow.Data;
    }

    protected override string GetPrimaryDescription()
    {
        return "";
    }

    protected override string GetSecondaryDescription()
    {
        return "";
    }

    public override unsafe void UpdateObtainedState()
    {
        isObtained = UIState.Instance()->IsUnlockLinkUnlocked(ExcelRow.Data);
    }

    protected override int GetIconId()
    {
        return (int)ExcelRow.Icon;
    }

    public override void Interact()
    {
        // Do nothing
    }
}
