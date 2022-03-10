using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class which will save all ConVars flaged with Save flag to config on app exit
public class SaveConfigModule : MonoBehaviour
{
    private void OnApplicationQuit()
    {
        DebugConfigTools.WriteConfigUnity("config");
    }
}
