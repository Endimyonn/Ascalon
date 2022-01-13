using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The debug UI module provides a front-end for the developer
//and users to work with the DebugCore - this can be through
//sending Calls to it and/or receiving output from it.

//This module is not necessary for DebugCore to function.
public class DebugUIModule : MonoBehaviour
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
        //stub, optional
    }

    public virtual void LateUpdate()
    {
        //stub, optional
    }

    public virtual void OnCallComplete(string argCallString, bool argSuccess, DebugCallSource argSource)
    {
        //stub, optional
    }

    public virtual void FeedEntry(string argTitle, string argContent, FeedEntryType argType)
    {
        //stub, optional (recommended)
    }

    public virtual void ClearEntries()
    {
        //stub, optional
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