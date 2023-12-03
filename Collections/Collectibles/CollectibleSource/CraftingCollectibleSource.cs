namespace Collections;

public class CraftingCollectibleSource : CollectibleSource
{
    private uint recipeId { get; init; }
    public CraftingCollectibleSource(uint recipeId)
    {
        this.recipeId = recipeId;
    }

    public override string GetName()
    {
        return "Craftable";
    }

    private readonly List<CollectibleSourceCategory> sourceType = new() { CollectibleSourceCategory.Crafting };
    public override List<CollectibleSourceCategory> GetSourceCategories()
    {
        return sourceType;
    }

    public override bool GetIslocatable()
    {
        return true;
    }

    public override void DisplayLocation()
    {
        try
        {
            RecipeOpener.OpenRecipeByRecipeId(recipeId);
        }
        catch (Exception e)
        {
            Dev.Log(e.ToString());
        }
    }

    public static int iconId = 62202;
    protected override int GetIconId()
    {
        return iconId;
    }
}
