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
        Ascalon.Log("Loading scene: " + argScene, "", LogMode.Info);
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
        Ascalon.Log("Reloading scene", "", LogMode.Info);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //this is how alias commands may be implemented - as a passthrough to the base command
    [ConCommand("game_reloadscene", "Reload the current scene")]
    static void cmd_game_reloadscene()
    {
        Ascalon.Call("game_reloadlevel");
    }

    [ConCommand("logic_timescale")]
    static void cvar_logic_timescale(float argTimeScale)
    {
        Ascalon.Log("Setting timescale to " + argTimeScale, "", LogMode.Info);
        Time.timeScale = argTimeScale;
    }

    [ConCommand("obj_spawn", "Instantiate an object at the mouse position.")]
    static void cmd_spawnobj(string argPath)
    {
        Object prefab = Resources.Load(argPath);
        Debug.Log(argPath);
        if (prefab != null)
        {
            Ascalon.Log("Spawning asset");
            GameObject.Instantiate(prefab, AscalonUtil.MouseWorldPosition(), Quaternion.Euler(0, 0, 0));
        }
        else
        {
            Ascalon.Log("Invalid asset specified");
        }
    }

    [ConCommand("obj_teleport", "Teleport an object to a specified position.", ConFlags.Cheat)]
    static void cmd_obj_teleport(GameObjectFinder argGameObject, float[] argPosition)
    {
        argGameObject.result.transform.position = new Vector3(argPosition[0], argPosition[1], argPosition[2]);
    }
}
