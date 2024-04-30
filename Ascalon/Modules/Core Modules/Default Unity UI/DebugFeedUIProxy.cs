using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugFeedUIProxy : MonoBehaviour
{
    [Header("References")]
    public Image infoFilterBackground;
    public Image warningFilterBackground;
    public Image errorFilterBackground;

    private void Start()
    {
        Ascalon.instance.onCallCompleted += Ascalon.instance.uiModule.OnCallComplete;
    }
    public void ClearEntries()
    {
        Ascalon.instance.uiModule.ClearEntries();
    }

    public void ToggleShowEntriesInfo()
    {
        DebugFeed instance = (DebugFeed)Ascalon.instance.uiModule;
        instance.ToggleInfoEntries();
        ToggleFilterBGColor(infoFilterBackground, instance.infoVisible);
    }

    public void ToggleShowEntriesWarning()
    {
        DebugFeed instance = (DebugFeed)Ascalon.instance.uiModule;
        instance.ToggleWarningEntries();
        ToggleFilterBGColor(warningFilterBackground, instance.warningVisible);
    }

    public void ToggleShowEntriesError()
    {
        DebugFeed instance = (DebugFeed)Ascalon.instance.uiModule;
        instance.ToggleErrorEntries();
        ToggleFilterBGColor(errorFilterBackground, instance.errorVisible);
    }

    private void ToggleFilterBGColor(Image argBG, bool argState)
    {
        if (argState == true)
        {
            argBG.color = new Color(1, 1, 1, 0.39215f);
        }
        else
        {
            argBG.color = new Color(0.26274f, 0.26274f, 0.26274f, 0.39215f);
        }
    }

    public void ScrollToBottom()
    {
        (Ascalon.instance.uiModule as DebugFeed).ScrollToBottom();
    }

    public void UpdateSuggestions()
    {
        (Ascalon.instance.uiModule as DebugFeed).UpdateSuggestions();
    }
}
