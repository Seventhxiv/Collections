namespace Collections;

public class DataGenerator
{
    public ShopsDataParser ShopsDataParser { get; private set; }
    public InstancesDataParser InstancesDataParser { get; private set; }
    public NpcLocationDataParser NpcLocationDataParser { get; private set; }
    public CollectibleUnlockItemDataParser CollectibleUnlockItemDataParser { get; private set; }
    public EventDataParser EventDataParser { get; private set; }
    public MogStationDataParser MogStationDataParser { get; private set; }
    public ContainersDataParser ContainersDataParser { get; private set; }
    public AchievementsDataParser AchievementsDataParser { get; private set; }
    public BeastTribesDataParser BeastTribesDataParser { get; private set; }
    public QuestsDataParser QuestsDataParser { get; private set; }

    public DataGenerator()
    {
        Dev.Start();
        ShopsDataParser = new ShopsDataParser();
        NpcLocationDataParser = new NpcLocationDataParser();
        InstancesDataParser = new InstancesDataParser();
        EventDataParser = new EventDataParser();
        BeastTribesDataParser = new BeastTribesDataParser();
        MogStationDataParser = new MogStationDataParser();
        QuestsDataParser = new QuestsDataParser();
        CollectibleUnlockItemDataParser = new CollectibleUnlockItemDataParser();
        ContainersDataParser = new ContainersDataParser();
        AchievementsDataParser = new AchievementsDataParser();
        Dev.Stop();
    }
}

