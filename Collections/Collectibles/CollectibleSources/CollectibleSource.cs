using ImGuiScene;
using System.Collections.Generic;

namespace Collections;

// Shop, Instance, Achievement, Quest, Event...
public abstract class CollectibleSource
{
    // Public interface
    public abstract string GetName();
    public abstract List<CollectibleSourceType> GetSourceType();
    public abstract bool GetIslocatable();
    public abstract LocationEntry GetLocationEntry();

    // Protected abstract methods
    protected abstract int GetIconId();
    protected IconHandler IconHandler { get; set; }

    public CollectibleSource()
    {
    }

    // Internal implementation
    public TextureWrap GetIconLazy()
    {
        if (IconHandler == null)
        {
            IconHandler = new IconHandler(GetIconId());
        }
        return IconHandler.GetIconLazy();
    }
}
