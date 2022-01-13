using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

//todo: rewrite this after stock modules are done

//DebugCore is a tool that implements two major components: a universal command
//and variable system, and a front-end console to interact with it easily. Commands
//and ConVars can be defined from anywhere in the project, and are very flexible
//in operation. It is designed in such a manner as to be easily implemented into
//any project with little to no modification.

//By design, the console front-end is made to be enabled and visibleby default. the
//implementation of toggling is done on a per-project basis, and no input systems
//aside from Unity's own EventSystem (for text entry frontend) are used.

//For examples of how to implement commands and ConVars into code, see BaseCommands.cs.
public partial class DebugCore : MonoBehaviour
{
    //singleton
    public static DebugCore instance;

    //UI module
    public DebugUIModule debugUI;

    //networking module
    public DebugNetModule debugNet;


    //delegates
    public delegate void OnCallCompleted(string argCallString, bool argSuccess, DebugCallSource argSource);
    public OnCallCompleted onCallCompleted;
    


    public bool validateCommandAddition; //NYI, for optional better handling of command/cvar definition conflict prevention



    public List<KeyValuePair<string, MethodInfo>> parameterParsers = new List<KeyValuePair<string, MethodInfo>>();








    public List<ConCommand> conCommands = new List<ConCommand>();  //master list of commands - see relevant struct
    public List<ConVar> conVars = new List<ConVar>();

    private void Awake()
    {
        //singleton logic
        if (instance)
        {
            Destroy(transform.root.gameObject);
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(transform.root.gameObject);







        //System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        //stopwatch.Start();

        //command and convar gathering
        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
        {
            //collect commands
            foreach (MethodInfo foundMethod in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Default))
            {
                if (foundMethod.GetCustomAttributes(typeof(ConCommandAttribute), false).Length > 0)
                {
                    //isolate the attribute information, since it stores metadata about the command
                    ConCommandAttribute foundAttribute = (ConCommandAttribute)foundMethod.GetCustomAttribute(typeof(ConCommandAttribute));

                    //create a new ConCommand and add it to the master list
                    conCommands.Add(new ConCommand(foundAttribute.cmdName, foundAttribute.cmdDescription, foundMethod, foundAttribute.cmdFlags));
                }
                else if (foundMethod.GetCustomAttributes(typeof(DebugParameterParserAttribute), false).Length > 0)
                {
                    DebugParameterParserAttribute foundAttribute = (DebugParameterParserAttribute)foundMethod.GetCustomAttribute(typeof(DebugParameterParserAttribute));
                    parameterParsers.Add(new KeyValuePair<string, MethodInfo>(foundAttribute.typeName, foundMethod));
                }
            }

            //collect convars
            foreach (FieldInfo foundField in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Default))
            {
                if (foundField.GetCustomAttributes(typeof(ConVarAttribute), false).Length > 0)
                {
                    //isolate the attribute information
                    ConVarAttribute foundAttribute = (ConVarAttribute)foundField.GetCustomAttribute(typeof(ConVarAttribute));

                    //initialize the ConVar and add it to the master list
                    ((ConVar)foundField.GetValue(null)).Initialize(foundAttribute.cvarName, foundAttribute.cvarDescription, foundAttribute.cvarFlags);
                    conVars.Add((ConVar)foundField.GetValue(null));
                }
                
            }
        }

