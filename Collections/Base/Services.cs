using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Game.Gui.Toast;
using Dalamud.Game.Libc;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace Collections;

public class Services
{
    [PluginService] public static DalamudPluginInterface PluginInterface { get; private set; }
    [PluginService] public static IChatGui ChatGui { get; private set; }
    [PluginService] public static IClientState ClientState { get; private set; }
    [PluginService] public static ICommandManager CommandManager { get; private set; }
    [PluginService] public static ICondition Condition { get; private set; }
    [PluginService] public static IDataManager DataManager { get; private set; }
    [PluginService] public static IFramework Framework { get; private set; }
    [PluginService] public static IGameGui GameGui { get; private set; }
    [PluginService] public static IKeyState KeyState { get; private set; }
    [PluginService] public static ILibcFunction LibcFunction { get; private set; }
    [PluginService] public static IObjectTable ObjectTable { get; private set; }
    [PluginService] public static ISigScanner SigScanner { get; private set; }
    [PluginService] public static ITargetManager TargetManager { get; private set; }
    [PluginService] public static IToastGui ToastGui { get; private set; }
    //[PluginService] public static IDragDropManager DragDropManager { get; private set; }

    public static Plugin Plugin { get; private set; }
    public static Configuration Configuration { get; private set; }
    public static CommandsHandler CommandsHandler { get; private set; }
    public static WindowsHandler WindowsHandler { get; private set; }

    public static DataGenerator DataGenerator { get; private set; }

    public static GameActionsExecutor GameFunctionsExecutor { get; private set; }
    public static ContentTypeResolver ContentTypeResolver { get; private set; }
    public static GlamourDresserManager GlamourDresserManager { get; private set; }
    public static UniversalisClient UniversalisClient { get; private set; }

    public static DataProvider DataProvider { get; private set; }
    public static ItemManager ItemManager { get; private set; }
    public static LodestoneClient LodestoneClient { get; private set; }

    public Services(Plugin plugin)
    {
        Plugin = plugin;

        // Init plugin stuff
        CommandsHandler = new CommandsHandler();
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        // Generic stuff
        GameFunctionsExecutor = new GameActionsExecutor();
        ContentTypeResolver = new ContentTypeResolver();
        GlamourDresserManager = new GlamourDresserManager();
        UniversalisClient = new UniversalisClient();

        // Data generators
        DataGenerator = new DataGenerator();

        // Data providers
        ItemManager = new ItemManager();
        DataProvider = new DataProvider();

        // Windows
        WindowsHandler = new WindowsHandler();

        LodestoneClient = new LodestoneClient();

        // Framework ticks
        Framework.Update += GlamourDresserManager.GlamDresserObserver.OnFrameworkTick;
        Framework.Update += WindowsHandler.InspectWindow.OnFrameworkTick;
        Framework.Update += WindowsHandler.InstanceWindow.OnFrameworkTick;
        Framework.Update += WindowsHandler.MainWindow.OnFrameworkTick;
    }

    public static void Dispose()
    {
        // Cmds
        CommandsHandler.RemoveHandlers(CommandManager);

        // Windows
        WindowsHandler.Dispose();

        // Framework ticks
        Framework.Update -= GlamourDresserManager.GlamDresserObserver.OnFrameworkTick;
        Framework.Update -= WindowsHandler.InspectWindow.OnFrameworkTick;
        Framework.Update -= WindowsHandler.InstanceWindow.OnFrameworkTick;
        Framework.Update -= WindowsHandler.MainWindow.OnFrameworkTick;
    }
}
