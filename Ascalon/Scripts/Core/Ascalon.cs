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

//By design, the console front-end is made to be enabled and visible by default. the
//implementation of toggling is done on a per-project basis, and no input systems
//aside from Unity's own EventSystem (for text entry frontend) are used.

//For examples of how to implement commands and ConVars into code, see BaseCommands.cs.
[System.Serializable]
public partial class Ascalon
{
    //singleton
    public static Ascalon instance;

    //UI module
    public AscalonUIModule uiModule;

    //networking module
    public AscalonNetModule netModule;

    //RCon server
    public AscalonRConServer rconServer;

    //lets things know whether initialization completed;
    public bool ready = false;

    //default config info
    public string mainConfigName = "config";
    public string mainConfigDirectory = "";
    public bool loadConfigUnityStyle = false;


    //delegates
    public delegate void OnCallCompleted(string argCallString, bool argSuccess, AscalonCallContext argContext);
    public OnCallCompleted onCallCompleted;
    


    public bool validateCommandAddition; //NYI, for optional better handling of command/cvar definition conflict prevention



    public List<KeyValuePair<string, MethodInfo>> parameterParsers = new List<KeyValuePair<string, MethodInfo>>();








    public List<ConCommand> conCommands = new List<ConCommand>();  //master list of commands - see relevant struct
    public List<ConVar> conVars = new List<ConVar>();

    public void Initialize()
    {
        //singleton logic
        if (instance != null)
        {
            Console.WriteLine("Duplicate Ascalon instance created - do not do this!");
            return;
        }
        else
        {
            instance = this;
        }







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
                else if (foundMethod.GetCustomAttributes(typeof(AscalonParameterParserAttribute), false).Length > 0)
                {
                    AscalonParameterParserAttribute foundAttribute = (AscalonParameterParserAttribute)foundMethod.GetCustomAttribute(typeof(AscalonParameterParserAttribute));
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

        //initialize modules
        if (uiModule != null)
        {
            uiModule.core = this;
            uiModule.Initialize();
        }

        if (netModule != null)
        {
            netModule.core = this;
            netModule.Initialize();
        }
        else
        {
            Console.WriteLine("Ascalon could not start because no NetModule was provided before initialization!");
        }

        rconServer = new AscalonRConServer();
        rconServer.StartListening();

        //load config
        if (!loadConfigUnityStyle)
        {
            AscalonConfigTools.ReadConfig(mainConfigDirectory + mainConfigName);
        }
        else
        {
            AscalonConfigTools.ReadConfigUnity(mainConfigName);
        }

        ready = true;
    }


    






    public static void Call(string argInput, AscalonCallContext argContext)
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
            Ascalon.Log("Error: no such command or ConVar '" + inputName.ToLower() + "'", "The command or ConVar entered does not exist.\nOriginal received string: " + argInput, LogMode.Error);

            //callback
            if (instance.onCallCompleted != null)
            {
                instance.onCallCompleted(argInput, false, argContext);
            }
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

                inputValidity = instance.ValidateInputParms(inputParms, AscalonUtil.ParmsToStrings(findCommand.parms));
            }
            else if (findConVar.name != null)
            {
                workingWith = 2;
                findConObject.conObject = findConVar;
                findConObject.flags = findConVar.flags;

                inputValidity = instance.ValidateInputParms(inputParms, AscalonUtil.ParmsToStrings(findConVar.cvarDataType));
            }

            //check ConFlags and context before proceeding
            bool flagsPassed = Ascalon.instance.ValidateFlags(argContext, findConObject.flags, inputName, argInput);
            bool contextPassed = Ascalon.instance.ValidateContext(argContext, findConObject.flags);



