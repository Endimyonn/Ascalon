using System.Collections;
using System.Collections.Generic;

//Defines various commands that echo a string to the console with the specified
//entry type.
public class EchoCommands
{
    [ConCommand("con_echo", "Log a specified string")]
    static void cmd_con_echo(string argTitle)
    {
        Ascalon.Log(argTitle, LogMode.Info);
    }

    [ConCommand("con_echowarning", "Log a specified string as a warning")]
    static void cmd_con_echowarning(string argTitle)
    {
        Ascalon.Log(argTitle, LogMode.Warning);
    }

    [ConCommand("con_echoerror", "Log a specified string as an error")]
    static void cmd_con_echoerror(string argTitle)
    {
        Ascalon.Log(argTitle, LogMode.Error);
    }

    [ConCommand("con_echoassertion", "Log a specified string as an assertion")]
    static void cmd_con_echoassertion(string argTitle)
    {
        Ascalon.Log(argTitle, LogMode.Assertion);
    }

    [ConCommand("con_echoexception", "Log a specified string as an exception")]
    static void cmd_con_echoexception(string argTitle)
    {
        Ascalon.Log(argTitle, LogMode.Exception);
    }



    [ConCommand("con_echoverbose", "Log a specified string (verbose)")]
    static void cmd_con_echoverbose(string argTitle)
    {
        Ascalon.Log(argTitle, LogMode.InfoVerbose);
    }

    [ConCommand("con_echowarningverbose", "Log a specified string as a warning (verbose)")]
    static void cmd_con_echowarningverbose(string argTitle)
    {
        Ascalon.Log(argTitle, LogMode.WarningVerbose);
    }

    [ConCommand("con_echoerrorverbose", "Log a specified string as an error (verbose)")]
    static void cmd_con_echoerrorverbose(string argTitle)
    {
        Ascalon.Log(argTitle, LogMode.ErrorVerbose);
    }

    [ConCommand("con_echoassertionverbose", "Log a specified string as an assertion (verbose)")]
    static void cmd_con_echoassertionverbose(string argTitle)
    {
        Ascalon.Log(argTitle, LogMode.AssertionVerbose);
    }

    [ConCommand("con_echoexceptionverbose", "Log a specified string as an exception (verbose)")]
    static void cmd_con_echoexceptionverbose(string argTitle)
    {
        Ascalon.Log(argTitle, LogMode.ExceptionVerbose);
    }
}
