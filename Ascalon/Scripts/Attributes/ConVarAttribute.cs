using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

public class ConVarAttribute : System.Attribute
{
    //basic data
    public string cvarName;
    public string cvarDescription;
    public ConFlags cvarFlags;

    

    //constructors
    public ConVarAttribute(string argName, string argDescription)
    {
        cvarName = argName;
        cvarDescription = argDescription;
    }

    public ConVarAttribute(string argName)
    {
        cvarName = argName;
        cvarDescription = "No description available.";
    }

    public ConVarAttribute(string argName, string argDescription, ConFlags argFlags)
    {
        cvarName = argName;
        cvarDescription = argDescription;
        cvarFlags = argFlags;
    }

    public ConVarAttribute(string argName, ConFlags argFlags)
    {
        cvarName = argName;
        cvarDescription = "No description available.";
        cvarFlags = argFlags;
    }
}
