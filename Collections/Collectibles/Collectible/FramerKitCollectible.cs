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
        return ExcelRow.Description.ToString().Split("<br>").First();
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

    // Framer Kits are weird. Every kit (minus one) has an associated BannerBg, but their only connection is through BannerCondition.
    // private (BannerBg, BannerFrame?, BannerDecoration?, CharaCardBase?, CharaCardDecoration?, CharaCardHeader?) GetUnlockedAssets()
    // {
        
    // }
}