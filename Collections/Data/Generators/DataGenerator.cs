namespace Collections;

public class DataGenerator
{
    //public ShopsDataGenerator ShopsDataGenerator { get; private set; }
    //public InstancesDataGenerator InstancesDataGenerator { get; private set; }
    public NpcLocationDataGenerator NpcLocationDataGenerator { get; private set; }
    public KeysDataGenerator KeysDataGenerator { get; private set; }
    //public EventDataGenerator EventDataGenerator { get; private set; }
    //public MogStationDataGenerator MogStationDataGenerator { get; private set; }
    //public ContainersDataGenerator ContainersDataGenerator { get; private set; }
    //public AchievementsDataGenerator AchievementsDataGenerator { get; private set; }
    //public QuestsDataGenerator QuestsDataGenerator { get; private set; }
    //public CraftingDataGenerator CraftingDataGenerator { get; private set; }
    public CurrencyDataGenerator CurrencyDataGenerator { get; private set; }
    public SourcesDataGenerator SourcesDataGenerator { get; private set; }

    public DataGenerator()
    {
        Dev.Start();
        Task.Run(AsyncInitializeDataGenerators).Wait();
        Dev.Stop();
    }

    private async Task AsyncInitializeDataGenerators()
    {
        //var ShopsDataGeneratorTask = Task.Run(() => new ShopsDataGenerator());
        var NpcLocationDataGeneratorTask = Task.Run(() => new NpcLocationDataGenerator());
        //var InstancesDataGeneratorTask = Task.Run(() => new InstancesDataGenerator());
        //var EventDataGeneratorTask = Task.Run(() => new EventDataGenerator());
        //var MogStationDataGeneratorTask = Task.Run(() => new MogStationDataGenerator());
        //var QuestsDataGeneratorTask = Task.Run(() => new QuestsDataGenerator());
        var KeysDataGeneratorTask = Task.Run(() => new KeysDataGenerator());
        //var ContainersDataGeneratorTask = Task.Run(() => new ContainersDataGenerator());
        //var AchievementsDataGeneratorTask = Task.Run(() => new AchievementsDataGenerator());
        //var CraftingDataGeneratorTask = Task.Run(() => new CraftingDataGenerator());
        var CurrencyDataGeneratorTask = Task.Run(() => new CurrencyDataGenerator());
        var SourcesDataGeneratorTask = Task.Run(() => new SourcesDataGenerator());

        //ShopsDataGenerator = await ShopsDataGeneratorTask;
        NpcLocationDataGenerator = await NpcLocationDataGeneratorTask;
        //InstancesDataGenerator = await InstancesDataGeneratorTask;
        //EventDataGenerator = await EventDataGeneratorTask;
        //MogStationDataGenerator = await MogStationDataGeneratorTask;
        //QuestsDataGenerator = await QuestsDataGeneratorTask;
        KeysDataGenerator = await KeysDataGeneratorTask;
        //ContainersDataGenerator = await ContainersDataGeneratorTask;
        //AchievementsDataGenerator = await AchievementsDataGeneratorTask;
        //CraftingDataGenerator = await CraftingDataGeneratorTask;
        CurrencyDataGenerator = await CurrencyDataGeneratorTask;
        SourcesDataGenerator = await SourcesDataGeneratorTask;
    }
}
