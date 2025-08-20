namespace Collections;

public class WishlistTab : IDrawable
{
    private Dictionary<string, List<ICollectible>> collections = new();

    private EventService EventService { get; init; }
    private CollectionWidget CollectionWidget { get; init; }
    public WishlistTab()
    {
        EventService = new EventService();
        CollectionWidget = new CollectionWidget(EventService, false, true);
        LoadCollectibles();

        EventService.Subscribe<FilterChangeEvent, FilterChangeEventArgs>(OnPublish);
    }

    public void Draw()
    {
        List<ICollectible> merged = [];
        foreach (var (name, collection) in collections) {
            if (collection.Any())
            {
                merged.AddRange(collection);
            }
        }
        if (merged.Count == 0)
        {
            ImGui.Text("No items in Wish List");
        }
        else
        {
            CollectionWidget.Draw(merged, enableFilters: false, enableCollectionHeaders: true);
        }
    }

    private void LoadCollectibles()
    {
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
