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

    [ConCommand("obj_spawn", "Instantiate an object at the mouse position.")]
    static void cmd_spawnobj(string argPath)
    {
        Object prefab = Resources.Load(argPath);
        Debug.Log(argPath);
        if (prefab != null)
        {
            DebugCore.FeedEntry("Spawning asset");
            GameObject.Instantiate(prefab, DebugCoreUtil.MouseWorldPosition(), Quaternion.Euler(0, 0, 0));
        }
        else
        {
            DebugCore.FeedEntry("Invalid asset specified");
        }
    }

    [ConCommand("obj_teleport", "Teleport an object to a specified position.", ConFlags.Cheat)]
    static void cmd_obj_teleport(GameObjectFinder argGameObject, float[] argPosition)
    {
        argGameObject.result.transform.position = new Vector3(argPosition[0], argPosition[1], argPosition[2]);
    }
}