        /*stopwatch.Stop();
        TimeSpan gatherTime = stopwatch.Elapsed;

        Debug.Log("Data gather read took " + String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            gatherTime.Hours,
            gatherTime.Minutes,
            gatherTime.Seconds,
            gatherTime.Milliseconds / 10));*/
    }

    private void Update()
    {
        
    }

    private void LateUpdate()
    {
        
    }


    






    public static void Call(string argInput, DebugCallSource argSource)
    {
        //make sure we have a command to work with
        if (argInput == null || argInput == "")
        {
            return;
        }

        //remove beginning and trailing whitespace
        argInput.Trim();

        //process the command and split it into an array of terms
        object[] inputParms = instance.GetInputParms(argInput).ToArray();

        //isolate first statement
        string inputName = argInput.Split()[0].ToLower();


        //----input string processing completed----//

        
        //check if the command exists
        if (!instance.conCommands.Any(ConCommand => ConCommand.name == inputName) && !instance.conVars.Any(ConVarInstance => ConVarInstance.name == inputName))
        {
            FeedEntry("Error: no such command or ConVar '" + inputName.ToLower() + "'", "The command or ConVar entered does not exist.\nOriginal received string: " + argInput, FeedEntryType.Error);
            instance.onCallCompleted(argInput, false, argSource);
        }
        else
        {
            ConCommand findCommand = instance.conCommands.AsParallel().FirstOrDefault(ConCommand => ConCommand.name == inputName);
            ConVar findConVar = instance.conVars.AsParallel().FirstOrDefault(ConVar => ConVar.name == inputName);

            //container for call parameter parsing and validation
            InputParmValidation inputValidity = new InputParmValidation();



            //identify what we are working with
            int workingWith = 0;
            MiscConObject findConObject = new MiscConObject();
            if (findCommand.name != null)
            {
                workingWith = 1;
                findConObject.conObject = findCommand;
                findConObject.flags = findCommand.flags;

                inputValidity = instance.ValidateInputParms(inputParms, DebugCoreUtil.ParmsToStrings(findCommand.parms));
            }
            else if (findConVar.name != null)
            {
                workingWith = 2;
                findConObject.conObject = findConVar;
                findConObject.flags = findConVar.flags;

                inputValidity = instance.ValidateInputParms(inputParms, DebugCoreUtil.ParmsToStrings(findConVar.cvarDataType));
            }

            //check ConFlags and context before proceeding
            bool flagsPassed = DebugCore.instance.ValidateFlags(findConObject.flags);
            bool contextPassed = DebugCore.instance.ValidateContext(argSource, findConObject.flags);



            //final logic tree
            if (inputValidity.valid)
            {
                if (flagsPassed && contextPassed)
                {
                    object[] finalParms = inputValidity.finalParms.ToArray();

                    if (workingWith == 1) //ConCommand
                    {
                        findCommand.method.Invoke(null, finalParms);

                        if (instance.onCallCompleted != null)
                        {
                            instance.onCallCompleted(argInput, true, argSource);
                        }
                    }
                    else if (workingWith == 2) //ConVar
                    {
                        findConVar.SetData(finalParms);

                        if (instance.onCallCompleted != null)
                        {
                            instance.onCallCompleted(argInput, true, argSource);
                        }
                    }
                }
                else
                {
                    instance.onCallCompleted(argInput, false, argSource);
                }
            }
            else //parameter(s) did not parse correctly
            {
                switch (inputValidity.failureReason)
                {
                    case InputParmValidationFailureReason.ParmCountMismatch:
                        DebugCore.FeedEntry("Error: incorrect number of arguments provided", "The number of arguments provided does not match the number expected for the command or ConVar.\nExpected: " + (findCommand.name != null ? findCommand.parms.Length : "1") + "\nProvided: " + inputParms.Length, FeedEntryType.Error);
                        break;

                    case InputParmValidationFailureReason.DataTypeMismatch:
                        string failedParms = "";
                        foreach (Tuple<string, string, int> failedParm in inputValidity.mismatchedParms)
                        {
                            failedParms += "Argument " + failedParm.Item3 + " (" + failedParm.Item1 + ") is not of type " + failedParm.Item2 + "\n";
                        }
                        DebugCore.FeedEntry("Error: argument types do not match", "The type(s) of the argument(s) provided for the specified command or ConVar do not match what is required by it and could not be converted. Failed arguments:\n" + failedParms.Remove(failedParms.Length - 1, 1), FeedEntryType.Error);
                        break;

                    case InputParmValidationFailureReason.NoParserForType:
                        string errorString = "Invalid arguments:\n";
                        foreach (Tuple<string, string, int> failedParm in inputValidity.mismatchedParms)
                        {
                            errorString += "Argument " + failedParm.Item3 + " (" + failedParm.Item2 + ")\n";
                        }
                        errorString = errorString.Remove(errorString.Length - 1);

                        DebugCore.FeedEntry("Error: no parser found for argument(s)", errorString, FeedEntryType.Error);
                        break;

                    case InputParmValidationFailureReason.Other:
                        DebugCore.FeedEntry("Error: issue occurred while parsing command arguments", inputValidity.customErrorMessage, FeedEntryType.Error);
                        break;
                }

                if (instance.onCallCompleted != null) //callback
                {
                    Debug.Log("hit");
                    instance.onCallCompleted(argInput, false, argSource);
                }
            }
        }
    }

    //if we receive a call without a specified source, assume it's internal
    public static void Call(string argString)
    {
        DebugCore.Call(argString, DebugCallSource.Internal);
    }

    List<string> GetInputParms(string argInput) //todo: array support formatted as {x|x}
    {
        List<string> returnList = new List<string>();
        CommandParmScrubState scrubState = CommandParmScrubState.Initial;
        string activeString = "";

        //loop through the command
        for (int i = 0; i < argInput.Length; i++)
        {
            if (scrubState == CommandParmScrubState.Normal && i == argInput.Length - 1 && activeString.Length > 0)
            {
                activeString += argInput[i];
                returnList.Add(activeString);
            }
            else if (scrubState == CommandParmScrubState.Initial)
            {
                if (char.IsWhiteSpace(argInput[i]))
                {
                    scrubState = CommandParmScrubState.Normal;
                }
            }
            else if (scrubState == CommandParmScrubState.Normal)
            {
                if (char.IsWhiteSpace((char)argInput[i]) && activeString.Length > 0)
                {
                    returnList.Add(activeString);
                    activeString = "";
                }
                else if (argInput[i] == '"')
                {
                    scrubState = CommandParmScrubState.String;
                }
                else if (char.IsWhiteSpace((char)argInput[i])) //prevent space before the next argument after a string argument
                {
                    continue;
                }
                else
                {
                    activeString += argInput[i];
                }
            }
            else if (scrubState == CommandParmScrubState.String)
            {
                if (argInput[i] == '"') //allow an empty string as an argument
                {
                    returnList.Add(activeString);
                    activeString = "";
                    scrubState = CommandParmScrubState.Normal;
                }
                else
                {
                    activeString += argInput[i];
                }
            }

            //prevents culling of 1-length arguments
            if (i == argInput.Length - 1 && activeString.Length == 1)
            {
                returnList.Add(activeString);
                activeString = "";
            }
        }

        return returnList;
    }

    InputParmValidation ValidateInputParms(object[] argInputParmsProvided, string[] argInputParms)
    {
        InputParmValidation result = new InputParmValidation(true);
        bool failed = false;

        //check if there are enough parameters provided for the command
        if (failed == false && argInputParmsProvided.Length != argInputParms.Length)
        {
            result.valid = false;
            result.failureReason = InputParmValidationFailureReason.ParmCountMismatch;
            failed = true;
        }

        //check if the paramaters provided have matching types with the command
        if (failed == false)
        {
            //try and parse the provided args to the target ones, and if it can't be done, validation has failed
            for (int i = 0; i < argInputParmsProvided.Length; i++)
            {
                //Object array of data to send to the parsing function. All parsing functions
                //should accept a string parameter and a parameter index.
                object[] providedParm = { argInputParmsProvided[i], i + 1 };

                bool parserExists = false;
                foreach (KeyValuePair<string, MethodInfo> parameterParser in parameterParsers)
                {
                    if (parameterParser.Key == argInputParms[i])
                    {
                        parserExists = true;
                    }
                }

                //return an error if no parser exists for the required type
                if (!parserExists)
                {
                    result.valid = false;
                    result.failureReason = InputParmValidationFailureReason.NoParserForType;
                    result.mismatchedParms.Add(new Tuple<string, string, int>("", argInputParms[i], i + 1));
                    failed = true;
                }
                
                DebugParameterParseResult parseResult = (DebugParameterParseResult)parameterParsers.Find(parser => parser.Key == argInputParms[i]).Value.Invoke(null, providedParm);

                if (parseResult.success)
                {
                    result.finalParms.Add(parseResult.result);
                }
                else
                {
                    result.failureReason = parseResult.failureReason;
                    foreach (Tuple<string, string, int> mismatchedParm in result.mismatchedParms)
                    {
                        result.mismatchedParms.Add(mismatchedParm);
                    }
                    failed = true;
                }
            }
        }

        //check if the command has passed all checks prior
        if (failed == false)
        {
            result.valid = true;
        }

        return result;
    }

    //validate flags on a call
    bool ValidateFlags(ConFlags argFlags)
    {
        if (argFlags.HasFlag(ConFlags.Cheat))
        {
            if ((bool)DebugCore.GetConVar("host_cheats") == false)
            {
                return false;
            }
        }

        if (argFlags.HasFlag(ConFlags.ServerOnly))
        {
            if (debugNet.isClient)
            {
                return false;
            }
        }

        if (argFlags.HasFlag(ConFlags.LockWhileConnected))
        {
            if (debugNet.sessionActive)
            {
                return false;
            }
        }

        return true;
    }

    //validate context on a call
    bool ValidateContext(DebugCallSource argContext, ConFlags argFlags)
    {
        if (argContext == DebugCallSource.Internal)
        {
            return true;
        }
        
        if (argContext == DebugCallSource.User)
        {
            return true;
        }

        if (argContext == DebugCallSource.Client)
        {
            if ((bool)DebugCore.GetConVar("client_allowclientcall") == true)
            {
                return true;
            }

            return false;
        }

        if (argContext == DebugCallSource.Server)
        {
            if (argFlags.HasFlag(ConFlags.ClientReplicated))
            {
                return true;
            }

            if ((bool)DebugCore.GetConVar("client_allowservercall") == true)
            {
                return true;
            }

            return false;
        }

        return false;
    }















    







    public static object GetConVar(string argConVarName) //todo: custom struct or tuple with bool and object for better nullreference handling
    {
        return DebugCore.instance.conVars.FirstOrDefault(ConVar => ConVar.name == argConVarName).GetData();
    }

    public static void SetConVar(string argConVarName, object argConVarData)
    {
        DebugCore.Call(argConVarName + " " + argConVarData.ToString());
    }

    public static ConCommand GetConCommandEntry(string argCommandName)
    {
        return DebugCore.instance.conCommands.FirstOrDefault(ConCommand => ConCommand.name == argCommandName);
    }

    public static ConVar GetConVarEntry(string argConVarName)
    {
        return DebugCore.instance.conVars.FirstOrDefault(ConVar => ConVar.name == argConVarName);
    }

    public static bool ConCommandExists(string argEntryName)
    {
        return DebugCore.instance.conCommands.Any(ConCommand => ConCommand.name == argEntryName);
    }

    public static bool ConVarExists(string argEntryName)
    {
        return DebugCore.instance.conVars.Any(ConVar => ConVar.name == argEntryName);
    }

    public static bool CommandOrConVarExists(string argEntryName)
    {
        return (ConCommandExists(argEntryName) == true || ConVarExists(argEntryName) == true);
    }









    //DebugUIModule extensions
    public static void FeedEntry(string argTitle, string argContent, FeedEntryType argType)
    {
        if (instance.debugUI == null)
        {
            Console.WriteLine("FeedEntry > " + argTitle + "\nContent   > " + argContent + "\nType      > " + argType.ToString());
            return;
        }

        instance.debugUI.FeedEntry(argTitle, argContent, argType);
    }

    public static void FeedEntry(string argTitle, FeedEntryType argType)
    {
        if (instance.debugUI == null)
        {
            Console.WriteLine("FeedEntry > " + argTitle + "\nType      > " + argType.ToString());
            return;
        }

        instance.debugUI.FeedEntry(argTitle, "", argType);
    }

    public static void FeedEntry(string argTitle, string argContent)
    {
        if (instance.debugUI == null)
        {
            Console.WriteLine("FeedEntry > " + argTitle + "\nContent   > " + argContent);
        }

        instance.debugUI.FeedEntry(argTitle, argContent, FeedEntryType.Info);
    }

    public static void FeedEntry(string argTitle)
    {
        if (instance.debugUI == null)
        {
            Console.WriteLine("FeedEntry > " + argTitle);
            return;
        }

        instance.debugUI.FeedEntry(argTitle, "", FeedEntryType.Info);
    }
}

