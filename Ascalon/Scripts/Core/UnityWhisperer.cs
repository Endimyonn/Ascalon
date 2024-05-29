#if UNITY_2019_1_OR_NEWER
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;
using UnityEngine;

//tool to direct functions to run on Unity's main thread
public class UnityWhisperer : MonoBehaviour
{
    //singleton
    public static UnityWhisperer instance;

    public static volatile bool queueHasActions = false;
    public static List<Action> backlog = new List<Action>();
    public static List<Action> actionQueue = new List<Action>();

    private void Awake()
    {
        //set up singleton
        if (instance != null)
        {
            Debug.Log("Duplicate UnityWhisperer was spawned, this is not allowed.");
            Destroy(this);
            return;
        }
        instance = this;
    }

    void Update()
    {
        if (queueHasActions)
        {
            //lock the backlog so it may not be modified while we are transferring actions
            lock (backlog)
            {
                //transfer the backlog to the active queue, then empty the backlog
                actionQueue.AddRange(backlog);
                backlog.Clear();

                queueHasActions = false;
            }

            //execute the queue
            foreach (Action action in actionQueue)
            {
                action();
            }

            actionQueue.Clear();
        }
    }



    public static void Run(Action argAction)
    {
        //make sure we have been set up properly
        if (instance == null)
        {
            Debug.Log("Cannot use UnityWhisperer as it has not been added to any GameObject in the scene!");
            return;
        }

        lock (backlog)
        {
            backlog.Add(argAction);
            queueHasActions = true;
        }
    }
}
#endif