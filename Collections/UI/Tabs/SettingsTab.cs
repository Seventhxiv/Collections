namespace Collections;

public class SettingsTab : IDrawable
{
    public SettingsTab()
    {
        autoOpenInstanceTab = Services.Configuration.AutoOpenInstanceTab;
    }

    //public Dictionary<string, bool> settings = new()
    //{
    //    { "Auto open Instance tab when entering an instance", true },
    //};

    private bool autoOpenInstanceTab;
    public void Draw()
    {
        if (ImGui.Checkbox("Auto open Instance tab when entering an instance", ref autoOpenInstanceTab))
        {
            Services.Configuration.AutoOpenInstanceTab = autoOpenInstanceTab;
            Services.Configuration.Save();
        }
    }

    public void OnOpen()
    {
        Dev.Log();
    }

    public void OnClose()
    {
    }

    public void Dispose()
    {
    }
}
