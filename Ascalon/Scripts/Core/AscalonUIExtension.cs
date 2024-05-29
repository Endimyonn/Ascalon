using System;

#if GODOT
using Godot;
#endif

/// <summary>
/// Extends the Ascalon core to provide logging functions.
/// </summary>
public partial class Ascalon
{
    private static void ConLog(string argContent)
    {
#if GODOT
        GD.Print(argContent);
#else
        Console.WriteLine(argContent);
#endif
    }

    public static void Log(string argTitle, string argContent, LogMode argType)
    {
        if (instance.uiModule == null)
        {
            ConLog("FeedEntry > " + argTitle + "\nContent   > " + argContent + "\nType      > " + argType.ToString());
            return;
        }

        instance.uiModule.Log(argTitle, argContent, argType);
    }

    public static void Log(string argTitle, LogMode argType)
    {
        if (instance.uiModule == null)
        {
            ConLog("FeedEntry > " + argTitle + "\nType      > " + argType.ToString());
            return;
        }

        instance.uiModule.Log(argTitle, "", argType);
    }

    public static void Log(string argTitle, string argContent)
    {
        if (instance.uiModule == null)
        {
            ConLog("FeedEntry > " + argTitle + "\nContent   > " + argContent);
        }

        instance.uiModule.Log(argTitle, argContent, LogMode.Info);
    }

    public static void Log(string argTitle)
    {
        if (instance.uiModule == null)
        {
            ConLog("FeedEntry > " + argTitle);
            return;
        }

        instance.uiModule.Log(argTitle, "", LogMode.Info);
    }

    [ConCommand("con_clear", "Clear the console of entries")]
    static void cmd_con_clear()
    {
        if (Ascalon.instance.uiModule != null)
        {
            Ascalon.instance.uiModule.ClearEntries();
        }
    }
}