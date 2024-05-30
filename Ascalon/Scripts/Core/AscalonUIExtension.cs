using System;

#if GODOT
using Godot;
#endif

/// <summary>
/// Extends the Ascalon core to provide logging functions.
/// </summary>
public partial class Ascalon
{
    private static bool doNativeLog = true;
    private static bool nativeLogHeader = true;

    private static void NativeLog(string argContent, LogMode argType = LogMode.Info)
    {
        //use header?
        if (nativeLogHeader == true)
        {
            argContent = $"# Ascalon Log ({argType.ToString()}):\n" + argContent;
        }
        
#if UNITY_2019_1_OR_NEWER
        if (argType == LogMode.Info || argType == LogMode.InfoVerbose)
        {
            Debug.Log(argContent);
        }
        else if (argType == LogMode.Warning || argType == LogMode.WarningVerbose)
        {
            Debug.LogWarning(argContent);
        }
        else
        {
            Debug.LogError(argContent);
        }
#elif GODOT
        if (argType == LogMode.Info || argType == LogMode.InfoVerbose)
        {
            GD.Print(argContent);
        }
        else if (argType == LogMode.Warning || argType == LogMode.WarningVerbose)
        {
            GD.PushWarning(argContent);
        }
        else
        {
            GD.PrintErr(argContent);
        }
#else
        Console.WriteLine(argContent);
#endif
    }

    public static void Log(string argTitle, string argContent, LogMode argType)
    {
        if (doNativeLog == true)
        {
            NativeLog($"{argTitle}\n{argContent}", argType);
        }

        if (instance.uiModule != null)
        {
            instance.uiModule.Log(argTitle, argContent, argType);
        }
    }

    public static void Log(string argTitle, LogMode argType)
    {
        if (doNativeLog == true)
        {
            NativeLog(argTitle, argType);
        }

        if (instance.uiModule != null)
        {
            instance.uiModule.Log(argTitle, "", argType);
        }
    }

    public static void Log(string argTitle, string argContent)
    {
        if (doNativeLog == true)
        {
            NativeLog($"{argTitle}\n{argContent}");
        }

        if (instance.uiModule != null)
        {
            instance.uiModule.Log(argTitle, argContent, LogMode.Info);
        }
    }

    public static void Log(string argTitle)
    {
        if (doNativeLog == true)
        {
            NativeLog(argTitle);
        }

        if (instance.uiModule != null)
        {
            instance.uiModule.Log(argTitle, "", LogMode.Info);
        }
    }

    [ConVar("con_nativelog", "If enabled, logs will also output to the platform's native console, if applicable.", ConFlags.Save)]
    static ConVar cvar_con_nativelog = new ConVar(true)
    {
        DataChanged = (object oldData, object newData) =>
        {
            doNativeLog = (bool)newData;
        }
    };

    [ConVar("con_nativelog_header", "If enabled, native logs will include a header indicating they come from Ascalon, along with the type of log.", ConFlags.Save)]
    static ConVar cvar_con_nativelog_header = new ConVar(true)
    {
        DataChanged = (object oldData, object newData) =>
        {
            nativeLogHeader = (bool)newData;
        }
    };

    [ConCommand("con_clear", "Clear the console of entries")]
    static void cmd_con_clear()
    {
        if (Ascalon.instance.uiModule != null)
        {
            Ascalon.instance.uiModule.ClearEntries();
        }
    }
}