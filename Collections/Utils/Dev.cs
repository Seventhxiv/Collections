using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Collections;

public class Dev
{
    public static void Log(string message = null, int frames = 1, [CallerMemberName] string caller = "", [CallerFilePath] string file = "")
    {
        var logMessage = "";
        if (frames == 1)
        {
            var callingFile = StripDirectoryPath(file);
            logMessage = callingFile + "->" + caller;
        }
        else
        {
            var frame = new StackFrame(frames, true);
            var callingFile = StripDirectoryPath(frame.GetFileName());
            logMessage = callingFile + "->" + frame.GetMethod().Name;
        }

        if (message != null)
        {
            logMessage += $": {message}";
        }

        Services.PluginLog.Information(logMessage);
    }

    public static string StripDirectoryPath(string file)
    {
        var lastIndexPathSymbol = file.LastIndexOf("\\");
        if (lastIndexPathSymbol == -1)
        {
            lastIndexPathSymbol = file.LastIndexOf("/");
        }
        return file.Substring(lastIndexPathSymbol + 1).RemoveSuffix(".cs");
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
        var timeTaken = sp.ElapsedMilliseconds;
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
