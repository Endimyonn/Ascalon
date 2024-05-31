using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_2019_1_OR_NEWER
using UnityEngine;
#endif

//The debug net module is a developer-created interface between
//Ascalon and their network library of choice. Any modules created
//should implement all of this base class, and base.Awake() must
//be called first.

//This module is necessary for Ascalon to function. If network
//support is not required, an offline module is included for use.

//All defined methods include skeletons to guide how a custom module
//should work.
[System.Serializable]
public abstract class AscalonNetModule
{
    //singleton
    public static AscalonNetModule instance;

    //reference to the core Ascalon instance
    public Ascalon core;

    //Network role - should always be kept updated by the network
    //library in use.
    public NetRole role = NetRole.Inactive;

    public virtual void Initialize()
    {
        //singleton logic
        if (instance != null)
        {
            Console.WriteLine("Duplicate AscalonNetModule of type " + this.GetType().Name + " created - do not do this!");
            return;
        }
        instance = this;
    }

    //Update function is not usually needed for NetModules, but it is included
    //for extensibility purposes.
    public virtual void Update()
    {

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
    public virtual void NetCall(string argCall, AscalonCallContext argContext, AscalonCallNetTarget argTarget)
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
    public virtual void SendReplicatedToAllClients(string argCall, AscalonCallContext argContext)
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
    public virtual void SendAllReplicatedToClient(AscalonCallNetTarget argTarget)
    {
        if (GetRole() != NetRole.Server)
        {
            return;
        }

        foreach (ConVar conVar in core.conVars)
        {
            if (conVar.flags.HasFlag(ConFlags.ClientReplicated))
            {
                //stub
            }
        }
    }

    public virtual void ReceiveClientInfo(object argData)
    {
        Ascalon.Log("Function in example AscalonNetModule was called - use a derived one!");

        //stub
    }
}

public enum NetRole
{
    Inactive,
    Client,
    Server
}

public struct AscalonCallNetTarget
{
    public string targetClient;
    public NetRole targetMode;

    public AscalonCallNetTarget(NetRole argTargetingMode, string argTargetClient = "")
    {
        targetClient = argTargetClient;
        targetMode = argTargetingMode;
    }
}