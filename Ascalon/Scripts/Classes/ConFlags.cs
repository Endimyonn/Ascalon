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
    Save = 1,
    //This ConVar should be saved when a config is generated

    Cheat = 2,
    //This command/ConVar may only be executed/updated when host_cheats is enabled

    ClientReplicated = 4,
    //This command/ConVar should be sent to the client when they connect to a server or it is updated. Additionally, any accessing of this will access the server copy if possible.
    
    ClientInfo = 8,
    //This ConVar should be sent to the server as client info
    
    ServerOnly = 16,
    //This command/ConVar may only be executed/updated by the host
    
    NotifyClients = 32,
    //This command/ConVar should generate a notification when executed/updated by the host
    
    Sensitive = 64,
    //This ConVar contains sensitive data (e.g. passwords) and should not be exposed easily
    
    LockWhileConnected = 128,
    //This command/ConVar may only be executed/updated when the client is not connected to a server. Also applies to clients acting as servers themselves, but not dedicated servers.
    
    RunOnServer = 256,
    //This command should be sent to the server for calling. Do not use on ConVars unless only 2 players may be connected at a time. Do not combine with ClientReplicated or you will experience infinite loops.

    NoRCon = 512
    //This command or ConVar cannot be called by RCon.
}
