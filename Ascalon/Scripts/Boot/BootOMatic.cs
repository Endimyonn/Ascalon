#if UNITY_2019_1_OR_NEWER
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//The Boot-O-Matic provides a way to quickly load Ascalon without having to start on the Boot scene.
//It sends to the boot scene, then immediately back.
public class BootOMatic : MonoBehaviour
{
    public static bool bootNeeded = false;
    public static string returnSceneName;


    private void Awake()
    {
        //is ascalon present?
        if (Ascalon.instance == null)
        {
            bootNeeded = true;
            returnSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene("Boot");
        }
    }
}
#endif