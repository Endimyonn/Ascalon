using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class defines commands and ConVars related to the system itself.
//Some of these are vital to the system's function and are marked as
//such - do not modify them unless you know what you're doing.
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
        ConCommand findCommand = Ascalon.GetConCommandEntry(argEntryName);
        ConVar findConVar = Ascalon.GetConVarEntry(argEntryName);

        if (Ascalon.ConCommandExists(argEntryName))
        {
            if (findCommand.flags.HasFlag(ConFlags.Hidden))
            {
                Ascalon.Log("No such command or ConVar \"" + argEntryName + "\"", "", LogMode.Error);
                return;
            }
            string parmsString = "";
            System.Tuple<string, string>[] commandParms = AscalonUtil.ParmsNamesToStrings(findCommand.parms);

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

            Ascalon.Log("Help for command '" + findCommand.name + "':",
                                   " * Description:\n" + findCommand.description +
                                   "\n\n * Parameters:\n" + parmsString +
                                   "\n\n * Flags: \n" + flagsString,
                                   LogMode.Info);
        }
        else if (Ascalon.ConVarExists(argEntryName))
        {
            if (findConVar.flags.HasFlag(ConFlags.Hidden))
            {
                Ascalon.Log("No such command or ConVar \"" + argEntryName + "\"", "", LogMode.Error);
                return;
            }
            string dataString = AscalonUtil.ConVarDataToString(findConVar.GetData());
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

                        if (flag == ConFlags.Sensitive)
                        {
                            dataString = "(PROTECTED)";
                        }
                    }
                }

                flagsString.Remove(flagsString.Length - 1, 1);
            }

            Ascalon.Log("Help for ConVar '" + findConVar.name + "':",
                                   " * Value:\n" + dataString +
                                   "\n\n * Description:\n" + findConVar.description +
                                   "\n\n * Type:\n" + findConVar.cvarDataType +
                                   "\n\n * Flags:\n" + flagsString,
                                   LogMode.Info);
        }
        else
        {
            Ascalon.Log("No such command or ConVar \"" + argEntryName + "\"", "", LogMode.Error);
        }
    }


    //----------------------------------------------------------------------------------//
    // The following commands and ConVars are vital to the functionality of the system! //
    //              Do not delete them unless you know what you are doing!              //
    //----------------------------------------------------------------------------------//

    //control whether we accept calls from other clients in a server
    [ConVar("client_allowclientcall", "Allows other clients to send calls to the player.", ConFlags.LockWhileConnected | ConFlags.Save)]
    static ConVar cvar_client_allowclientcall = new ConVar(false);

    //control whether we accept calls without ClientReplicated from a server
    [ConVar("client_allowservercall", "Allows the server to send non-standard calls to the player", ConFlags.LockWhileConnected | ConFlags.Save)]
    static ConVar cvar_client_allowservercall = new ConVar(false);

    [ConVar("server_cheats", "Determines whether commands and ConVars marked\nas cheats may be used", ConFlags.ClientReplicated | ConFlags.NotifyClients)]
    static ConVar cvar_server_cheats = new ConVar(false);

    [ConCommand("client_writeconfig", "Write a configuration file with the specified name")]
    static void cmd_client_writeconfig(string argConfigName)
    {
        Debug.Log("Writing config to \"" + Application.persistentDataPath + System.IO.Path.DirectorySeparatorChar + "config" + System.IO.Path.DirectorySeparatorChar + argConfigName + ".cfg\"");
        AscalonConfigTools.WriteConfigUnity(argConfigName);
    }

    [ConCommand("client_runconfig", "Run a configuration file with the specified name")]
    static void cmd_client_runconfig(string argConfigName)
    {
        AscalonConfigTools.ReadConfigUnity(argConfigName);
    }

    //-------------------//
    // End vital section //
    //-------------------//
}
