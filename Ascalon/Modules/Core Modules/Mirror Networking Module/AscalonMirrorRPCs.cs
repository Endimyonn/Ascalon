using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

//contains the RPCs for AscalonMirrorNet
public class AscalonMirrorRPCs : NetworkBehaviour
{
    [Command]
    public void RpcNetCall(string argCall, AscalonCallContext argContext, AscalonCallNetTarget argTarget)
    {
        NetworkConnection targetConnection = null;

        if (argTarget.targetMode == NetRole.Server)
        {
            //we are the server running a server call, just run it
            if (AscalonNetModule.GetRole() == NetRole.Server)
            {
                Ascalon.Call(argCall, argContext);
            }
            else
            {
                if (argTarget.targetMode == NetRole.Server)
                {
                    targetConnection = NetworkClient.localPlayer.connectionToClient;
                }
                else if (argTarget.targetMode == NetRole.Client)
                {
                    //untested, but should work
                    targetConnection = GameObject.Find(argTarget.targetClient).GetComponent<NetworkIdentity>().connectionToClient;
                }
            }
        }


        RpcNetCallInternal(targetConnection, argCall, argContext);
    }

    //todo: prevent call to server from being called back on client too
    [TargetRpc]
    private void RpcNetCallInternal(NetworkConnection argTarget, string argCall, AscalonCallContext argContext)
    {
        Ascalon.Call(argCall, argContext);
    }

    public void RpcReplicateToClient(NetworkConnection argClient, string argCall, AscalonCallContext argContext)
    {
        RpcNetCallInternal(argClient, argCall, argContext);
    }

    [ClientRpc]
    public void RpcNetCallAllClientsOld(string argCall, AscalonCallContext argContext)
    {
        Debug.Log("rpc hit!!");
        Ascalon.Call(argCall, argContext);
    }
}
