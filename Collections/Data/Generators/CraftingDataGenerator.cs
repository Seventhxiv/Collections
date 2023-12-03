namespace Collections;

public class CraftingDataGenerator
{
    public Dictionary<uint, List<uint>> itemsToRecipes = new();

    public CraftingDataGenerator()
    {
        PopulateData();
    }

    private void PopulateData()
    {
        var recipes = ExcelCache<Recipe>.GetSheet();
        foreach (var recipe in recipes)
        {
            var itemId = recipe.ItemResult.Row;
            if (!itemsToRecipes.ContainsKey(itemId))
                itemsToRecipes[itemId] = new List<uint>();

            itemsToRecipes[itemId].Add(recipe.RowId);
        }
    }
}

