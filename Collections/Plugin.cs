using Dalamud.Plugin;

namespace Collections;

public sealed class Plugin : IDalamudPlugin
{
    public string Name => "Collections";
    public string NameSpace => "Collections";

    public Plugin(IDalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Services>(this);
    }

    public void Dispose()
    {
        Services.Dispose();
    }
}
