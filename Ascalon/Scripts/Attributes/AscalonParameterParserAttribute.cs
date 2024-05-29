using System.Collections;
using System.Collections.Generic;

//Attribute class for debug parameter parsers. All custom parsers
//should be tagged with this attribute, and initialized using their
//type-string.
public class AscalonParameterParserAttribute : System.Attribute
{
    public string typeName;

    public AscalonParameterParserAttribute(string argTypeName)
    {
        typeName = argTypeName;
    }
}