//container to hold everything the console needs to work with commands and display info to the visual console
public struct ConCommand
{
    public string name;
    public string description;
    public MethodInfo method;
    public ConFlags flags;
    public ParameterInfo[] parms;

    public ConCommand(string argName, string argDescription, MethodInfo argMethod, ConFlags argFlags)
    {
        name = argName;
        description = argDescription;
        method = argMethod;
        flags = argFlags;
        parms = method.GetParameters();
    }
}

//container to hold ConCommand or ConVar (or more) plus flags for Call()
public class MiscConObject
{
    public object conObject;
    public ConFlags flags;

    public MiscConObject()
    {
        conObject = null;
        flags = 0;
    }
}

public enum DebugCallSource
{
    Internal, //from some local code somewhere
    User,     //from user input (eg a console frontend)
    Client,   //from another client in the server - treat with caution!
    Server,   //from the server
}

//todo: figure out extensible way to implement this. string?
public enum CallResult
{
    Success,
    FailureGeneric,
    FailureParmCountMismatch,
    FailureDataTypeMismatch
}

enum CommandParmScrubState
{
    Initial,
    Normal,
    String,
    Array
}

public class DebugParameterParseResult
{
    public bool success;
    public object result;
    public InputParmValidationFailureReason failureReason;
    public List<Tuple<string, string, int>> mismatchedParms;
    public string customErrorMessage;

