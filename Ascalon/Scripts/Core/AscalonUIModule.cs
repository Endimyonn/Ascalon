using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The UI module provides a front-end for the developer and users to work with Ascalon - this can be through
/// sending Calls to it and/or receiving output from it. It also provides the basic functionality for logging.
/// 
/// This module is not necessary for Ascalon to function.
/// </summary>
[System.Serializable]
public abstract class AscalonUIModule
{
    //singleton
    public static AscalonUIModule instance;

    public virtual void Initialize()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;
    }

    public virtual void Update()
    {

    }

    //gets run after a call is completed - for instance, this can be used to clear an input field. optional
    public virtual void OnCallComplete(string argCallString, bool argSuccess, AscalonCallContext argContext)
    {
        
    }

    //create a feed entry - optional but recommended
    public virtual void Log(string argTitle, string argContent, LogMode argType)
    {
        
    }

    //clear all feed entries - optional
    public virtual void ClearEntries()
    {
        
    }
}


public enum LogMode
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