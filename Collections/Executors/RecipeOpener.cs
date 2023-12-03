using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace Collections;

public class RecipeOpener
{
    public static unsafe void OpenRecipeByItemId(uint itemId)
    {
        AgentRecipeNote.Instance()->OpenRecipeByItemId(itemId);
    }

    public static unsafe void OpenRecipeByRecipeId(uint recipeId)
    {
        AgentRecipeNote.Instance()->OpenRecipeByRecipeId(recipeId);
    }
}

