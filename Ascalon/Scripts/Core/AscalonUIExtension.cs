using System;

#if UNITY_2019_1_OR_NEWER
using UnityEngine;
#endif

#if GODOT
using Godot;
#endif

/// <summary>
/// Extends the Ascalon core to provide logging functions.
/// This is distinct from the UI Module: the Module provides the basic functionality for logging and UI,
/// while this is an extension of the core with the primary purposes of providing global static logging
/// methods and environment-native logging support.
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

        //get current verbosity
        bool verbose = (bool)Ascalon.GetConVar("con_verbose");

    #if !(UNITY_2019_1_OR_NEWER || GODOT) //default handling (not an explicitly supported environment)
        Console.WriteLine(argContent);
    #else //environment-specific handling
        if (argType == LogMode.Info || (argType == LogMode.InfoVerbose && verbose == true))
        {
        #if UNITY_2019_1_OR_NEWER
            Debug.Log(argContent);
        #elif GODOT
            GD.Print(argContent);
        #endif
        }
        else if (argType == LogMode.Warning || (argType == LogMode.WarningVerbose && verbose == true))
        {
        #if UNITY_2019_1_OR_NEWER
            Debug.LogWarning(argContent);
        #elif GODOT
            GD.PushWarning(argContent);
        #endif
        }
        else if (argType == LogMode.Error || (argType == LogMode.ErrorVerbose && verbose == true))
        {
        #if UNITY_2019_1_OR_NEWER
            Debug.LogError(argContent);
        #elif GODOT
            GD.PrintErr(argContent);
        #endif
        }
        else if (argType == LogMode.Exception || (argType == LogMode.ExceptionVerbose && verbose == true))
        {
        #if UNITY_2019_1_OR_NEWER
            Debug.LogError(argContent);
        #elif GODOT
            GD.PrintErr(argContent);
        #endif
        }
        else if (argType == LogMode.Assertion || (argType == LogMode.AssertionVerbose && verbose == true))
        {
        #if UNITY_2019_1_OR_NEWER
            Debug.LogError(argContent);
        #elif GODOT
            GD.PrintErr(argContent);
        #endif
        }
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