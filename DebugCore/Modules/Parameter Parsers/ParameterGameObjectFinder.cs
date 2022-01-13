using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParameterGameObjectFinder
{
    [DebugParameterParser("GameObjectFinder")]
    public static DebugParameterParseResult ParseGameObjectFinder(string argParameter, int argIndex)
    {
        DebugParameterParseResult result = new DebugParameterParseResult();

        if (argParameter.ToLower() == "mouseposition" || argParameter.ToLower() == "mousepos" || argParameter.ToLower() == "mpos")
        {
            GameObjectFinder finder = new GameObjectFinder(GameObjectFinderTargeting.MousePosition);
            if (finder.result != null)
            {
                result.result = finder;
            }
            else
            {
                result.success = false;
                result.failureReason = InputParmValidationFailureReason.Other;
                result.customErrorMessage = "No object could be found at the mouse position.";
            }
        }
        else if (argParameter.ToLower() == "forwardcast" || argParameter.ToLower() == "fwdcast")
        {
            //NYI
            result.success = false;
            result.failureReason = InputParmValidationFailureReason.Other;
            result.customErrorMessage = "forwardcast is not yet implemented.";
        }
        else if (argParameter.Length >= 10 && argParameter.Substring(0, 10).ToLower() == "objectname") //todo: better system for reading object names with quotes
        {
            if (argParameter.Length > 11)
            {
                GameObjectFinder finder = new GameObjectFinder(GameObjectFinderTargeting.ObjectName, argParameter.Substring(11));   //search for any object with the specified name
                if (finder.result != null)
                {
                    result.result = finder;
                }
                else //no object with the provided name could be found
                {
                    result.success = false;
                    result.failureReason = InputParmValidationFailureReason.Other;
                    result.customErrorMessage = "No object could be found with the given name.";
                }
            }
            else //no object name to search for was provided
            {
                result.success = false;
                result.failureReason = InputParmValidationFailureReason.Other;
                result.customErrorMessage = "No object name provided.";
            }
        }
        else //no valid descriptor of GameObjectFinderType found, so it's a type mismatch
        {
            result.success = false;
            result.failureReason = InputParmValidationFailureReason.DataTypeMismatch;
            result.mismatchedParms.Add(new Tuple<string, string, int>(argParameter, "GameObjectFinder", argIndex));
        }

        return result;
    }
}
