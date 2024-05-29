#if GODOT
using System;
using System.Collections.Generic;
using Godot;

public partial class GodotWhisperer : Node
{
    //singleton
    public static GodotWhisperer instance;


    private static volatile bool queueHasActions = false;
    private static List<Action> backlog = new List<Action>();
    private static List<Action> actionQueue = new List<Action>();



    public override void _Ready()
    {
        if (instance != null)
        {
            GD.PrintErr("GodotWhisperer was initialized twice. This is not allowed.");
            this.QueueFree();
            return;
        }
        instance = this;
    }

    public override void _Process(double delta)
    {
        if (queueHasActions == true)
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
            Ascalon.Log("Cannot use UnityWhisperer as it has not been added to any GameObject in the scene!");
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
