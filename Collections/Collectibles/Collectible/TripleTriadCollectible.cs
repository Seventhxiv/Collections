using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace Collections;

public class TripleTriadCollectible : Collectible<ItemAdapter>, ICreateable<TripleTriadCollectible, ItemAdapter>
{
    public new static string CollectionName => "Triple Triad";

    public TripleTriadCollectible(ItemAdapter excelRow) : base(excelRow)
    {
    }

    public static TripleTriadCollectible Create(ItemAdapter excelRow)
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
    
    protected override HintModule GetSecondaryHint()
    {
        return new HintModule($"Card No. {ExcelRow.Description.ToString().Split("Card No. ").Last()}", null);
    }

    public override unsafe void UpdateObtainedState()
    {
        isObtained = UIState.Instance()->IsTripleTriadCardUnlocked((ushort)GetCollectibleFromUnlock().RowId);
    }

    protected override int GetIconId()
    {
        return (int)GetCollectibleFromUnlock().RowId + 87000;
    }

    public override unsafe void Interact()
    {
        // Do nothing
    }

    public override void OpenGamerEscape()
    {
        WikiOpener.OpenGamerEscape(GetDisplayName());
    }

    public TripleTriadCard GetCollectibleFromUnlock()
    {
        return ExcelCache<TripleTriadCard>.GetSheet().GetRow(ExcelRow.ItemAction.Value.Data.ElementAt(0)).Value;
    }
}
