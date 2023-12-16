namespace Collections;

public class DataGenerator
{
    public NpcLocationDataGenerator NpcLocationDataGenerator { get; private set; }
    public CurrencyDataGenerator CurrencyDataGenerator { get; private set; }
    public KeysDataGenerator KeysDataGenerator { get; private set; }
    public SourcesDataGenerator SourcesDataGenerator { get; private set; }

    public DataGenerator()
    {
        Dev.Start();
        Task.Run(AsyncInitializeDataGenerators).Wait();
        Dev.Stop();
    }

    private async Task AsyncInitializeDataGenerators()
    {
        var NpcLocationDataGeneratorTask = Task.Run(() => new NpcLocationDataGenerator());
        var KeysDataGeneratorTask = Task.Run(() => new KeysDataGenerator());
        var CurrencyDataGeneratorTask = Task.Run(() => new CurrencyDataGenerator());
        var SourcesDataGeneratorTask = Task.Run(() => new SourcesDataGenerator());

        NpcLocationDataGenerator = await NpcLocationDataGeneratorTask;
        KeysDataGenerator = await KeysDataGeneratorTask;
        CurrencyDataGenerator = await CurrencyDataGeneratorTask;
        SourcesDataGenerator = await SourcesDataGeneratorTask;
    }
}
