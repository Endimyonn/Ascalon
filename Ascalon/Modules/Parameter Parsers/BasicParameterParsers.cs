using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//This class defines several functions for command parameter
//parsing. It includes handlers for many built-in datatypes.

//This class is not meant to have a need to be modified, but
//it may be modified.
public class BasicParameterParsers
{
    [AscalonParameterParser("System.Boolean")]
    public static AscalonParameterParseResult ParseBoolean(string argParameter, int argIndex)
    {
        AscalonParameterParseResult result = new AscalonParameterParseResult();

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

    [AscalonParameterParser("System.Boolean[]")]
    public static AscalonParameterParseResult ParseBooleanArray(string argParameter, int argIndex)
    {
        AscalonParameterParseResult result = new AscalonParameterParseResult();

        List<string> data = argParameter.Trim().Split('|').ToList();

        List<bool> resultData = new List<bool>();

        foreach (string arrayObject in data)
        {
            if (arrayObject == "")
            {
                continue;
            }

            //remove extra spaces
            string trimmedObject = arrayObject.Trim();

            if (trimmedObject == "1" || trimmedObject.ToLower() == "true" || trimmedObject.ToLower() == "yes")
            {
                resultData.Add(true);
            }
            else if (trimmedObject == "0" || trimmedObject.ToLower() == "false" || trimmedObject.ToLower() == "no")
            {
                resultData.Add(false);
            }
            else
            {
                result.success = false;
                result.failureReason = InputParmValidationFailureReason.DataTypeMismatch;
                result.mismatchedParms.Add(new Tuple<string, string, int>(argParameter, "System.Boolean", argIndex));
            }
        }

        return result;
    }

    [AscalonParameterParser("System.Single")]
    public static AscalonParameterParseResult ParseFloat(string argParameter, int argIndex)
    {
        AscalonParameterParseResult result = new AscalonParameterParseResult();

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

    [AscalonParameterParser("System.Single[]")]
    public static AscalonParameterParseResult ParseFloatArray(string argParameter, int argIndex)
    {
        AscalonParameterParseResult result = new AscalonParameterParseResult();

        List<float> parseFloatArray = new List<float>();
        string[] data = argParameter.Trim().Split('|');

        foreach (string arrayObject in data)
        {
            if (arrayObject == "")
            {
                continue;
            }

            //remove extra spaces
            string trimmedObject = arrayObject.Trim();

            float parseFloatCandidate;
            bool parseParm;
            parseParm = float.TryParse(trimmedObject, out parseFloatCandidate);

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

    [AscalonParameterParser("System.Int32")]
    public static AscalonParameterParseResult ParseInt32(string argParameter, int argIndex)
    {
        AscalonParameterParseResult result = new AscalonParameterParseResult();

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

    [AscalonParameterParser("System.Int32[]")]
    public static AscalonParameterParseResult ParseInt32Array(string argParameter, int argIndex)
    {
        AscalonParameterParseResult result = new AscalonParameterParseResult();

        List<int> parseIntArray = new List<int>();
        string[] data = argParameter.Trim().Split('|');

        foreach (string arrayObject in data)
        {
            if (arrayObject == "")
            {
                continue;
            }

            //remove extra spaces
            string trimmedObject = arrayObject.Trim();

            int parseIntCandidate;
            bool parseParm;
            parseParm = int.TryParse(trimmedObject, out parseIntCandidate);

            if (parseParm == true)
            {
                parseIntArray.Add(parseIntCandidate);
            }
            else
            {
                result.success = false;
                result.failureReason = InputParmValidationFailureReason.DataTypeMismatch;
                result.mismatchedParms.Add(new Tuple<string, string, int>(argParameter, "System.Int32[]", argIndex));
            }
        }

        result.result = parseIntArray.ToArray();

        return result;
    }

    [AscalonParameterParser("System.Double")]
    public static AscalonParameterParseResult ParseDouble(string argParameter, int argIndex)
    {
        AscalonParameterParseResult result = new AscalonParameterParseResult();

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

    [AscalonParameterParser("System.Double[]")]
    public static AscalonParameterParseResult ParseDoubleArray(string argParameter, int argIndex)
    {
        AscalonParameterParseResult result = new AscalonParameterParseResult();

        List<double> parseDoubleArray = new List<double>();
        string[] data = argParameter.Trim().Split('|');

        foreach (string arrayObject in data)
        {
            if (arrayObject == "")
            {
                continue;
            }

            //remove extra spaces
            string trimmedObject = arrayObject.Trim();

            double parseDoubleCandidate;
            bool parseParm;
            parseParm = double.TryParse(trimmedObject, out parseDoubleCandidate);

            if (parseParm == true)
            {
                parseDoubleArray.Add(parseDoubleCandidate);
            }
            else
            {
                result.success = false;
                result.failureReason = InputParmValidationFailureReason.DataTypeMismatch;
                result.mismatchedParms.Add(new Tuple<string, string, int>(argParameter, "System.Double[]", argIndex));
            }
        }

        result.result = parseDoubleArray.ToArray();

        return result;
    }

    [AscalonParameterParser("System.String")]
    public static AscalonParameterParseResult ParseString(string argParameter, int argIndex)
    {
        AscalonParameterParseResult result = new AscalonParameterParseResult();

        result.result = argParameter;

        return result;
    }

    //todo: create test case
    [AscalonParameterParser("System.String[]")]
    public static AscalonParameterParseResult ParseStringArray(string argParameter, int argIndex)
    {
        AscalonParameterParseResult result = new AscalonParameterParseResult();

        List<string> data = argParameter.Split('|').ToList();

        List<string> correctedData = new List<string>();

        foreach (string arrayObject in data)
        {
            if (arrayObject != "")
            {
                correctedData.Add(arrayObject);
            }
        }

        result.result = correctedData.ToArray();

        return result;
    }
}
