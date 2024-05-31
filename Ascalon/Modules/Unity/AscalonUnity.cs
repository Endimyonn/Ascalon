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


    [Header("Main Config Settings")]
    public bool autoLoadMainConfig = true;
    public string mainConfigName = "config";
    public bool autoSaveOnQuit = true;



    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    private static void Boot()
    {
        //load and instantiate runner
        GameObject loadRunner = Resources.Load<GameObject>("Ascalon Runner");
        GameObject createdRunner = GameObject.Instantiate(loadRunner);
        createdRunner.name = loadRunner.name;

        //load and instantiate UI
        GameObject loadUI = Resources.Load<GameObject>("Ascalon UI");
        GameObject createdUI = GameObject.Instantiate(loadUI);
        createdUI.name = loadUI.name;
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        instance = this;
        DontDestroyOnLoad(transform.root.gameObject);

        //initialize Ascalon
        ascalonInstance = new Ascalon();
        ascalonInstance.netModule = new AscalonEmptyNet();
        ascalonInstance.Initialize();

        //main config
        if (autoLoadMainConfig == true)
        {
            AscalonConfigTools.ReadConfigUnity(mainConfigName);
        }
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
            AscalonConfigTools.WriteConfigUnity("config");
        }
    }
}
#endif