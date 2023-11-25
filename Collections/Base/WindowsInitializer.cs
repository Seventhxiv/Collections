using Dalamud.Interface.Windowing;

namespace Collections;

public class WindowsInitializer
{

    public WindowSystem WindowSystem { get; init; }
    //public InspectWindow InspectWindow { get; init; }
    public MainWindow MainWindow { get; init; }

    public WindowsInitializer()
    {
        // Attach windows
        MainWindow = new MainWindow();
        WindowSystem = new WindowSystem(Services.Plugin.NameSpace);
        //InspectWindow = new InspectWindow();
        WindowSystem.AddWindow(MainWindow);
        //WindowSystem.AddWindow(InspectWindow);
        //InspectWindow.IsOpen = true;

        // Attach draw functions
        Services.PluginInterface.UiBuilder.Draw += DrawUI;
        Services.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
    }

    public void DrawUI()
    {
        WindowSystem.Draw();
    }

    public void DrawConfigUI()
    {
        MainWindow.IsOpen = true;
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();
        MainWindow.Dispose();
    }
}
