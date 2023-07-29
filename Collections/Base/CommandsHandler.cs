using Dalamud.Game.Command;

namespace Collections;

public class CommandsHandler
{
    public static string mainWindowCmd => "/collections";
    public static string mainWindowSecondaryCmd => "/co";
    public static string mainWindowCmdMessage => "Open the Collections window";

    private static string instanceWindowCmd => "/coi";
    private static string instanceWindowCmdMessage => "Open the Instance Collections window";

    public CommandsHandler()
    {
        registerCommand();
    }

    public void registerCommand()
    {
        Services.CommandManager.AddHandler(mainWindowSecondaryCmd, new CommandInfo(OpenMainWindow)
        {
            HelpMessage = mainWindowCmdMessage
        });
        Services.CommandManager.AddHandler(mainWindowCmd, new CommandInfo(OpenMainWindow)
        {
            HelpMessage = mainWindowCmdMessage
        });
        Services.CommandManager.AddHandler(instanceWindowCmd, new CommandInfo(OpenInstanceWindow)
        {
            HelpMessage = instanceWindowCmdMessage
        });
    }

    private void OpenMainWindow(string command, string args)
    {
        Services.WindowsHandler.MainWindow.IsOpen = true;
    }

    private void OpenInstanceWindow(string command, string args)
    {
        Services.WindowsHandler.InstanceWindow.IsOpen = true;
    }

    public static void RemoveHandlers(CommandManager CommandManager)
    {
        CommandManager.RemoveHandler(mainWindowCmd);
        CommandManager.RemoveHandler(mainWindowSecondaryCmd);
        CommandManager.RemoveHandler(instanceWindowCmd);
    }
}
