using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

/// <summary>
/// Tools for writing and reading config files formatted for Ascalon
/// </summary>
public partial class AscalonConfigTools
{
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

    public static bool ConfigExists(string argPath)
    {
        return File.Exists(argPath);
    }
}
