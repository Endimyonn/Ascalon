using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The debug net module is a developer-created interface between
//the DebugCore and their network library of choice. Any modules
//created should implement all of this base class, and base.Awake()
//must be called first.

//This module is necessary for DebugCore to function. If network
//support is not required, an offline module is included for use.

//All defined methods include skeletons to guide how a custom module
//should work.
public abstract class DebugNetModule : MonoBehaviour
{
    //singleton
    public static DebugNetModule instance;

    //Network role - should always be kept updated by the network
    //library in use.
    public NetRole role = NetRole.Inactive;

    protected virtual void Awake()
    {
        //singleton logic
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;
    }

    public static void SetRole(NetRole argRole)
    {
        instance.role = argRole;
    }

    public static NetRole GetRole()
    {
        return instance.role;
    }

    //clients and servers should call this on a client with the proper context
    public virtual void NetCall(string argCall, DebugCallContext argContext, DebugCallNetTarget argTarget)
    {
        if (GetRole() == NetRole.Inactive)
        {
            return;
        }

        if (GetRole() == NetRole.Client)
        {

        }
        else if (GetRole() == NetRole.Server)
        {

        }
    }

    //Send a command/ConVar with ClientReplicated flag to all clients. Only
    //the server is allowed to run this.
    public virtual void SendReplicatedToAllClients(string argCall, DebugCallContext argContext)
    {
        //security
        if (GetRole() != NetRole.Server)
        {
            return;
        }

        //stub
    }

    //Send all stored ConVars flagged with ClientReplicated to a client. Only
    //the server is allowed to run this.
    public virtual void SendAllReplicatedToClient(DebugCallNetTarget argTarget)
    {
        if (GetRole() != NetRole.Server)
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
        Debug.Log("Function in example DebugNetModule was called - use a derived one!");

        //stub
    }
}

public enum NetRole
{
    Inactive,
    Client,
    Server
}

public struct DebugCallNetTarget
{
    public string targetClient;
    public NetRole targetMode;

    public DebugCallNetTarget(NetRole argTargetingMode, string argTargetClient = "")
    {
        targetClient = argTargetClient;
        targetMode = argTargetingMode;
    }
}