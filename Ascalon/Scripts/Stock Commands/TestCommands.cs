using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCommands
{
    /*

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

    };*/

    [ConCommand("obj_moveupfive", "Move a GameObject up 5 units", ConFlags.Cheat)]
    static void cmd_obj_moveupfive(GameObjectFinder argGameObject)
    {
        argGameObject.result.transform.position += new Vector3(0, 5, 0);
    }

    [ConVar("test_array", "Another test of ConVar array implementation")]
    static ConVar cvar_test_array = new ConVar(new float[] { 5, 10, 15, 20, 21, 23 })
    {
        DataChanged = (object oldData, object newData) =>
        {
            Debug.Log("test_array has datachanged. old data: " + oldData + " | new data: " + newData);
        }
    };

    [ConCommand("server_listconnections", "List all connected clients.", ConFlags.ServerOnly)]
    static void cmd_server_listconnections()
    {
        string connectionsString = "";

        foreach (KeyValuePair<int, Mirror.NetworkConnectionToClient> connection in Mirror.NetworkServer.connections)
        {
            connectionsString += "(" + connection.Key + ") " + connection.Value.address + "\n";
        }

        //remove trailing newline if there are any connections
        if (connectionsString.Length > 0)
        {
            connectionsString = connectionsString.Substring(0, connectionsString.Length - 1);
        }

        DebugCore.FeedEntry("List of connected players: ", connectionsString, FeedEntryType.Info);
    }

    [ConCommand("test_rpcbroadcaster", ConFlags.ServerOnly | ConFlags.ClientReplicated)]
    static void cmd_test_rpcbroadcaster(string argBroadcast)
    {
        DebugCore.FeedEntry(argBroadcast, FeedEntryType.Info);
    }

    [ConCommand("test_rpctoserver", ConFlags.RunOnServer)]
    static void cmd_test_rpctoserver(string argBroadcast)
    {
        DebugCore.FeedEntry(argBroadcast);
    }

    [ConVar("test_repvar", "Replicated variable test", ConFlags.ClientReplicated | ConFlags.ServerOnly)]
    static ConVar cvar_test_repvar = new ConVar(3981.23f);

    [ConCommand("test_srvstatus")]
    static void cmd_test_srvstatus()
    {
        //Mirror.NetworkServer.
    }

    [ConCommand("test_localplayer")]
    static void cmd_test_localplayer()
    {
        Debug.Log("LocalPlayer is " + (Mirror.NetworkClient.localPlayer != null ? "not null" : "null"));
    }

    [ConCommand("test_cubethrow", ConFlags.RunOnServer | ConFlags.Cheat)]
    static void cmd_test_cubethrow(float argStrength)
    {
        if (GameObject.Find("throwcube") == null)
        {
            DebugCore.FeedEntry("Couldn't find a valid cube to throw.", "No GameObject named 'throwcube' could be found.", FeedEntryType.Warning);
        }

        GameObject.Find("throwcube").GetComponent<Rigidbody>().AddForce(new Vector3(DebugCoreUtil.RandomFloatInRange(-0.4f, 0.4f), argStrength, DebugCoreUtil.RandomFloatInRange(-0.4f, 0.4f)), ForceMode.Impulse);
    }

    [ConCommand("test_combine", "Combines both players, Authority-style.", ConFlags.RunOnServer | ConFlags.Cheat)]
    static void cmd_test_combine()
    {
        if (GameObject.Find("Bottom Half Player(Clone)") == null)
        {
            DebugCore.FeedEntry("Couldn't find the bottom half.", "GameObject.Find was null.", FeedEntryType.Error);
        }

        GameObject.Find("Bottom Half Player(Clone)").GetComponent<PlayerConnection>().Connect();
    }

    [ConCommand("test_clactive")]
    static void cmd_test_clactive()
    {
        Debug.Log("NetworkClient is " + (Mirror.NetworkClient.active ? "active" : "inactive"));
    }
}
