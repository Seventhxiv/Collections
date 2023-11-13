using Lumina.Excel.GeneratedSheets;

namespace Collections;

public class DataGenerator
{
    public ShopsDataGenerator ShopsDataGenerator { get; private set; }
    public InstancesDataGenerator InstancesDataGenerator { get; private set; }
    public NpcLocationDataGenerator NpcLocationDataGenerator { get; private set; }
    public CollectibleKeyDataGenerator CollectibleKeyDataGenerator { get; private set; }
    public EventDataGenerator EventDataGenerator { get; private set; }
    public MogStationDataGenerator MogStationDataGenerator { get; private set; }
    public ContainersDataGenerator ContainersDataGenerator { get; private set; }
    public AchievementsDataGenerator AchievementsDataGenerator { get; private set; }
    public BeastTribesDataGenerator BeastTribesDataGenerator { get; private set; }
    public QuestsDataGenerator QuestsDataGenerator { get; private set; }

    public DataGenerator()
    {
        Dev.Start();
        ShopsDataGenerator = new ShopsDataGenerator();
        NpcLocationDataGenerator = new NpcLocationDataGenerator();
        InstancesDataGenerator = new InstancesDataGenerator();
        EventDataGenerator = new EventDataGenerator();
        BeastTribesDataGenerator = new BeastTribesDataGenerator();
        MogStationDataGenerator = new MogStationDataGenerator();
        QuestsDataGenerator = new QuestsDataGenerator();
        CollectibleKeyDataGenerator = new CollectibleKeyDataGenerator();
        ContainersDataGenerator = new ContainersDataGenerator();
        AchievementsDataGenerator = new AchievementsDataGenerator();
        Dev.Stop();
    }
}

