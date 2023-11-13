namespace Collections;

public class SettingsTab : IDrawable
{
    public SettingsTab()
    {
        autoOpenInstanceTab = Services.Configuration.autoOpenInstanceTab;
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
            Services.Configuration.autoOpenInstanceTab = autoOpenInstanceTab;
            Services.Configuration.Save();
        }
    }

    public void OnOpen()
    {
    }

    public void Dispose()
    {
    }
}
