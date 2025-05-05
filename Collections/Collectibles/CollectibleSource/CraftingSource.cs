namespace Collections;

public class CraftingSource : CollectibleSource
{
    private uint recipeId { get; init; }
    public CraftingSource(uint recipeId)
    {
        this.recipeId = recipeId;
    }

    public override string GetName()
    {
        return "Craftable";
    }

    private readonly List<SourceCategory> sourceType = new() { SourceCategory.Crafting };
    public override List<SourceCategory> GetSourceCategories()
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

    public override CraftingSource Clone()
    {
        return new CraftingSource(recipeId);
    }
}
