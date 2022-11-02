using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//Attach to a canvas with a Window as a child. Kind of hacky, but it works

public class UIConsoleToggler : MonoBehaviour
{
    public TMP_InputField inputField;
    private bool active;

    private void Awake()
    {
        
    }

    private void Start()
    {
        active = transform.GetChild(0).gameObject.activeInHierarchy;
    }

    private void Update()
    {
        //do we need to toggle?
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            if (!inputField.isFocused)
            {
                ToggleWindow();
            }
        }
    }

    public void ToggleWindow()
    {
        if (active)
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }

        active = !active;
    }
}