    public DebugParameterParseResult()
    {
        success = true;
        result = null;
        failureReason = 0;
        mismatchedParms = new List<Tuple<string,string,int>>();
        customErrorMessage = "";
    }
}

struct InputParmValidation
{
    public bool valid;
    public InputParmValidationFailureReason failureReason;
    public List<Tuple<string,string,int>> mismatchedParms;
    public List<object> finalParms;
    public string customErrorMessage;

    public InputParmValidation(bool argStartState)
    {
        valid = argStartState;
        failureReason = InputParmValidationFailureReason.None;
        mismatchedParms = new List<Tuple<string,string,int>>();
        finalParms = new List<object>();
        customErrorMessage = "";
    }
}

public enum InputParmValidationFailureReason : byte
{
    None = 0,
    ParmCountMismatch = 1,
    DataTypeMismatch = 2,
    NoParserForType = 3,
    Other = 4,
}

//helper datatype used as an argument by command functions that finds a gameobject for use as an object reference in the command
public class GameObjectFinder
{
    private GameObjectFinderTargeting targetMode;
    private string targetObjectName;
    public GameObject result;

    public GameObjectFinder(GameObjectFinderTargeting argTargetMode)
    {
        targetMode = argTargetMode;
        FindObject();
    }

    public GameObjectFinder(GameObjectFinderTargeting argTargetMode, string argTargetObjectName)
    {
        targetMode = argTargetMode;
        targetObjectName = argTargetObjectName;
        FindObject();
    }

    private void FindObject()
    {
        switch (targetMode)
        {
            case GameObjectFinderTargeting.MousePosition:
                Ray finderRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit finderHit;

                if (Physics.Raycast(finderRay, out finderHit))
                {
                    result = finderHit.transform.gameObject;
                }
                break;

            case GameObjectFinderTargeting.ForwardCast:
                //NYI
                break;

            case GameObjectFinderTargeting.ObjectName:
                GameObject finder = GameObject.Find(targetObjectName);
                if (finder != null)
                {
                    result = finder.transform.gameObject;
                }
                break;
        }
    }
}

//methods of finding a gameobject
public enum GameObjectFinderTargeting
{
    MousePosition,
    ForwardCast,
    ObjectName
}