            //final logic tree
            if (inputValidity.valid)
            {
                if (flagsPassed && contextPassed)
                {
                    //send call to server if it is marked to be ran on server
                    if (findConObject.flags.HasFlag(ConFlags.RunOnServer) && AscalonNetModule.GetRole() != NetRole.Server && argContext.source != AscalonCallSource.Server)
                    {
                        instance.netModule.NetCall(argInput, new AscalonCallContext(AscalonNetModule.GetRole()), new AscalonCallNetTarget(NetRole.Server));
                        return;
                    }
                    else //run locally
                    {
                        object[] finalParms = inputValidity.finalParms.ToArray();

                        if (workingWith == 1) //ConCommand
                        {
                            findCommand.method.Invoke(null, finalParms);
                        }
                        else if (workingWith == 2) //ConVar
                        {
                            findConVar.SetData(finalParms);
                        }

                        //if the call was a replicated object, replicate it on all clients
                        if (findConObject.flags.HasFlag(ConFlags.ClientReplicated) && AscalonNetModule.GetRole() == NetRole.Server)
                        {
                            instance.netModule.SendReplicatedToAllClients(argInput, new AscalonCallContext(AscalonCallSource.Server));
                        }

                        //run callback
                        if (instance.onCallCompleted != null)
                        {
                            instance.onCallCompleted(argInput, true, argContext);
                        }
                    }
                }
                else
                {
                    if (instance.onCallCompleted != null)
                    {
                        instance.onCallCompleted(argInput, false, argContext);
                    }
                }
            }
            else //parameter(s) did not parse correctly
            {
                switch (inputValidity.failureReason)
                {
                    case InputParmValidationFailureReason.ParmCountMismatch:
                        Ascalon.Log("Error: incorrect number of arguments provided", "The number of arguments provided does not match the number expected for the command or ConVar.\nExpected: " + (findCommand.name != null ? findCommand.parms.Length.ToString() : "1") + "\nProvided: " + inputParms.Length, LogMode.Error);
                        break;

                    case InputParmValidationFailureReason.DataTypeMismatch:
                        string failedParms = "";
                        foreach (Tuple<string, string, int> failedParm in inputValidity.mismatchedParms)
                        {
                            failedParms += "Argument " + failedParm.Item3 + " (" + failedParm.Item1 + ") is not of type " + failedParm.Item2 + "\n";
                        }
                        Ascalon.Log("Error: argument types do not match", "The type(s) of the argument(s) provided for the specified command or ConVar do not match what is required by it and could not be converted. Failed arguments:\n" + failedParms.Remove(failedParms.Length - 1, 1), LogMode.Error);
                        break;

                    case InputParmValidationFailureReason.NoParserForType:
                        string errorString = "Invalid arguments:\n";
                        foreach (Tuple<string, string, int> failedParm in inputValidity.mismatchedParms)
                        {
                            errorString += "Argument " + failedParm.Item3 + " (" + failedParm.Item2 + ")\n";
                        }
                        errorString = errorString.Remove(errorString.Length - 1);

                        Ascalon.Log("Error: no parser found for argument(s)", errorString, LogMode.Error);
                        break;

                    case InputParmValidationFailureReason.Other:
                        Ascalon.Log("Error: issue occurred while parsing command arguments", inputValidity.customErrorMessage, LogMode.Error);
                        break;
                }

                if (instance.onCallCompleted != null) //callback
                {
                    instance.onCallCompleted(argInput, false, argContext);
                }
            }
        }
    }

    //if we receive a call without a specified source, assume it's internal
    public static void Call(string argString)
    {
        AscalonCallContext context = new AscalonCallContext();
        context.source = AscalonCallSource.Internal;

        Ascalon.Call(argString, context);
    }

    List<string> GetInputParms(string argInput)
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
                
                AscalonParameterParseResult parseResult = (AscalonParameterParseResult)parameterParsers.Find(parser => parser.Key == argInputParms[i]).Value.Invoke(null, providedParm);

                if (parseResult.success)
                {
                    result.finalParms.Add(parseResult.result);
                }
                else
                {
                    result.valid = false;
                    result.failureReason = parseResult.failureReason;
                    result.customErrorMessage = parseResult.customErrorMessage;
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
    bool ValidateFlags(AscalonCallContext argContext, ConFlags argFlags, string argCallName, string argCall)
    {
        if (argFlags.HasFlag(ConFlags.Cheat))
        {
            //don't call if cheats are disabled
            if ((bool)Ascalon.GetConVar("server_cheats") == false)
            {
                return false;
            }
        }

        if (argFlags.HasFlag(ConFlags.ServerOnly))
        {
            //don't call if we are not the server
            if (AscalonNetModule.GetRole() != NetRole.Server)
            {

                if (argContext.source == AscalonCallSource.Server)
                {
                    //if it's ClientReplicated, we can do it still
                    if (!argFlags.HasFlag(ConFlags.ClientReplicated))
                    {
                        Ascalon.Log("You do not have permission for this.", LogMode.WarningVerbose);
                        return false;
                    }
                }
                else
                {
                    Ascalon.Log("You do not have permission for this.", LogMode.WarningVerbose);
                    return false;
                }
            }

            //don't call if this came from a client call
            if (argContext.source == AscalonCallSource.Client)
            {
                Ascalon.Log("This command or ConVar may only be run by the server.", LogMode.WarningVerbose);
                return false;
            }
        }

        if (argFlags.HasFlag(ConFlags.LockWhileConnected))
        {
            //don't call if we are in a network session
            if (AscalonNetModule.GetRole() != NetRole.Inactive)
            {
                return false;
            }
        }

        if (argContext.source == AscalonCallSource.RCon)
        {
            //don't call if RCon is disabled on this command or ConVar
            if (argFlags.HasFlag(ConFlags.NoRCon))
            {
                Ascalon.Log("RCon call was made for command or ConVar that disallows RCon.", LogMode.WarningVerbose);
                return false;
            }
        }

        if (argFlags.HasFlag(ConFlags.Hidden))
        {
            if (argContext.source != AscalonCallSource.Internal)
            {
                Ascalon.Log("Error: no such command or ConVar '" + argCallName.ToLower() + "'", "The command or ConVar entered does not exist.\nOriginal received string: " + argCall, LogMode.Error);
                return false;
            }
        }

        return true;
    }

    //validate context on a call
    bool ValidateContext(AscalonCallContext argContext, ConFlags argFlags)
    {
        if (argContext.source == AscalonCallSource.Internal)
        {
            return true;
        }
        
        if (argContext.source == AscalonCallSource.User)
        {
            return true;
        }

        if (argContext.source == AscalonCallSource.Client)
        {
            //server should always accept client calls
            if (netModule.role == NetRole.Server)
            {
                return true;
            }

            if ((bool)Ascalon.GetConVar("client_allowclientcall") == true)
            {
                return true;
            }

            return false;
        }

        if (argContext.source == AscalonCallSource.Server)
        {
            if (argFlags.HasFlag(ConFlags.ClientReplicated))
            {
                return true;
            }

            if ((bool)Ascalon.GetConVar("client_allowservercall") == true)
            {
                return true;
            }

            return false;
        }

        if (argContext.source == AscalonCallSource.RCon)
        {
            if ((bool)Ascalon.GetConVar("rcon_acceptincoming") == true)
            {
                return true;
            }

            return false;
        }

        return false;
    }















    







    public static object GetConVar(string argConVarName) //todo: custom struct or tuple with bool and object for better nullreference handling?
    {
        return Ascalon.instance.conVars.FirstOrDefault(ConVar => ConVar.name == argConVarName).GetData();
    }

    public static void SetConVar(string argConVarName, object argConVarData)
    {
        Ascalon.Call(argConVarName + " " + argConVarData.ToString());
    }

    public static ConCommand GetConCommandEntry(string argCommandName)
    {
        return Ascalon.instance.conCommands.FirstOrDefault(ConCommand => ConCommand.name == argCommandName);
    }

    public static ConVar GetConVarEntry(string argConVarName)
    {
        return Ascalon.instance.conVars.FirstOrDefault(ConVar => ConVar.name == argConVarName);
    }

    public static bool ConCommandExists(string argEntryName)
    {
        return Ascalon.instance.conCommands.Any(ConCommand => ConCommand.name == argEntryName);
    }

    public static bool ConVarExists(string argEntryName)
    {
        return Ascalon.instance.conVars.Any(ConVar => ConVar.name == argEntryName);
    }

    public static bool CommandOrConVarExists(string argEntryName)
    {
        return (ConCommandExists(argEntryName) == true || ConVarExists(argEntryName) == true);
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

public enum AscalonCallSource
{
    Internal, //from some local code somewhere
    User,     //from user input (eg a console frontend)
    Client,   //from another client in the server - treat with caution!
    Server,   //from the server
    RCon      //from an RCon client
}

[System.Serializable]
public struct AscalonCallContext
{
    public AscalonCallSource source;  //basic source info
    public string clientName;       //if call came from another client

    public AscalonCallContext(AscalonCallSource argSource, string argClientName = "")
    {
        source = argSource;
        clientName = argClientName;
    }

    public AscalonCallContext(NetRole argSource, string argClientName = "")
    {
        if (argSource == NetRole.Client)
        {
            source = AscalonCallSource.Client;
        }
        else if (argSource == NetRole.Server)
        {
            source = AscalonCallSource.Server;
        }
        else //assume the call is internal otherwise
        {
            source = AscalonCallSource.Internal;
        }

        clientName = argClientName;
    }
}

//todo: figure out extensible way to implement this. string?
public enum AscalonCallResult
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

public class AscalonParameterParseResult
{
    public bool success;
    public object result;
    public InputParmValidationFailureReason failureReason;
    public List<Tuple<string, string, int>> mismatchedParms;
    public string customErrorMessage;

    public AscalonParameterParseResult()
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