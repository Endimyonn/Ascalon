using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The debug UI module provides a front-end for the developer
//and users to work with the DebugCore - this can be through
//sending Calls to it and/or receiving output from it.

//This module is not necessary for DebugCore to function.
public abstract class DebugUIModule : MonoBehaviour
{
    //singleton
    public static DebugUIModule instance;

    public virtual void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        instance = this;
    }

    public virtual void Update()
    {
        
    }

    public virtual void LateUpdate()
    {
        
    }

    //gets run after a call is completed - for instance, this can be used to clear an input field. optional
    public virtual void OnCallComplete(string argCallString, bool argSuccess, DebugCallContext argContext)
    {
        
    }

    //create a feed entry - optional but recommended
    public virtual void FeedEntry(string argTitle, string argContent, FeedEntryType argType)
    {
        
    }

    //clear all feed entries - optional
    public virtual void ClearEntries()
    {
        
    }
}


public enum FeedEntryType
{
    Info,
    Warning,
    Error,
    Assertion,
    Exception,

    InfoVerbose,
    WarningVerbose,
    ErrorVerbose,
    AssertionVerbose,
    ExceptionVerbose
}