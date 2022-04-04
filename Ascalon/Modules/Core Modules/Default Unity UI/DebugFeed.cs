using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

//this is the stock implementation of the UI
public class DebugFeed : AscalonUIModule
{

    //tracking
    //todo: sort better
    [SerializeField] private TMP_InputField uiInputField;

    [SerializeField] private GameObject uiDebugLogPanel;
    public GameObject uiDebugLogContent;

    [SerializeField] private GameObject uiTooltipPanel;
    [SerializeField] private TextMeshProUGUI uiTooltipText;

    [SerializeField] public GameObject uiFeedEntryPrefab;

    [SerializeField] private GameObject uiSuggestionPanel;
    [SerializeField] private TextMeshProUGUI uiSuggestionText;


    //tooltip raycast
    [SerializeField] private GraphicRaycaster uiRaycaster;
    private EventSystem uiEventSystem;
    private PointerEventData uiPointerEventData;

    //entry visibility
    private bool infoVisible = true;
    private bool warningVisible = true;
    private bool errorVisible = true;

    //limit number of entries
    public int uiMaxFeedEntries = 150;

    //view management
    public bool logWasAtBottom = false;
    public bool entryAdded = false;
    public bool inputFieldActive = false;

    public override void Initialize()
    {
        base.Initialize();

        uiInputField = AscalonUnity.instance.transform.GetChild(0).Find("Command Entry").GetComponent<TMP_InputField>();
        uiDebugLogPanel = AscalonUnity.instance.transform.GetChild(0).gameObject;
        uiDebugLogContent = AscalonUnity.instance.transform.GetChild(0).Find("Viewport").Find("Content").gameObject;
        uiTooltipPanel = AscalonUnity.instance.transform.GetChild(1).gameObject;
        uiTooltipText = AscalonUnity.instance.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        uiSuggestionPanel = AscalonUnity.instance.transform.GetChild(0).Find("Command Entry").Find("Suggestions Panel").gameObject;
        uiSuggestionText = uiSuggestionPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        uiRaycaster = AscalonUnity.instance.gameObject.GetComponent<GraphicRaycaster>();

        uiFeedEntryPrefab = AscalonUnity.instance.uiFeedEntryPrefab.gameObject;

        //add a callback to handle standard unity debug logs
        Application.logMessageReceived += FeedLogHandler;

        uiEventSystem = EventSystem.current;

        uiInputField.onSubmit.AddListener(Call);
    }

