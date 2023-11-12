using Dalamud.Interface.Internal;
using ImGuiScene;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;

namespace Collections.Types;

public class GlamourSet
{
    public Dictionary<EquipSlot, GlamourItem?> set = new();
    public string name;

    public class GlamourItem
    {
        public Item item;
        public IDalamudTextureWrap icon;
        public StainEntity stain;
    }
}
