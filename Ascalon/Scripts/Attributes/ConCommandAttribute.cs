using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;

public class ConCommandAttribute : System.Attribute
{
    //basic data
    public string cmdName;
    public string cmdDescription;
    public ConFlags cmdFlags;









    //constructors
    public ConCommandAttribute(string argName, string argDescription)
    {
        cmdName = argName;
        cmdDescription = argDescription;
    }

    public ConCommandAttribute(string argName)
    {
        cmdName = argName;
        cmdDescription = "No description available.";
    }

    public ConCommandAttribute(string argName, string argDescription, ConFlags argFlags)
    {
        cmdName = argName;
        cmdDescription = argDescription;
        cmdFlags = argFlags;
    }

    public ConCommandAttribute(string argName, ConFlags argFlags)
    {
        cmdName = argName;
        cmdDescription = "No description available.";
        cmdFlags = argFlags;
    }
}