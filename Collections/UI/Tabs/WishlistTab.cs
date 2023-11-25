namespace Collections;

public class WishlistTab : IDrawable
{
    private Dictionary<string, List<ICollectible>> collections = new();

    private EventService EventService { get; init; }
    private CollectionWidget CollectionWidget { get; init; }
    public WishlistTab()
    {
        EventService = new EventService();
        CollectionWidget = new CollectionWidget(EventService, false);
        LoadCollectibles();

        EventService.Subscribe<FilterChangeEvent, FilterChangeEventArgs>(OnPublish);
    }

    public void Draw()
    {
        var isEmpty = true;
        foreach (var (name, collection) in collections)
        {
            if (collection.Any())
            {
                ImGui.Selectable(name);
                CollectionWidget.Draw(collection, false, false);
                isEmpty = false;
            }
        }

        if (isEmpty)
        {
            ImGui.Text("No items in Wish List");
        }
    }

    private void LoadCollectibles()
    {
        Dev.Log("Loading wish list collectibles");
        collections = Services.DataProvider.GetCollections();
        foreach (var (name, collection) in collections)
        {
            collections[name] = collection.Where(c => c.IsWishlist()).ToList();
        }
    }

    public void OnPublish(FilterChangeEventArgs args)
    {
        LoadCollectibles();
    }

    public void OnOpen()
    {
        Dev.Log();
        LoadCollectibles();
    }

    public void OnClose()
    {
    }

    public void Dispose()
    {
    }
}
