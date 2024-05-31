#if GODOT
using System;
using System.IO;
using Godot;

public partial class AscalonConfigTools
{
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

    //Read a config from user data
    public static void ReadConfigGodot(string argFileName)
    {
        ReadConfig(OS.GetUserDataDir() + Path.DirectorySeparatorChar + "config" + Path.DirectorySeparatorChar + argFileName + ".cfg");
    }

    public static bool ConfigExistsGodot(string argFileName)
    {
        return ConfigExists(OS.GetUserDataDir() + Path.DirectorySeparatorChar + "config" + Path.DirectorySeparatorChar + argFileName + ".cfg");
    }
}
#endif