    public override void Update()
    {
        //tooltip logic
        if (uiDebugLogPanel.activeInHierarchy == true)
        {
            uiPointerEventData = new PointerEventData(uiEventSystem);
            uiPointerEventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            uiRaycaster.Raycast(uiPointerEventData, results);
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.name == "TooltipRaycastCatcher")
                {
                    uiTooltipText.text = result.gameObject.GetComponent<TooltipStorage>().tooltip
                                                                                         .Replace("\\n", "\n"); //represent newlines properly

                    if (uiTooltipText.text != "")
                    {
                        uiTooltipPanel.SetActive(true);
                        uiTooltipPanel.transform.position = new Vector3(Input.mousePosition.x + 12, Input.mousePosition.y - 18, uiTooltipPanel.transform.position.z);
                    }
                    else //handle cases where we go straight from an entry with a tooltip to one without
                    {
                        uiTooltipPanel.SetActive(false);
                    }
                }
                else
                {
                    uiTooltipPanel.SetActive(false);
                }
            }
            if (results.Count == 0)
            {
                uiTooltipPanel.SetActive(false);
            }
        }

        //todo: reimplement scroll-to-bottom-if-at-bottom
    }








    public override void OnCallComplete(string argCallString, bool argSuccess, AscalonCallContext argContext)
    {
        uiInputField.text = "";
    }








    public override void Log(string argTitle, string argContent, LogMode argType)	//enters a string into the feed
    {
        this.entryAdded = true;


        if (argContent == null)
        {
            argContent = "";    //avoid NullReferenceExceptions
        }

        try
        {
            switch (argType) //add entry
            {
                case LogMode.Info:
                    GameObject newEntryInfo = GameObject.Instantiate(this.uiFeedEntryPrefab, this.uiDebugLogContent.transform); //todo: the second var is missingreference on scene reload, but apparently not in editor?? why???
                    newEntryInfo.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = argTitle;
                    newEntryInfo.transform.GetChild(1).gameObject.GetComponent<TooltipStorage>().tooltip = argContent;
                    newEntryInfo.transform.GetChild(2).gameObject.SetActive(true);
                    break;

                case LogMode.Warning:
                    GameObject newEntryWarning = GameObject.Instantiate(this.uiFeedEntryPrefab, this.uiDebugLogContent.transform);
                    newEntryWarning.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = argTitle;
                    newEntryWarning.transform.GetChild(1).gameObject.GetComponent<TooltipStorage>().tooltip = argContent;
                    newEntryWarning.transform.GetChild(3).gameObject.SetActive(true);
                    break;

                case LogMode.Error:
                    GameObject newEntryError = GameObject.Instantiate(this.uiFeedEntryPrefab, this.uiDebugLogContent.transform);
                    newEntryError.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = argTitle;
                    newEntryError.transform.GetChild(1).gameObject.GetComponent<TooltipStorage>().tooltip = argContent;
                    newEntryError.transform.GetChild(4).gameObject.SetActive(true);
                    break;

                case LogMode.Assertion:
                    GameObject newEntryAssertion = GameObject.Instantiate(this.uiFeedEntryPrefab, this.uiDebugLogContent.transform);
                    newEntryAssertion.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = argTitle;
                    newEntryAssertion.transform.GetChild(1).gameObject.GetComponent<TooltipStorage>().tooltip = argContent;
                    newEntryAssertion.transform.GetChild(5).gameObject.SetActive(true);
                    break;

                case LogMode.Exception:
                    GameObject newEntryException = GameObject.Instantiate(this.uiFeedEntryPrefab, this.uiDebugLogContent.transform);
                    newEntryException.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = argTitle;
                    newEntryException.transform.GetChild(1).gameObject.GetComponent<TooltipStorage>().tooltip = "";
                    newEntryException.transform.GetChild(5).gameObject.SetActive(true);
                    break;

                case LogMode.InfoVerbose:
                    if ((bool)Ascalon.GetConVar("con_verbose"))
                    {
                        GameObject newEntryInfoVerbose = GameObject.Instantiate(this.uiFeedEntryPrefab, this.uiDebugLogContent.transform);
                        newEntryInfoVerbose.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = argTitle;
                        newEntryInfoVerbose.transform.GetChild(1).gameObject.GetComponent<TooltipStorage>().tooltip = argContent;
                        newEntryInfoVerbose.transform.GetChild(2).gameObject.SetActive(true);
                    }
                    break;

                case LogMode.WarningVerbose:
                    if ((bool)Ascalon.GetConVar("con_verbose"))
                    {
                        GameObject newEntryWarningVerbose = GameObject.Instantiate(this.uiFeedEntryPrefab, this.uiDebugLogContent.transform);
                        newEntryWarningVerbose.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = argTitle;
                        newEntryWarningVerbose.transform.GetChild(1).gameObject.GetComponent<TooltipStorage>().tooltip = argContent;
                        newEntryWarningVerbose.transform.GetChild(3).gameObject.SetActive(true);
                    }
                    break;

                case LogMode.ErrorVerbose:
                    if ((bool)Ascalon.GetConVar("con_verbose"))
                    {
                        GameObject newEntryErrorVerbose = GameObject.Instantiate(this.uiFeedEntryPrefab, this.uiDebugLogContent.transform);
                        newEntryErrorVerbose.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = argTitle;
                        newEntryErrorVerbose.transform.GetChild(1).gameObject.GetComponent<TooltipStorage>().tooltip = argContent;
                        newEntryErrorVerbose.transform.GetChild(4).gameObject.SetActive(true);
                    }
                    break;

                case LogMode.AssertionVerbose:
                    if ((bool)Ascalon.GetConVar("con_verbose"))
                    {
                        GameObject newEntryAssertionVerbose = GameObject.Instantiate(this.uiFeedEntryPrefab, this.uiDebugLogContent.transform);
                        newEntryAssertionVerbose.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = argTitle;
                        newEntryAssertionVerbose.transform.GetChild(1).gameObject.GetComponent<TooltipStorage>().tooltip = argContent;
                        newEntryAssertionVerbose.transform.GetChild(5).gameObject.SetActive(true);
                    }
                    break;

                case LogMode.ExceptionVerbose:
                    if ((bool)Ascalon.GetConVar("con_verbose"))
                    {
                        GameObject newEntryExceptionVerbose = GameObject.Instantiate(this.uiFeedEntryPrefab, this.uiDebugLogContent.transform);
                        newEntryExceptionVerbose.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = argTitle;
                        newEntryExceptionVerbose.transform.GetChild(1).gameObject.GetComponent<TooltipStorage>().tooltip = "";
                        newEntryExceptionVerbose.transform.GetChild(5).gameObject.SetActive(true);
                    }
                    break;
            }
        }
        catch (Exception issue)
        {
            Debug.Log("Exception occurred while trying to log data with Ascalon: " + issue.Message);
        }
        

        //don't keep too many entries to prevent lag from buildup
        if (this.uiDebugLogContent.transform.childCount > this.uiMaxFeedEntries)
        {
            GameObject.Destroy(this.uiDebugLogContent.transform.GetChild(0).gameObject);
        }
    }

    //delegate member to handle builtin unity logs and add them as feed entries
    private void FeedLogHandler(string argLogString, string argStackTrace, LogType argLogType)
    {
        if (Application.isEditor)
        {
            argStackTrace = "Stack trace: \n\n" + argStackTrace;
        }
        switch (argLogType)
        {
            case LogType.Log:
                Ascalon.Log(argLogString, argStackTrace, LogMode.Info);
                break;

            case LogType.Warning:
                Ascalon.Log(argLogString, argStackTrace, LogMode.Warning);
                break;

            case LogType.Error:
                Ascalon.Log(argLogString, argStackTrace, LogMode.Error);
                break;

            case LogType.Assert:
                Ascalon.Log(argLogString, argStackTrace, LogMode.Assertion);
                break;

            case LogType.Exception:
                //DebugConsole.FeedEntry(argLogString, argStackTrace, LogType.Exception);
                break;
        }
    }







    //todo: comments please!!
    public void UpdateSuggestions()
    {
        if (uiInputField.text.Length > 0)
        {
            if (uiInputField.text.TrimStart().Contains(" "))
            {
                string firstSegment = uiInputField.text.Split()[0];
                uiSuggestionText.text = firstSegment;

                if (Ascalon.ConCommandExists(firstSegment))
                {
                    if (!Ascalon.GetConCommandEntry(firstSegment).flags.HasFlag(ConFlags.Hidden))
                    {
                        uiSuggestionText.text += " (";
                        ConCommand passedCommand = Ascalon.GetConCommandEntry(firstSegment);
                        if (passedCommand.parms.Length > 0)
                        {
                            foreach (ParameterInfo parm in passedCommand.parms)
                            {
                                uiSuggestionText.text += "" + parm.Name + ":" + parm.ParameterType + "  ";
                            }
                            uiSuggestionText.text = uiSuggestionText.text.Remove(uiSuggestionText.text.Length - 2);
                            uiSuggestionText.text += ")";
                        }
                        else
                        {
                            uiSuggestionText.text = firstSegment + " (none)";
                        }
                    }
                }
                else if (Ascalon.ConVarExists(firstSegment))
                {
                    if (Ascalon.GetConVarEntry(firstSegment).flags.HasFlag(ConFlags.Hidden))
                    {
                        ConVar passedConVar = Ascalon.GetConVarEntry(firstSegment);
                        uiSuggestionText.text += " " + passedConVar.GetData() + " (" + passedConVar.cvarDataType.ToString() + ")";
                    }
                }
            }
            else //we don't have a complete first segment yet
            {
                string inputName = "";
                for (int i = 0; i < uiInputField.text.Length; i++)
                {
                    if (char.IsLetterOrDigit(uiInputField.text[i]) || "_\"\'".Contains(uiInputField.text[i]))
                    {
                        inputName += uiInputField.text[i];
                    }
                    else
                    {
                        return;
                    }
                }
                inputName = inputName.ToLower();

                //todo: compile combined list of entries for efficiency?

                //add all matching commands
                List<string> matchingEntries = Ascalon.instance.conCommands.FindAll(ConCommand => ConCommand.name.StartsWith(inputName))
                                                          .Where(ConCommand => !ConCommand.flags.HasFlag(ConFlags.Hidden))
                                                          .Select(ConCommand => ConCommand.name)
                                                          .ToList();

                //add all matching convars
                matchingEntries.AddRange(Ascalon.instance.conVars.FindAll(ConVar => ConVar.name.StartsWith(inputName))
                                               .Where(ConVar => !ConVar.flags.HasFlag(ConFlags.Hidden))
                                               .Select(ConVar => ConVar.name + " " + (ConVar.flags.HasFlag(ConFlags.Sensitive) ? "(PROTECTED)" : AscalonUtil.ConVarDataToString(ConVar.GetData())))
                                               .ToList());

                //sort alphabetically
                matchingEntries.Sort();


                if (matchingEntries.Count > 0)  //if we have any matching commands, proceed
                {
                    if (uiSuggestionPanel.activeSelf == false)
                    {
                        uiSuggestionPanel.SetActive(true);
                    }
                    uiSuggestionText.text = "";

                    for (int i = 0; i < matchingEntries.Count; i++)
                    {
                        if (i == matchingEntries.Count)
                        {
                            //we've hit the end, so don't add a newline
                            uiSuggestionText.text += matchingEntries[i];
                        }
                        else
                        {
                            uiSuggestionText.text += matchingEntries[i] + "\n";
                        }
                    }
                }
                else
                {
                    uiSuggestionText.text = "";
                    uiSuggestionPanel.SetActive(false);
                }
            }
            
        }
        else
        {
            uiSuggestionText.text = "";
            uiSuggestionPanel.SetActive(false);
        }
    }








    //todo: these can be simplified
    public void ToggleInfoEntries()
    {
        if (infoVisible)
        {
            foreach (Transform entryObject in uiDebugLogContent.transform)
            {
                if (entryObject.GetChild(2).gameObject.activeSelf)
                {
                    entryObject.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            foreach (Transform entryObject in uiDebugLogContent.transform)
            {
                if (entryObject.GetChild(2).gameObject.activeSelf)
                {
                    entryObject.gameObject.SetActive(true);
                }
            }
        }

        infoVisible = !infoVisible;
    }

    public void ToggleWarningEntries()
    {
        if (warningVisible)
        {
            foreach (Transform entryObject in uiDebugLogContent.transform)
            {
                if (entryObject.GetChild(3).gameObject.activeSelf)
                {
                    entryObject.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            foreach (Transform entryObject in uiDebugLogContent.transform)
            {
                if (entryObject.GetChild(3).gameObject.activeSelf)
                {
                    entryObject.gameObject.SetActive(true);
                }
            }
        }

        warningVisible = !warningVisible;
    }

    public void ToggleErrorEntries()
    {
        if (errorVisible)
        {
            foreach (Transform entryObject in uiDebugLogContent.transform)
            {
                if (entryObject.GetChild(4).gameObject.activeSelf)
                {
                    entryObject.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            foreach (Transform entryObject in uiDebugLogContent.transform)
            {
                if (entryObject.GetChild(4).gameObject.activeSelf)
                {
                    entryObject.gameObject.SetActive(true);
                }
            }
        }

        errorVisible = !errorVisible;
    }








    public void Clear()
    {
        foreach (Transform feedEntry in uiDebugLogContent.transform)
        {
            GameObject.Destroy(feedEntry.gameObject);
        }
    }








    public void Call(string argInput)
    {
        Ascalon.Call(argInput, new AscalonCallContext(AscalonCallSource.User));
    }







    public override void ClearEntries()
    {
        foreach (Transform entry in uiDebugLogContent.transform)
        {
            GameObject.Destroy(entry.gameObject);
        }
    }








    [ConCommand("con_clear", "Clear the console of entries")]
    static void cmd_con_clear()
    {
        Ascalon.instance.uiModule.ClearEntries();
    }
}

public partial class Ascalon
{
    public static void Log(string argTitle, string argContent, LogMode argType)
    {
        if (instance.uiModule == null)
        {
            Console.WriteLine("FeedEntry > " + argTitle + "\nContent   > " + argContent + "\nType      > " + argType.ToString());
            return;
        }

        instance.uiModule.Log(argTitle, argContent, argType);
    }

    public static void Log(string argTitle, LogMode argType)
    {
        if (instance.uiModule == null)
        {
            Console.WriteLine("FeedEntry > " + argTitle + "\nType      > " + argType.ToString());
            return;
        }

        instance.uiModule.Log(argTitle, "", argType);
    }

    public static void Log(string argTitle, string argContent)
    {
        if (instance.uiModule == null)
        {
            Console.WriteLine("FeedEntry > " + argTitle + "\nContent   > " + argContent);
        }

        instance.uiModule.Log(argTitle, argContent, LogMode.Info);
    }

    public static void Log(string argTitle)
    {
        if (instance.uiModule == null)
        {
            Console.WriteLine("FeedEntry > " + argTitle);
            return;
        }

        instance.uiModule.Log(argTitle, "", LogMode.Info);
    }
}