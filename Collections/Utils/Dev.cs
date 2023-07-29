using Dalamud.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Collections;

public class Dev
{
    public static void Log(string suffix = null, int frames = 1, [CallerMemberName] string caller = "", [CallerFilePath] string file = "")
    {
        var callingClass = file.Substring(file.LastIndexOf("\\") + 1);
        var toLog = callingClass + "::" + caller;
        if (suffix != null)
        {
            toLog += $": {suffix}";
        }
        PluginLog.Debug(toLog);
    }

    // Old stopwatch
    private static Stopwatch Stopwatch = new();
    public static void StartStopwatch()
    {
        Stopwatch.Restart();
    }

    public static void EndStopwatch(string prefix = "", [CallerMemberName] string caller = "", [CallerFilePath] string file = "")
    {
        Stopwatch.Stop();
        var timeTaken = Stopwatch.ElapsedMilliseconds;
        Log(prefix + "Time Taken: " + timeTaken + "ms", 2, caller, file);
    }

    // Stopwatch
    private static Dictionary<string, Stopwatch> StopwatchStorage = new();
    public static void Start([CallerMemberName] string caller = "", [CallerFilePath] string file = "")
    {
        var sp = GetStopwatch(caller, file);
        sp.Restart();
    }

    public static void Stop(string prefix = "", [CallerMemberName] string caller = "", [CallerFilePath] string file = "")
    {
        var sp = GetStopwatch(caller, file);
        sp.Stop();
        var timeTaken = sp.ElapsedMilliseconds;
        Log(prefix + "Time Taken: " + timeTaken + "ms", 2, caller, file);
    }

    private static Stopwatch GetStopwatch(string caller, string file)
    {
        var key = caller + file;
        if (!StopwatchStorage.ContainsKey(key))
        {
            StopwatchStorage[key] = new Stopwatch();
        }
        return StopwatchStorage[key];
    }
}
