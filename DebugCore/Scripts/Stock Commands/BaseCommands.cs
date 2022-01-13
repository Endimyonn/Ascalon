using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using System.Reflection;

public class BaseCommands
{
    [ConCommand("game_changelevel", "Load a specified scene.")]
    static void cmd_game_changelevel(string argScene)
    {
        DebugCore.FeedEntry("Loading scene: " + argScene, "", FeedEntryType.Info);
        SceneManager.LoadScene(argScene);
    }

    [ConCommand("game_loadscene", "Load a specified scene.")]
    static void cmd_game_loadscene(string argScene)
    {
        cmd_game_changelevel(argScene);
    }

    [ConCommand("game_reloadlevel", "Reload the current scene.")]
    static void cmd_game_reloadlevel()
    {
        DebugCore.FeedEntry("Reloading scene", "", FeedEntryType.Info);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //this is how alias commands may be implemented - as a passthrough to the base command
    [ConCommand("game_reloadscene", "Reload the current scene")]
    static void cmd_game_reloadscene()
    {
        DebugCore.Call("game_reloadlevel");
    }

    [ConCommand("logic_timescale")]
    static void cvar_logic_timescale(float argTimeScale)
    {
        DebugCore.FeedEntry("Setting timescale to " + argTimeScale, "", FeedEntryType.Info);
        Time.timeScale = argTimeScale;
    }

    [ConCommand("obj_spawn", "Instantiate an object at the mouse position. Not yet functional.")]
    static void cmd_spawnobj(string argPath)  //this is not yet functional
    {
        Object prefab = Resources.Load(argPath);
        Debug.Log(argPath);
        if (prefab != null)
        {
            DebugCore.FeedEntry("Spawning asset");
            GameObject.Instantiate(prefab, ControlTools.MouseWorldPosition(), Quaternion.Euler(0, 0, 0));
        }
        else
        {
            DebugCore.FeedEntry("Invalid asset specified");
        }
    }

    [ConVar("host_cheats", "Determines whether commands and ConVars marked\nas cheats may be used")]
    static ConVar cvar_host_cheats = new ConVar(false);

    [ConCommand("host_writeconfig", "Write a configuration file with the specified name")]
    static void cmd_host_writeconfig(string argConfigName)
    {
        //stubbed until config reading/writing is implemented
    }

    [ConCommand("test_testerfunc", "command testing command")]
    static void cmd_test_testerfunc()
    {
        DebugCore.FeedEntry("tester", "this is\na test command!\nif you see this, it worked", FeedEntryType.Info);
    }

    [ConCommand("test_testerfunc2", "command testing command with args for testing args")]
    static void cmd_test_testerfunc2(string argText, string argText2)
    {
        DebugCore.FeedEntry("TesterFunc2 called", "arg 1: " + argText + "\narg 2: " + argText2, FeedEntryType.Info);
    }

    [ConCommand("test_testerfunc3", "command testing command for command validity checking testing")]
    static void cmd_test_testerfunc3(float argFloatIn, int argIntIn)
    {
        DebugCore.FeedEntry("TesterFunc3 called", "Result of the two args: " + argIntIn * argFloatIn, FeedEntryType.Info);
    }

    [ConCommand("test_testerfunc4", "command testing command for bools")]
    static void cmd_test_testerfunc4(bool argBool)
    {
        if (argBool == true)
        {
            DebugCore.FeedEntry("TesterFunc4 called", "Received a bool of true", FeedEntryType.Info);
        }
        else
        {
            DebugCore.FeedEntry("TesterFunc4 called", "Received a bool of false", FeedEntryType.Info);
        }
    }

    [ConCommand("obj_teleport", "Teleport an object to a specified position.", ConFlags.Cheat)]
    static void cmd_obj_teleport(GameObjectFinder argGameObject, float[] argPosition)
    {
        argGameObject.result.transform.position = new Vector3(argPosition[0], argPosition[1], argPosition[2]);
    }

    [ConCommand("obj_moveupfive", "Move a GameObject up 5 units", ConFlags.Cheat)]
    static void cmd_obj_moveupfive(GameObjectFinder argGameObject)
    {
        argGameObject.result.transform.position += new Vector3(0, 5, 0);
    }

    [ConCommand("obj_moveupgive", "Dummy command for alphabetical suggestion testing")]
    static void cmd_obj_moveupgive()
    {
        DebugCore.FeedEntry("This is a dummy command", "", FeedEntryType.Info);
    }

    [ConCommand("obj_moveupbive", "Dummy command for alphabetical suggestion testing")]
    static void cmd_obj_moveupbive()
    {
        DebugCore.FeedEntry("This is a dummy command", "", FeedEntryType.Info);
    }

    [ConVar("test_testervar1", "Test of ConVar implementation")]
    static ConVar cvar_test_testervar1 = new ConVar("default value")
    {
        DataChanged = (object oldData, object newData) =>
        {
            //Debug.Log("passed");
            Debug.Log("test_testervar1 DataChanged callback triggered!. old data: " + oldData + " | new data: " + newData);
        }
    };

    [ConVar("test_testervar2", "Another test of ConVar implementation")]
    static ConVar cvar_test_testervar2 = new ConVar(3.15f)
    {
        DataChanged = (object oldData, object newData) =>
        {
            Debug.Log("test_testervar2 has datachanged. old data: " + oldData + " | new data: " + newData);
        }
    };

    [ConVar("test_testervar3", "Another test of ConVar implementation", ConFlags.Cheat | ConFlags.Sensitive)]
    static ConVar cvar_test_testervar3 = new ConVar(555)
    {
        DataChanged = (object oldData, object newData) =>
        {
            Debug.Log("test_testervar3 has datachanged. old data: " + oldData + " | new data: " + newData);
        }
    };

    [ConVar("test_testervar4", "Another test of ConVar array implementation")]
    static ConVar cvar_test_testervar4 = new ConVar(new float[] { 5, 10, 15, 20, 21, 23 })
    {
        DataChanged = (object oldData, object newData) =>
        {
            Debug.Log("test_testervar4 has datachanged. old data: " + oldData + " | new data: " + newData);
        }
    };

    [ConCommand("test_testerfunc5", "Testing command for testing ConVar accessing")]
    static void cmd_test_testerfunc5()
    {
        Debug.Log("The value of test_testervar1 is: " + DebugCore.GetConVar("test_testervar1"));
    }

    [ConVar("test_testervar5", "Test of ConFlag.Save", ConFlags.Save)]
    static ConVar cvar_test_testervar5 = new ConVar(99.315f);

    [ConVar("test_cheatvar", "Test of ConFlag.Cheat", ConFlags.Cheat)]
    static ConVar cvar_test_cheatvar = new ConVar(44.323f);

    [ConCommand("test_cheatcmd", "Test of ConFlag.Cheat", ConFlags.Cheat)]
    static void cmd_test_cheatcmd()
    {
        Debug.Log("Cheats are enabled!");
    }

    [ConVar("test_clampvar", "Test of ConVar clamping code")]
    static ConVar cvar_test_clampvar = new ConVar(44.05f)
    {
        DataChanged = (object oldData, object newData) =>
        {
            if ((float)newData > 99.5f)
            {
                DebugCore.SetConVar("test_clampvar", 99.5f);
            }
            else if ((float)newData < 20.5f)
            {
                DebugCore.SetConVar("test_clampvar", 20.5f);
            }
        }
    
    };
}
