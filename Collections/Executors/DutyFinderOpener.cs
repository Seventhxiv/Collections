using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace Collections.Executors;

public unsafe class DutyFinderOpener
{
    private static AgentContentsFinder* ContentsFinderAgent = AgentContentsFinder.Instance();

    public static void OpenRegularDuty(uint dutyId)
    {
        ContentsFinderAgent->OpenRegularDuty(dutyId);
    }
}
