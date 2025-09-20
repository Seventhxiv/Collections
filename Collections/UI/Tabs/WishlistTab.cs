namespace Collections;

public class WishlistTab : IDrawable
{
    private List<ICollectible> collections = new();

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
        if (collections.Count == 0)
        {
            ImGui.Text("No items in Wish List");
        }
        else
        {
            CollectionWidget.Draw(collections, enableFilters: false, enableCollectionHeaders: true);
        }
    }

    private void LoadCollectibles()
    {
        collections = Services.DataProvider.GetCollections().Values.Aggregate((full, c) => [..full, ..c]).Where(c => c.IsWishlist()).ToList();
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
