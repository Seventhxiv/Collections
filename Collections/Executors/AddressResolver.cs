using System.Runtime.InteropServices;
using Dalamud.Game;

namespace Collections;

public class AddressResolver  : BaseAddressResolver
{
    public delegate void setGlamourPlateSlotDelegate(IntPtr agent, PlateItemSource plateItemSource, int glamId, uint itemId, byte stainId);
    public readonly setGlamourPlateSlotDelegate setGlamourPlateSlot;
    protected nint setGlamourPlateSlotAddress { get; private set; }

    public AddressResolver()
    {
        Setup(Services.SigScanner);
        setGlamourPlateSlot = Marshal.GetDelegateForFunctionPointer<setGlamourPlateSlotDelegate>(setGlamourPlateSlotAddress);
    }

    protected override void Setup64Bit(ISigScanner SigScanner)
    {
        setGlamourPlateSlotAddress = SigScanner.ScanText("E8 ?? ?? ?? ?? E9 ?? ?? ?? ?? 48 8B 46 10 8B 1B");
    }
}
