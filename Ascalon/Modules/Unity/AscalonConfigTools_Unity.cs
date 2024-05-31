#if UNITY_2019_1_OR_NEWER
using System;
using System.IO;
using UnityEngine;

public partial class AscalonConfigTools
{
    //Write a config in the persistent data path
    public static void WriteConfigUnity(string argFileName)
    {
        //ensure config directory exists
        if (!Directory.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + "config"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + Path.DirectorySeparatorChar + "config");
        }

        WriteConfig(Application.persistentDataPath + Path.DirectorySeparatorChar + "config" + Path.DirectorySeparatorChar + argFileName + ".cfg");
    }

    //Read a config from the persistent data path
    public static void ReadConfigUnity(string argFileName)
    {
        ReadConfig(Application.persistentDataPath + Path.DirectorySeparatorChar + "config" + Path.DirectorySeparatorChar + argFileName + ".cfg");
    }

    public static bool ConfigExistsUnity(string argFileName)
    {
        return ConfigExists(Application.persistentDataPath + Path.DirectorySeparatorChar + "config" + Path.DirectorySeparatorChar + argFileName + ".cfg");
    }
}
#endif
