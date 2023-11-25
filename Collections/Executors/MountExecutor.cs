using FFXIVClientStructs.FFXIV.Client.Game;

namespace Collections;

public class SummonActionExecutor
{
    public static void SummonMount(uint mountId)
    {
        UseActionManagerAction(ActionType.Mount, mountId);
    }

    public static void SummonMinion(uint minionId)
    {
        UseActionManagerAction(ActionType.Companion, minionId);
    }

    private static unsafe void UseActionManagerAction(ActionType actionType, uint actionId)
    {
        ActionManager.Instance()->UseAction(actionType, actionId);
    }
}
