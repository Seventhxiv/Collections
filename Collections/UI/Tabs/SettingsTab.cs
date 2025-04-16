namespace Collections;

public class SettingsTab : IDrawable
{
    public SettingsTab()
    {
        autoOpenInstanceTab = Services.Configuration.AutoOpenInstanceTab;
        autoHideObtainedFromInstanceTab = Services.Configuration.AutoHideObtainedFromInstanceTab;
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
        if (ImGui.Checkbox("Auto hide obtained items from Instance tab", ref autoHideObtainedFromInstanceTab))
        {
            Services.Configuration.HideObtainedFromInstanceTab = hideObtainedFromInstanceTab;
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
