#if UNITY_2019_1_OR_NEWER
using UnityEngine;

public class UnityConsoleUI : AscalonUIModule
{
    public override void Log(string argTitle, string argContent, LogMode argType)
    {
        UnityConsoleUIProxy.instance.Log(argTitle, argContent, argType);
    }

    public override void ClearEntries()
    {
        UnityConsoleUIProxy.instance.ClearEntries();
    }
}
#endif
