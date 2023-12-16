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
    public QuestsDataGenerator QuestsDataGenerator { get; private set; }
    public CraftingDataGenerator CraftingDataGenerator { get; private set; }
    public CurrencyDataGenerator CurrencyDataGenerator { get; private set; }
    public BlueMageDataGenerator BlueMageDataGenerator { get; private set; }

    public DataGenerator()
    {
        Dev.Start();
        Task.Run(AsyncInitializeDataGenerators).Wait();
        Dev.Stop();
    }

    private async Task AsyncInitializeDataGenerators()
    {
        var ShopsDataGeneratorTask = Task.Run(() => new ShopsDataGenerator());
        var NpcLocationDataGeneratorTask = Task.Run(() => new NpcLocationDataGenerator());
        var InstancesDataGeneratorTask = Task.Run(() => new InstancesDataGenerator());
        var EventDataGeneratorTask = Task.Run(() => new EventDataGenerator());
        var MogStationDataGeneratorTask = Task.Run(() => new MogStationDataGenerator());
        var QuestsDataGeneratorTask = Task.Run(() => new QuestsDataGenerator());
        var CollectibleKeyDataGeneratorTask = Task.Run(() => new CollectibleKeyDataGenerator());
        var ContainersDataGeneratorTask = Task.Run(() => new ContainersDataGenerator());
        var AchievementsDataGeneratorTask = Task.Run(() => new AchievementsDataGenerator());
        var CraftingDataGeneratorTask = Task.Run(() => new CraftingDataGenerator());
        var CurrencyDataGeneratorTask = Task.Run(() => new CurrencyDataGenerator());
        var BlueMageDataGeneratorTask = Task.Run(() => new BlueMageDataGenerator());

        ShopsDataGenerator = await ShopsDataGeneratorTask;
        NpcLocationDataGenerator = await NpcLocationDataGeneratorTask;
        InstancesDataGenerator = await InstancesDataGeneratorTask;
        EventDataGenerator = await EventDataGeneratorTask;
        MogStationDataGenerator = await MogStationDataGeneratorTask;
        QuestsDataGenerator = await QuestsDataGeneratorTask;
        CollectibleKeyDataGenerator = await CollectibleKeyDataGeneratorTask;
        ContainersDataGenerator = await ContainersDataGeneratorTask;
        AchievementsDataGenerator = await AchievementsDataGeneratorTask;
        CraftingDataGenerator = await CraftingDataGeneratorTask;
        CurrencyDataGenerator = await CurrencyDataGeneratorTask;
        BlueMageDataGenerator = await BlueMageDataGeneratorTask;
    }
}
