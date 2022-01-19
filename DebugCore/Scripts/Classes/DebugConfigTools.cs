using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

//Tools for writing and reading config files formatted for DebugCore

public class DebugConfigTools
{
    public static async void WriteConfig(string argPath)
    {
        await Task.Run(() =>
        {
            //first, prepare the config string
            string cfgString = "";

            foreach (ConVar conVar in DebugCore.instance.conVars)
            {
                if (conVar.flags.HasFlag(ConFlags.Save))
                {
                    cfgString += conVar.name + " " + DebugCoreUtil.ConVarDataToString(conVar.GetData()) + "\n";
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

    public static void WriteConfigUnity(string argFileName)
    {
        //ensure config directory exists
        if (!Directory.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + "config"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + Path.DirectorySeparatorChar + "config");
        }

        WriteConfig(Application.persistentDataPath + Path.DirectorySeparatorChar + "config" + Path.DirectorySeparatorChar + argFileName + ".cfg");
    }

    public static void ReadConfig(string argPath, bool argSilent)
    {
        //handle bad config names
        if (!File.Exists(argPath))
        {
            if (!argSilent)
            {
                DebugCore.FeedEntry("Configuration file not found.", "File \"" + argPath + "\" does not exist.", FeedEntryType.Warning);
            }
            return;
        }

        using (System.IO.StreamReader reader = new System.IO.StreamReader(argPath))
        {
            string line = "";
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Length > 0 && line.Substring(0, 2) != "//") //filter blank lines and comments
                {
                    DebugCore.Call(line, DebugCallSource.Internal);
                }
            }
        }
    }

    public static void ReadConfig(string argPath)
    {
        ReadConfig(argPath, false);
    }

    public static void ReadConfigUnity(string argFileName)
    {
        ReadConfig(Application.persistentDataPath + System.IO.Path.DirectorySeparatorChar + "config" + System.IO.Path.DirectorySeparatorChar + argFileName + ".cfg");
    }
}
