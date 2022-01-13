using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attribute class for debug parameter parsers. All custom parsers
//should be tagged with this attribute, and initialized using their
//type-string.
public class DebugParameterParserAttribute : System.Attribute
{
    public string typeName;

    public DebugParameterParserAttribute(string argTypeName)
    {
        typeName = argTypeName;
    }
}
