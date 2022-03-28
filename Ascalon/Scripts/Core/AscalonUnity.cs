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

    //modules to be used
    //[SerializeField] private AscalonUIModule uiModule;
    //[SerializeField] private AscalonNetModule netModule;
    public GameObject uiFeedEntryPrefab;

    //settings
    public string mainConfigName = "config";
    public bool loadConfigUnityStyle = true;

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
        ascalonInstance.netModule = new AscalonMirrorNet();

        //settings
        ascalonInstance.mainConfigName = mainConfigName;
        ascalonInstance.loadConfigUnityStyle = loadConfigUnityStyle;

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
}
