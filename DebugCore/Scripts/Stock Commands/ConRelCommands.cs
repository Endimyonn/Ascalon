using System;
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

            string flagsString = "";
            if (findCommand.flags == 0)
            {
                flagsString = " None";
            }
            else
            {
                foreach (ConFlags flag in Enum.GetValues(typeof(ConFlags)))
                {
                    if (findCommand.flags.HasFlag(flag))
                    {
                        flagsString += flag.ToString() + "\n";
                    }
                }

                flagsString.Remove(flagsString.Length - 1, 1);
            }

            DebugCore.FeedEntry("Help for command '" + findCommand.name + "':",
                                   " * Description:\n" + findCommand.description +
                                   "\n\n * Parameters:\n" + parmsString +
                                   "\n\n * Flags: \n" + flagsString,
                                   FeedEntryType.Info);
        }
        else if (DebugCore.ConVarExists(argEntryName))
        {
            string flagsString = "";
            if (findConVar.flags == 0)
            {
                flagsString = " None";
            }
            else
            {
                foreach (ConFlags flag in Enum.GetValues(typeof(ConFlags)))
                {
                    if (findConVar.flags.HasFlag(flag))
                    {
                        flagsString += flag.ToString() + "\n";
                    }
                }

                flagsString.Remove(flagsString.Length - 1, 1);
            }

            DebugCore.FeedEntry("Help for ConVar '" + findConVar.name + "':",
                                   " * Value:\n" + DebugCoreUtil.ConVarDataToString(findConVar.GetData()) +
                                   "\n\n * Description:\n" + findConVar.description +
                                   "\n\n * Type:\n" + findConVar.cvarDataType +
                                   "\n\n * Flags:\n" + flagsString,
                                   FeedEntryType.Info);
        }
        else
        {
            DebugCore.FeedEntry("No such command or ConVar \"" + argEntryName + "\"", "", FeedEntryType.Error);
        }
    }


    //----------------------------------------------------------------------------------//
    // The following commands and ConVars are vital to the functionality of the system! //
    //              Do not delete them unless you know what you are doing!              //
    //----------------------------------------------------------------------------------//

    //control whether we accept calls from other clients in a server
    [ConVar("client_allowclientcall", "Allows other clients to send calls to the player.", ConFlags.LockWhileConnected)]
    static ConVar cvar_client_allowclientcall = new ConVar(false);

    //control whether we accept calls without ClientReplicated from a server
    [ConVar("client_allowservercall", "Allows the server to send non-standard calls to the player", ConFlags.LockWhileConnected)]
    static ConVar cvar_client_allowservercall = new ConVar(false);

    [ConVar("host_cheats", "Determines whether commands and ConVars marked\nas cheats may be used")]
    static ConVar cvar_host_cheats = new ConVar(false);

    [ConCommand("client_writeconfig", "Write a configuration file with the specified name")]
    static void cmd_client_writeconfig(string argConfigName)
    {
        //stubbed until config reading/writing is implemented
    }

    //-------------------//
    // End vital section //
    //-------------------//

}
