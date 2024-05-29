using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

public class ConVar
{
    public System.Type cvarDataType;    //should be one or more System.Type
    private object cvarData;            //can be an array

    //attribute info
    public string name;
    public string description;
    public ConFlags flags;




    public virtual object GetData()
    {
        return cvarData;
    }

    public virtual void SetData(object[] argData)
    {
        object oldData = cvarData;
        cvarData = argData[0];

        //run callback if it exists
        if (DataChanged != null)
        {
            DataChanged(oldData, argData[0]);
        }
    }

    public virtual void SetDataSimple(object[] argData)
    {
        cvarData = argData[0];
    }

    //function that is called when data is changed
    public System.Action<object, object> DataChanged { get; set; }



    //constructors
    public ConVar(System.Type argDataType, object argDefaultCvarData)
    {
        cvarDataType = argDataType;

        cvarData = argDefaultCvarData;
    }

    public ConVar(System.Type argDataType)
    {
        cvarDataType = argDataType;
    }

    public ConVar(object argDefaultCvarData)
    {
        cvarDataType = argDefaultCvarData.GetType();
        cvarData = argDefaultCvarData;
    }



    public void Initialize(string argName, string argDescription, ConFlags argFlags)
    {
        name = argName;
        description = argDescription;
        flags = argFlags;
    }
}
