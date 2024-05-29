#if UNITY_2019_1_OR_NEWER
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class creates and manages an Ascalon instance in the Unity
//environment. In most cases, this does not need to be modified.
public class AscalonUnity : MonoBehaviour
{
    //singleton
    [HideInInspector] public static AscalonUnity instance;


    //core
    private static Ascalon ascalonInstance;


    [Header("Settings")]
    public bool autoSaveOnQuit = true;
    public string mainConfigName = "config";
    public bool loadConfigUnityStyle = true;

    public GameObject uiFeedEntryPrefab;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        instance = this;
        DontDestroyOnLoad(transform.root.gameObject);

        ascalonInstance = new Ascalon();

        //for the time being I have been unable to figure out a solution
        //to allow non-Monobehaviour classes to be shown in the editor,
        //so they must be hardcoded here for now.
        ascalonInstance.uiModule = new DebugFeed();
        ascalonInstance.netModule = new AscalonEmptyNet();

        //settings
        ascalonInstance.mainConfigName = this.mainConfigName;
        ascalonInstance.loadConfigUnityStyle = this.loadConfigUnityStyle;
        ascalonInstance.loadConfigGodotStyle = false;

        ascalonInstance.Initialize();
    }

    private void Update()
    {
        if (ascalonInstance.uiModule != null)
        {
            ascalonInstance.uiModule.Update();
        }

        ascalonInstance.netModule.Update();
    }

    private void OnApplicationQuit()
    {
        if (autoSaveOnQuit == true)
        {
            if (ascalonInstance.loadConfigUnityStyle == true)
            {
                AscalonConfigTools.WriteConfigUnity("config");
            }
            else
            {
                AscalonConfigTools.WriteConfig("config");
            }
        }
    }
}
#endif