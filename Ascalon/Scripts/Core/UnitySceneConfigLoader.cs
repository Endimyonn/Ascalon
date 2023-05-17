#if UNITY_2019_1_OR_NEWER
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnitySceneConfigLoader : MonoBehaviour
{
    [Header("Settings")]
    public LoadOn loadOn = LoadOn.Start;

    private void Awake()
    {
        if (loadOn == LoadOn.Awake)
        {
            AscalonConfigTools.ReadConfigUnity(SceneManager.GetActiveScene().name);
        }
    }

    private void Start()
    {
        if (loadOn == LoadOn.Start)
        {
            AscalonConfigTools.ReadConfigUnity(SceneManager.GetActiveScene().name);
        }
    }

    [System.Serializable]
    public enum LoadOn
    {
        Awake,
        Start
    }
}
#endif
