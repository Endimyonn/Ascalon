using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//AscalonNetModule implementation for the Mirror library. As Mirror
//requires a class derived from NetworkBehaviour for RPCs, those
//functions have been placed in AscalonMirrorSegment. This class
//also provides an example of how to implement networking.
public class AscalonMirrorNet : AscalonNetModule
{
    [SerializeField] public AscalonMirrorRPCs debugRPCs;

    public override void Initialize()
    {
        base.Initialize();
    }

    public void InitializeNet()
    {
        //if the network isn't fully initialized, do so
        if (debugRPCs == null)
        {
            debugRPCs = NetworkClient.localPlayer.gameObject.GetComponent<AscalonMirrorRPCs>();
        }

        if (debugRPCs == null)
        {
            Ascalon.Log("AscalonMirrorNet could not initialize", LogMode.Error);
        }
    }


    public override void NetCall(string argCall, AscalonCallContext argContext, AscalonCallNetTarget argTarget)
    {
        if (instance.role == NetRole.Inactive)
        {
            return;
        }

        InitializeNet();

        debugRPCs.RpcNetCall(argCall, argContext, argTarget);
    }

    public override void SendReplicatedToAllClients(string argCall, AscalonCallContext argContext)
    {
        //security
        if (GetRole() != NetRole.Server)
        {
            return;
        }

        //in the event we are replicating before the local server player has been spawned,
        //wait a frame and then proceed as the RPCs cannot be used before then
        if (NetworkClient.localPlayer == null)
        {
            DelayedReplication(argCall, argContext);
            return;
        }

        InitializeNet();

        //loop through every connected client
        foreach (KeyValuePair<int, Mirror.NetworkConnectionToClient> connection in Mirror.NetworkServer.connections)
        {
            //check to make sure the client is not also the server
            if (connection.Key != NetworkClient.connection.connectionId)
            {
                debugRPCs.RpcReplicateToClient(connection.Value, argCall, argContext);
            }
        }
    }


    public IEnumerator DelayedReplication(string argCall, AscalonCallContext argContext)
    {
        yield return new WaitForEndOfFrame();

        InitializeNet();

        //loop through every connected client
        foreach (KeyValuePair<int, Mirror.NetworkConnectionToClient> connection in Mirror.NetworkServer.connections)
        {
            //check to make sure the client is not also the server
            if (connection.Key != NetworkClient.connection.connectionId)
            {
                debugRPCs.RpcReplicateToClient(connection.Value, argCall, argContext);
            }
        }
    }

    public override void SendAllReplicatedToClient(AscalonCallNetTarget argTarget)
    {
        if (AscalonNetModule.GetRole() == NetRole.Client)
        {
            return;
        }

        InitializeNet();

        foreach (ConVar conVar in Ascalon.instance.conVars)
        {
            if (conVar.flags.HasFlag(ConFlags.ClientReplicated))
            {
                debugRPCs.RpcReplicateToClient(
                    GameObject.Find(argTarget.targetClient).GetComponent<NetworkIdentity>().connectionToClient,
                    conVar.name + " " + AscalonUtil.ConVarDataToString(conVar.GetData()),
                    new AscalonCallContext(AscalonCallSource.Server)
                    );
            }
        }
    }

    //a NetworkManager derivative should call this on every joining client using OnServerConnect()
    public void SendAllReplicatedToClient(NetworkConnection argClient)
    {
        if (AscalonNetModule.GetRole() != NetRole.Server)
        {
            return;
        }

        InitializeNet();

        //todo: fix
        foreach (ConVar conVar in Ascalon.instance.conVars)
        {
            if (conVar.flags.HasFlag(ConFlags.ClientReplicated))
            {
                //do not replicate to the server
                if (argClient.connectionId != NetworkClient.connection.connectionId)
                {
                    debugRPCs.RpcReplicateToClient(argClient, conVar.name + " " + AscalonUtil.ConVarDataToString(conVar.GetData()), new AscalonCallContext(AscalonCallSource.Server));
                }
            }
        }
    }

    public override void ReceiveClientInfo(object argData)
    {
        //NYI
    }
}