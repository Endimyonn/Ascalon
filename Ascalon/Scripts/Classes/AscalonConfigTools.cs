using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

#if UNITY_2019_1_OR_NEWER
using UnityEngine;
#endif

#if GODOT
using Godot;
#endif

//Tools for writing and reading config files formatted for DebugCore

public class AscalonConfigTools
{
    #region Writing
    public static async void WriteConfig(string argPath)
    {
        await Task.Run(() =>
        {
            //first, prepare the config string
            string cfgString = "";

            foreach (ConVar conVar in Ascalon.instance.conVars)
            {
                if (conVar.flags.HasFlag(ConFlags.Save))
                {
                    cfgString += conVar.name + " " + AscalonUtil.ConVarDataToString(conVar.GetData()) + "\n";
                }
            }

            //remove trailing newline if at least one saveable ConVar was found
            if (cfgString != "")
            {
                cfgString = cfgString.Remove(cfgString.Length - 1, 1);
            }

            
            using (StreamWriter writer = new StreamWriter(argPath, false))
            {
                writer.Write(cfgString);
                writer.Close();
            }
        }
        );
    }

    //Write a config in the persistent data path
    public static void WriteConfigUnity(string argFileName)
    {
#if UNITY_2019_1_OR_NEWER
        //ensure config directory exists
        if (!Directory.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + "config"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + Path.DirectorySeparatorChar + "config");
        }

        WriteConfig(Application.persistentDataPath + Path.DirectorySeparatorChar + "config" + Path.DirectorySeparatorChar + argFileName + ".cfg");
#else
        Ascalon.Log("Cannot write a config Unity-style outside of Unity!", LogMode.Error);
#endif
    }

#if GODOT
    //Write a config to user data
    public static void WriteConfigGodot(string argFileName)
    {
        //ensure config directory exists
        if (Directory.Exists(OS.GetUserDataDir() + Path.DirectorySeparatorChar + "config") == false)
        {
            Directory.CreateDirectory(OS.GetUserDataDir() + Path.DirectorySeparatorChar + "config");
        }

        WriteConfig(OS.GetUserDataDir() + Path.DirectorySeparatorChar + "config" + Path.DirectorySeparatorChar + argFileName + ".cfg");
    }
#endif
#endregion

    #region Reading
    public static void ReadConfig(string argPath, bool argSilent)
    {
        //handle bad config names
        if (!File.Exists(argPath))
        {
            if (!argSilent)
            {
                if (Ascalon.instance.ready)
                {
                    Ascalon.Log("Configuration file not found.", "File \"" + argPath + "\" does not exist.", LogMode.Warning);
                }
                else
                {
                    Console.WriteLine("Configuration file not found. File \"" + argPath + "\" does not exist.");
                }
                
            }
            return;
        }

        using (StreamReader reader = new StreamReader(argPath))
        {
            string line = "";
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Length > 0 && line.Substring(0, 2) != "//") //filter blank lines and comments
                {
                    try
                    {
                        Ascalon.Call(line, new AscalonCallContext(AscalonCallSource.Internal));
                    }
                    catch (Exception e)
                    {
                        Ascalon.Log("Exception while loading config line: " + e.Message, line);
                    }
                }
            }
        }
    }

    public static void ReadConfig(string argPath)
    {
        ReadConfig(argPath, false);
    }

#if UNITY_2019_1_OR_NEWER
    //Read a config from the persistent data path
    public static void ReadConfigUnity(string argFileName)
    {
        ReadConfig(Application.persistentDataPath + Path.DirectorySeparatorChar + "config" + Path.DirectorySeparatorChar + argFileName + ".cfg");
    }
#endif

#if GODOT
    //Read a config from user data
    public static void ReadConfigGodot(string argFileName)
    {
        ReadConfig(OS.GetUserDataDir() + Path.DirectorySeparatorChar + "config" + Path.DirectorySeparatorChar + argFileName + ".cfg");
    }
#endif
#endregion

    #region Existence Checking
    public static bool ConfigExists(string argPath)
    {
        return File.Exists(argPath);
    }

#if UNITY_2019_1_OR_NEWER
    public static bool ConfigExistsUnity(string argFileName)
    {
        return ConfigExists(Application.persistentDataPath + Path.DirectorySeparatorChar + "config" + Path.DirectorySeparatorChar + argFileName + ".cfg");
    }
#endif

#if GODOT
    public static bool ConfigExistsGodot(string argFileName)
    {
        return ConfigExists(OS.GetUserDataDir() + Path.DirectorySeparatorChar + "config" + Path.DirectorySeparatorChar + argFileName + ".cfg");
    }
#endif
#endregion
}
