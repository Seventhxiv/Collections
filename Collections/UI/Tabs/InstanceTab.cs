namespace Collections;

public class InstanceTab : IDrawable
{
    private EventService EventService { get; init; }
    private CollectionWidget CollectionWidget { get; init; }
    //private CollectionWidget GlamCollectionWidget { get; init; }
    public InstanceTab()
    {
        EventService = new EventService();
        CollectionWidget = new CollectionWidget(EventService, false);
        //GlamCollectionWidget = new CollectionWidget(EventService, true);

        // Register to DutyStart event
        Services.DutyState.DutyStarted += OnDutyStarted;
    }

    public void OnDutyStarted(object sender, ushort arg)
    {
        Dev.Log("Got DutyStarted event");
        if (Services.Configuration.autoOpenInstanceTab)
        {
            Services.WindowsInitializer.MainWindow.forceInstanceTab = "Instance";
            Services.WindowsInitializer.MainWindow.IsOpen = true;
        }
    }

    private uint loadedInstance = 0;
    public void Draw()
    {
        // Not in valid instance
        if (!InValidInstance())
        {
            ImGui.Text("Please enter an instance to view this tab");
            loadedInstance = 0;
            return;
        }

        // Load instance collection if not done already
        if (GetCurrentInstance() != loadedInstance)
        {
            LoadCurrentInstanceCollection();
        }

        // Draw collection
        if (ImGui.BeginTable("instance-table", 1, ImGuiTableFlags.Borders | ImGuiTableFlags.NoHostExtendX | ImGuiTableFlags.SizingFixedFit))
        {
            ImGui.TableNextColumn();
            ImGui.TableHeader("Collectibles Obtained From This Instance");

            ImGui.TableNextRow();
            ImGui.TableNextColumn();
            CollectionWidget.Draw(collection, true);

            ImGui.EndTable();
        }
    }

    private static uint GetCurrentInstance()
    {
        var territoryType = Excel.GetExcelSheet<TerritoryType>().GetRow(Services.ClientState.TerritoryType);
        return territoryType.ContentFinderCondition.Row;
    }

    private static bool InValidInstance()
    {
        var instanceId = GetCurrentInstance();
        var inInstance = instanceId != 0;
        var collectionExists = Services.DataGenerator.InstancesDataGenerator.contentFinderConditionToItems.ContainsKey(instanceId);
        return inInstance && collectionExists;
    }

    private static List<uint>? CurrentDutyItemIds(uint instanceId)
    {

        if (Services.DataGenerator.InstancesDataGenerator.contentFinderConditionToItems.ContainsKey(instanceId))
        {
            return Services.DataGenerator.InstancesDataGenerator.contentFinderConditionToItems[instanceId];
        }
        Dev.Log($"Unable to find items obtained by the current duty id: {instanceId}");
        return new List<uint>();
    }

    private List<ICollectible> collection = new();
    public void LoadCurrentInstanceCollection()
    {
        loadedInstance = GetCurrentInstance();
        collection.Clear();

        var itemIds = CurrentDutyItemIds(loadedInstance);

        foreach (var item in itemIds)
        {
            var glamItem = Services.DataProvider.GlamourCollection.SelectMany(e => e.Value).Where(e => e.CollectibleKey.item.RowId == item).FirstOrDefault();
            if (glamItem != null)
            {
                glamItem.UpdateObtainedState();
                collection.Add(glamItem);
                continue;
            }

            var mountItem = Services.DataProvider.GetCollection<MountCollectible>().Where(e =>
            {
                if (e.CollectibleKey != null)
                    return e.CollectibleKey.item.RowId == item;
                else
                    return false;
            }
            ).FirstOrDefault();
            if (mountItem != null)
            {
                mountItem.UpdateObtainedState();
                collection.Add(mountItem);
                continue;
            }

            var minionItem = Services.DataProvider.GetCollection<MinionCollectible>().Where(e =>
            {
                if (e.CollectibleKey != null)
                    return e.CollectibleKey.item.RowId == item;
                else
                    return false;
            }
            ).FirstOrDefault();
            if (minionItem != null)
            {
                minionItem.UpdateObtainedState();
                collection.Add(minionItem);
                continue;
            }
        }
    }

    public void OnOpen()
    {
    }

    public void Dispose()
    {
        Services.DutyState.DutyStarted -= OnDutyStarted;
    }
}
