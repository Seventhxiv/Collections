namespace Collections;

public class CraftingDataGenerator : BaseDataGenerator<uint>
{
    protected override void InitializeData()
    {
        var recipes = ExcelCache<Recipe>.GetSheet();
        foreach (var recipe in recipes)
        {
            var itemId = recipe.ItemResult.RowId;
            AddEntry(itemId, recipe.RowId);
        }
        var resourceData = CSVHandler.Load<ItemIdToSource>("OutfitsToCrafted.csv");
        foreach(var entry in resourceData)
        {
            // grab crafting link to item
            var recipeId = data[entry.SourceId];
            AddEntry(entry.ItemId, recipeId.First());
        }
    }
}
