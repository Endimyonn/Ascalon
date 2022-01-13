using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The debug net module is a developer-created interface between
//the DebugCore and their network library of choice. Any modules
//created should implement all of this base class, and base.Awake()
//must be called first.

//This module is necessary for DebugCore to function. If network
//support is not required, an offline module is included for use.
public class DebugNetModule : MonoBehaviour
{
    //singleton
    public static DebugNetModule instance;

    public bool isClient;

    public bool sessionActive;

    protected virtual void Awake()
    {
        //singleton logic
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;
    }

    public virtual void SendCallToClient(string argCall, string argClient)
    {
        if (isClient)
        {
            return;
        }

        //nyi
    }

    public virtual void SendReplicatedsToClient(string argClient)
    {
        if (isClient)
        {
            return;
        }

        foreach (ConVar conVar in GetComponent<DebugCore>().conVars)
        {
            if (conVar.flags.HasFlag(ConFlags.ClientReplicated))
            {
                //stub
            }
        }
    }

    public virtual void ReceiveClientInfo(object argData)
    {
        //stub
    }
}