#if GODOT
using System;

public class GodotConsoleUI : AscalonUIModule
{
    public override void Log(string argTitle, string argContent, LogMode argType)
    {
        GodotConsoleUIProxy.instance.Log(argTitle, argContent, argType);
    }

    public override void ClearEntries()
    {
        GodotConsoleUIProxy.instance.ClearEntries();
    }
}
#endif
