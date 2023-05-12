#if UNITY_2019_1_OR_NEWER
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionFromBoot : MonoBehaviour
{
    public string defaultSceneName = "";

    private void LateUpdate()
    {
        if (BootOMatic.bootNeeded)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(BootOMatic.returnSceneName);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(defaultSceneName);
        }

        Destroy(gameObject);
    }
}
#endif