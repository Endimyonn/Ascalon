#if UNITY_2019_1_OR_NEWER
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnitySceneConfigLoader
{
	private static bool doConfigLoad = true;
	
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Initialize()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}
	
	private static void OnSceneLoaded(Scene argScene, LoadSceneMode argLoadMode)
	{
		if (AscalonConfigTools.ConfigExistsUnity(argScene.name) == true)
		{
			AscalonConfigTools.ReadConfigUnity(argScene.name);
		}
	}
	
	[ConVar("config_load_scene_configs", "If enabled, upon loading a scene, the config matching the scene's name will be loaded, if it exists.", ConFlags.Save)]
	static ConVar cvar_config_load_scene_configs = new ConVar(true)
	{
		DataChanged = (object oldData, object newData) =>
		{
			doConfigLoad = (bool)newData;
		}
	};
}
#endif
