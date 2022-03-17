using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugFeedUIProxy : MonoBehaviour
{
    private void Start()
    {
        Ascalon.instance.onCallCompleted += Ascalon.instance.uiModule.OnCallComplete;
    }
    public void ClearEntries()
    {
        Ascalon.instance.uiModule.ClearEntries();
    }

    public void UpdateSuggestions()
    {
        (Ascalon.instance.uiModule as DebugFeed).UpdateSuggestions();
    }
}
