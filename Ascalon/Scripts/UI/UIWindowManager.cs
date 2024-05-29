#if UNITY_2019_1_OR_NEWER
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWindowManager : MonoBehaviour
{
    public List<UIWindow> windows = new List<UIWindow>();

    //todo: is this fully optimal?
    public static UIWindowManager instance;

    public static int numActive = 0;

    private void Awake()
    {
        //singleton
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;


        for (int i = 0; i < transform.childCount; i++)
        {
            windows.Add(new UIWindow(transform.GetChild(i).gameObject));
            if (transform.GetChild(i).gameObject.activeSelf)
            {
                numActive++;
            }
        }
    }

    public void ToggleWindow(string argWindowName)
    {
        windows.Find(window => window.gameObject.name == argWindowName).Toggle();
    }

    public GameObject GetWindow(string argWindowName)
    {
        return windows.Find(window => window.gameObject.name == argWindowName).gameObject;
    }

    public static void UpdateWindowCount()
    {
        if (numActive == 0)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (numActive > 0)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Debug.LogWarning("Number of active windows is <0! This shouldn't be happening, tell Joe about this.");
        }
    }
}

public class UIWindow
{
    public GameObject gameObject;
    public bool active;

    public UIWindow(GameObject argWindowObject)
    {
        gameObject = argWindowObject;
        active = argWindowObject.activeSelf;
    }

    public void Toggle()
    {
        gameObject.SetActive(!active);

        active = !active;

        //update active window count
        UIWindowManager.numActive += (active == true ? 1 : -1);
    }
}
#endif
