using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionFromBoot : MonoBehaviour
{
    public string sceneName = "";

    private void LateUpdate()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        Destroy(gameObject);
    }
}
