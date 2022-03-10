using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//the RCon module allows commands to be received from any client
//built to support it over the network
public class DebugRConModule : MonoBehaviour
{
    [ConVar("rcon_password", "RCon password", ConFlags.Sensitive | ConFlags.ServerOnly)]
    ConVar cvar_rcon_password = new ConVar("");

    [ConVar("rcon_address", "The address to send RCon requests to")]
    ConVar cvar_rcon_address = new ConVar("");

    [ConCommand("rcon", "Send an RCon command")]
    static void cmd_rcon(string argCall)
    {
        //NYI
    }
}
