using Dalamud.Game.Command;

namespace Collections;

public class CommandsInitializer
{
    public static string mainWindowCmd => "/collections";
    public static string mainWindowSecondaryCmd => "/co";
    public static string mainWindowCmdMessage => "Open the Collections window";

    public CommandsInitializer()
    {
        RegisterCommands();
    }

    public void RegisterCommands()
    {
        Services.CommandManager.AddHandler(mainWindowSecondaryCmd, new CommandInfo(OpenMainWindow)
        {
            HelpMessage = mainWindowCmdMessage
        });
        Services.CommandManager.AddHandler(mainWindowCmd, new CommandInfo(OpenMainWindow)
        {
            HelpMessage = mainWindowCmdMessage
        });
    }

    private void OpenMainWindow(string command, string args)
    {
        Services.WindowsInitializer.MainWindow.IsOpen = true;
    }

    public void Dispose(ICommandManager CommandManager)
    {
        CommandManager.RemoveHandler(mainWindowCmd);
        CommandManager.RemoveHandler(mainWindowSecondaryCmd);
    }
}
