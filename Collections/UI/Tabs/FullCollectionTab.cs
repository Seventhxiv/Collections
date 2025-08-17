
namespace Collections;

public class FullCollectionTab : IDrawable
{
    const int ContentFiltersWidth = 17;

    private List<ICollectible> collections = new();
    private ContentFiltersWidget ContentFiltersWidget { get; init; }
    private CollectionWidget CollectionWidget { get; init; }
    private EventService EventService { get; init; }
    public FullCollectionTab()
    {
        EventService = new EventService();
        CollectionWidget = new CollectionWidget(EventService, false, true);
        collections = Services.DataProvider.GetCollections().Values.Aggregate((full, current) => [.. full, .. current]);
        EventService.Subscribe<FilterChangeEvent, FilterChangeEventArgs>(OnPublish);
    }

    public void Draw()
    {
        // Draw collections
        DrawCollections();
    }

    public void DrawCollections()
    {
        CollectionWidget.Draw(collections, enableFilters: true);
    }
    public void OnPublish(FilterChangeEventArgs args)
    {
        
    }

    public void OnOpen()
    {
        Dev.Log();

        Task.Run(() =>
        {
            foreach (var collectible in collections)
            {
                collectible.UpdateObtainedState();
            }
        });
    }

    public void OnClose()
    {
    }

    public void Dispose()
    {
    }
}

