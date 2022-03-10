using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//hacky workaround until I properly implement window hiding by default
public class DisableUIOnStart : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(false);
    }
}
