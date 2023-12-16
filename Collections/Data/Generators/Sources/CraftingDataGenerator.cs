namespace Collections;

public class CraftingDataGenerator : BaseDataGenerator<uint>
{
    protected override void InitializeData()
    {
        var recipes = ExcelCache<Recipe>.GetSheet();
        foreach (var recipe in recipes)
        {
            var itemId = recipe.ItemResult.Row;
            AddEntry(itemId, recipe.RowId);
        }
    }
}
