using Dalamud.Interface.Windowing;

namespace Collections;

public class WindowsHandler
{

    public WindowSystem WindowSystem { get; init; }
    public InspectWindow InspectWindow { get; init; }
    //public GlamourPlatesWidget GlamourPlatesWidget { get; init; }
    public MainWindow MainWindow { get; init; }
    public InstanceWindow InstanceWindow { get; init; }

    public WindowsHandler()
    {
        // Attach windows
        MainWindow = new MainWindow();
        WindowSystem = new WindowSystem(Services.Plugin.NameSpace);
        InspectWindow = new InspectWindow();
        //GlamourPlatesWidget = new GlamourPlatesWidget();
        InstanceWindow = new InstanceWindow();
        WindowSystem.AddWindow(MainWindow);
        WindowSystem.AddWindow(InspectWindow);
        WindowSystem.AddWindow(InstanceWindow);
        //InspectWindow.IsOpen = true;

        // Attach draw functions
        Services.PluginInterface.UiBuilder.Draw += DrawUI;
        Services.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
    }

    public void DrawUI()
    {
        WindowSystem.Draw();
        //GlamourPlatesWindow.DrawIfVisible();
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
