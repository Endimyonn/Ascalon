using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//console-related commands
public class ConRelCommands
{
    [ConVar("con_verbose", "Set verbose mode on debug console")]
    static ConVar cvar_con_verbose = new ConVar(typeof(bool), false)
    {
        DataChanged = (object oldData, object newData) =>
        {

        }
    };

    [ConCommand("help", "Get info about a command or ConVar")]
    static void cmd_help(string argEntryName)
    {
        ConCommand findCommand = DebugCore.GetConCommandEntry(argEntryName);
        ConVar findConVar = DebugCore.GetConVarEntry(argEntryName);

        if (DebugCore.ConCommandExists(argEntryName))
        {
            string parmsString = "";
            System.Tuple<string, string>[] commandParms = DebugCoreUtil.ParmsNamesToStrings(findCommand.parms);

            for (int i = 0; i < commandParms.Length; i++)
            {
                parmsString += "(" + (i + 1) + ") " + commandParms[i].Item1 + " (" + commandParms[i].Item2 + ")" + "\n";
            }
            if (commandParms.Length > 0)
            {
                parmsString = parmsString.Remove(parmsString.Length - 1, 1);  //remove trailing newline
            }
            else
            {
                parmsString = "None";   //if the command has no arguments
            }

            DebugCore.FeedEntry("Help for command '" + findCommand.name + "':",
                                   " * Description:\n" + findCommand.description +
                                   "\n\n * Parameters:\n" + parmsString,
                                   FeedEntryType.Info);
        }
        else if (DebugCore.ConVarExists(argEntryName))
        {
            DebugCore.FeedEntry("Help for ConVar '" + findConVar.name + "':",
                                   " * Value:\n" + DebugCoreUtil.ConVarDataToString(findConVar.GetData()) +
                                   "\n\n * Description:\n" + findConVar.description +
                                   "\n\n * Type:\n" + findConVar.cvarDataType,
                                   FeedEntryType.Info);
        }
        else
        {
            DebugCore.FeedEntry("No such command or ConVar \"" + argEntryName + "\"", "", FeedEntryType.Error);
        }
    }

    //control whether we accept calls from other clients in a server
    //this is typically very dangerous and should not be enabled by default
    [ConVar("client_allowclientcall", ConFlags.LockWhileConnected)]
    static ConVar cvar_client_allowclientcall = new ConVar(false);

    //control whether we accept calls without ClientReplicated from a server
    [ConVar("client_allowservercall", ConFlags.LockWhileConnected)]
    static ConVar cvar_client_allowservercall = new ConVar(false);
}
