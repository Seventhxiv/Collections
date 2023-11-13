using Dalamud.Logging;
using Dalamud.Plugin.Services;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Collections;

public class Dev
{
    public static void Log(string suffix = null, int frames = 1, [CallerMemberName] string caller = "", [CallerFilePath] string file = "")
    {
        var lastIndexPathSymbol = file.LastIndexOf("\\");
        if (lastIndexPathSymbol == -1)
        {
            lastIndexPathSymbol = file.LastIndexOf("/");
        }
        var callingClass = file.Substring(lastIndexPathSymbol + 1);

        var logMessage = callingClass + "::" + caller;

        if (suffix != null)
        {
            logMessage += $": {suffix}";
        }

        Services.PluginLog.Debug(logMessage);
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
        //var timeTaken = Stopwatch.Elapsed.TotalMilliseconds*1000;
        Log(prefix + "Time Taken: " + timeTaken + "ms", 2, caller, file);
    }

    // Stopwatch
    private static Dictionary<string, Stopwatch> StopwatchStorage = new();
    public static void Start([CallerMemberName] string caller = "", [CallerFilePath] string file = "")
    {
        var sp = GetStopwatch(caller, file);
        sp.Restart();
    }

    public static double Stop(bool log = true, string prefix = "", [CallerMemberName] string caller = "", [CallerFilePath] string file = "")
    {
        var sp = GetStopwatch(caller, file);
        sp.Stop();
        var timeTaken = Stopwatch.ElapsedMilliseconds;
        //var timeTaken = sp.Elapsed.TotalMilliseconds * 1000;
        if (log)
            Log(prefix + "Time Taken: " + timeTaken + "ms", 2, caller, file);
        return timeTaken;
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
