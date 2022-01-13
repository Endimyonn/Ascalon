using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This defines ConFlags, which are used for gating access to,
//and controlling the usage of, commands and ConVars. This enum
//may be freely added onto.

[Flags]
public enum ConFlags
{
    Save = 0,               //This ConVar should be saved when a config is generated
    Cheat = 1,              //This command/ConVar may only be executed/updated when host_cheats is enabled
    ClientReplicated = 2,   //This command/ConVar should be sent to the client when they connect to a server
    ClientInfo = 4,         //This ConVar should be sent to the server as client info
    ServerOnly = 8,         //This command/ConVar may only be executed/updated by the host
    NotifyClients = 16,      //This command/ConVar should generate a notification when executed/updated by the host
    Sensitive = 32,          //This ConVar contains sensitive data (e.g. passwords) and should not be exposed easily
    LockWhileConnected = 64  //This command/ConVar may only be executed/updated when the client is not connected to a server. Also applies to clients acting as servers themselves, but not dedicated servers.
}
