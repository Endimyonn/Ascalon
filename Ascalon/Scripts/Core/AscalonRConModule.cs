using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

//Unity does not support System.Text.Json by default, so we use
//Unity's substitute, JsonUtility, if we're running under Unity.
#if UNITY_2019_1_OR_NEWER
using UnityEngine;
#else
using System.Text.Json;
using System.Text.Json.Serialization;
#endif

//the RCon module allows commands to be received from any client
//built to support it over the network
public class AscalonRConModule
{
    [ConVar("rcon_password", "RCon password", ConFlags.Sensitive)]
    static ConVar cvar_rcon_password = new ConVar("");

    [ConVar("rcon_address", "The address to send RCon requests to")]
    static ConVar cvar_rcon_address = new ConVar("");

    [ConVar("rcon_port", "The port to send RCon requests to")]
    static ConVar cvar_rcon_port = new ConVar(7701);

    [ConCommand("rcon", "Send an RCon command")]
    static void cmd_rcon(string argCall)
    {
        //create TcpClient, try to establish connection to remote
        TcpClient client = new TcpClient();
        try
        {
            client = new TcpClient((string)Ascalon.GetConVar("rcon_address"), (int)Ascalon.GetConVar("rcon_port"));
        }
        catch (Exception issue)
        {
            Ascalon.Log("Error while establishing RCon connection: " + issue.Message, LogMode.Error);
        }

        NetworkStream stream = client.GetStream();

        //compose and send the data
        byte[] callPacket = CreatePacket(new AscalonRConData(argCall, (string)Ascalon.GetConVar("rcon_password")));
        stream.Write(callPacket, 0, callPacket.Length);

        //todo: receive issues from server back

        client.Close();
    }

    //Method for serializing an RCon command to a packet
    public static byte[] CreatePacket(AscalonRConData argData)
    {
        #if UNITY_2019_1_OR_NEWER
        string jsonConv = JsonUtility.ToJson(argData);
        byte[] packet = Encoding.ASCII.GetBytes(JsonUtility.ToJson(argData));
        #else
        byte[] packet = JsonSerializer.SerializeToUtf8Bytes(argData);
        #endif

        return packet;
    }

    //Method for deserializing a received RCon packet back to a command
    public static AscalonRConData DecodePacket(byte[] argPacket)
    {
        #if UNITY_2019_1_OR_NEWER
        return JsonUtility.FromJson<AscalonRConData>(Encoding.ASCII.GetString(argPacket));
        #else
        Utf8JsonReader reader = new Utf8JsonReader(argPacket);
        return JsonSerializer.Deserialize<AscalonRConData>(ref reader);
        #endif
    }

    //Method to send RCon call
    public static void Send(string argAddress, ushort argPort, string argCall, string argPassword)
    {
        
    }
}

[System.Serializable]
public class AscalonRConData
{
    //Unity's JsonUtility doesn't like the getset part, so we get rid of it
    //if running under Unity (System.Text.Json) requires it
    #if UNITY_2019_1_OR_NEWER
    public string call;
    public string password;
    #else
    public string call { get; set; }
    public string password { get; set; }
    #endif

    public AscalonRConData(string argCall, string argPassword)
    {
        call = argCall;
        password = argPassword;
    }

    public AscalonRConData(string argCall)
    {
        call = argCall;
        password = "";
    }

    public AscalonRConData()
    {
        call = "";
        password = "";
    }
}