using System.Collections;
using System.Collections.Generic;
using System;

//DUMMY network module
public class AscalonEmptyNet : AscalonNetModule
{

    public override void Initialize()
    {
        base.Initialize();
    }

    public void InitializeNet()
    {
        Ascalon.Log("Ascalon running offline");
    }


    public override void NetCall(string argCall, AscalonCallContext argContext, AscalonCallNetTarget argTarget)
    {
        
    }

    public override void SendReplicatedToAllClients(string argCall, AscalonCallContext argContext)
    {
        
    }


    public IEnumerator DelayedReplication(string argCall, AscalonCallContext argContext)
    {
        yield return 0;
    }

    public override void SendAllReplicatedToClient(AscalonCallNetTarget argTarget)
    {
        
    }

    public override void ReceiveClientInfo(object argData)
    {
        
    }
}
