using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class defines several functions for command parameter
//parsing. It includes handlers for many built-in datatypes.

//This class is not meant to have a need to be modified, but
//it may be modified.
public class BasicParameterParsers
{
    //todo: array version
    [DebugParameterParser("System.Boolean")]
    public static DebugParameterParseResult ParseBoolean(string argParameter, int argIndex)
    {
        DebugParameterParseResult result = new DebugParameterParseResult();

        if (argParameter == "1" || argParameter.ToLower() == "true" || argParameter.ToLower() == "yes")
        {
            result.result = true;
        }
        else if (argParameter == "0" || argParameter.ToLower() == "false" || argParameter.ToLower() == "no")
        {
            result.result = false;
        }
        else
        {
            result.failureReason = InputParmValidationFailureReason.DataTypeMismatch;
            result.mismatchedParms.Add(new Tuple<string, string, int>(argParameter, "System.Boolean", argIndex));
        }

        return result;
    }

    [DebugParameterParser("System.Single")]
    public static DebugParameterParseResult ParseFloat(string argParameter, int argIndex)
    {
        DebugParameterParseResult result = new DebugParameterParseResult();

        float parseFloat;
        bool parseParm;
        parseParm = float.TryParse(argParameter, out parseFloat);

        if (parseParm == true)
        {
            result.result = parseFloat;
        }
        else
        {
            result.success = false;
            result.failureReason = InputParmValidationFailureReason.DataTypeMismatch;
            result.mismatchedParms.Add(new Tuple<string, string, int>(argParameter, "System.Single", argIndex));
        }

        return result;
    }

    //todo: handle "3 | " without errors
    [DebugParameterParser("System.Single[]")]
    public static DebugParameterParseResult ParseFloatArray(string argParameter, int argIndex)
    {
        DebugParameterParseResult result = new DebugParameterParseResult();

        List<float> parseFloatArray = new List<float>();
        string[] data = argParameter.Split('|');

        foreach (string arrayObject in data)
        {
            float parseFloatCandidate;
            bool parseParm;
            parseParm = float.TryParse(arrayObject, out parseFloatCandidate);

            if (parseParm == true)
            {
                parseFloatArray.Add(parseFloatCandidate);
            }
            else
            {
                result.success = false;
                result.failureReason = InputParmValidationFailureReason.DataTypeMismatch;
                result.mismatchedParms.Add(new Tuple<string, string, int>(argParameter, "System.Single[]", argIndex));
            }
        }

        result.result = parseFloatArray.ToArray();

        return result;
    }

    //todo: array version
    [DebugParameterParser("System.Int32")]
    public static DebugParameterParseResult ParseInt32(string argParameter, int argIndex)
    {
        DebugParameterParseResult result = new DebugParameterParseResult();

        int parseInt;
        bool parseParm;
        parseParm = int.TryParse(argParameter, out parseInt);

        if (parseParm == true)
        {
            result.result = parseInt;
        }
        else
        {
            //check if we can convert it to a float and then an integer - eg 3.00
            float parseIntFloat;
            parseParm = float.TryParse(argParameter, out parseIntFloat);

            if (parseParm == true && (Mathf.Floor(parseIntFloat) == parseIntFloat)) //only do this if the decimal value is 0
            {
                result.result = (int)parseIntFloat;
            }
            else
            {
                result.success = false;
                result.failureReason = InputParmValidationFailureReason.DataTypeMismatch;
                result.mismatchedParms.Add(new Tuple<string, string, int>(argParameter, "System.Int32", argIndex));
            }
        }

        return result;
    }

    //todo: array version
    [DebugParameterParser("System.Double")]
    public static DebugParameterParseResult ParseDouble(string argParameter, int argIndex)
    {
        DebugParameterParseResult result = new DebugParameterParseResult();

        double parseDouble;
        bool parseParm;
        parseParm = double.TryParse(argParameter, out parseDouble);

        if (parseParm == true)
        {
            result.result = parseDouble;
        }
        else
        {
            result.success = false;
            result.failureReason = InputParmValidationFailureReason.DataTypeMismatch;
            result.mismatchedParms.Add(new Tuple<string, string, int>(argParameter, "System.Double", argIndex));
        }

        return result;
    }

    [DebugParameterParser("System.String")]
    public static DebugParameterParseResult ParseString(string argParameter, int argIndex)
    {
        DebugParameterParseResult result = new DebugParameterParseResult();

        result.result = argParameter;

        return result;
    }

    //todo: create test case
    //todo: handle "afei | " without errors
    [DebugParameterParser("System.String[]")]
    public static DebugParameterParseResult ParseStringArray(string argParameter, int argIndex)
    {
        DebugParameterParseResult result = new DebugParameterParseResult();

        result.result = argParameter.Split('|');

        return result;
    }
}
