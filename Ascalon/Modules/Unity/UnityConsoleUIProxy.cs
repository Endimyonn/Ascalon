#if UNITY_2019_1_OR_NEWER
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class UnityConsoleUIProxy : MonoBehaviour
{
    public static UnityConsoleUIProxy instance;



    //storage
    private bool windowActive = true;
    private Vector3 windowLastPosition = Vector3.zero;
    private Vector3 windowInactivePosition = new Vector3(-9999.0f, -9999.0f, 0.0f);
    private static bool toggleClosesDuringEntry = true;
    private static bool autoFocusEntry = true;
    private static bool autoClearEntry = false;
    private bool wasAtBottom = false;
    private bool infoVisible = true;
    private bool warningVisible = true;
    private bool errorVisible = true;

    //autocomplete
    private bool canUseAutoComplete = false;
    private string autoCompleteValue = string.Empty;

    //command memory
    private string lastCommand = string.Empty;


    [Header("Settings")]
    public int maxEntries = 100;
    public int maxSuggestions = 20;
    public bool stayAtBottom = true;


    [Header("External References")]
    [SerializeField] private GameObject entryTemplate;


    [Header("Local References")]
    [SerializeField] private RectTransform windowRectTransform;
    [SerializeField] private TMP_InputField inputArea;
    [SerializeField] private GameObject inputSuggestionPanel;
    [SerializeField] private TextMeshProUGUI inputSuggestionLabel;
    [SerializeField] private Scrollbar entryScrollBar;
    [SerializeField] private Toggle stayAtBottomToggle;
    [SerializeField] private RectTransform entryParent;
    [SerializeField] private Image infoFilterImage;
    [SerializeField] private Image warningFilterImage;
    [SerializeField] private Image errorFilterImage;
    [SerializeField] private UnityUITooltipController tooltipController;



    private void Awake()
    {
        //initialize, connect to Ascalon instance
        if (instance != null)
        {
            Debug.LogError("UnityConsoleUIProxy was initialized twice. This is not allowed.");
            GameObject.Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        Ascalon.instance.uiModule = new UnityConsoleUI();

        //setup UI
        stayAtBottomToggle.isOn = stayAtBottom;

        //set initial visibility
        inputSuggestionPanel.gameObject.SetActive(false);
        SetWindowActive(false);
    }

    private void Update()
    {
        //window toggle hotkey
        if (Input.GetKeyDown(KeyCode.BackQuote) == true)
        {
            if (windowActive == false || inputArea.isFocused == false)
            {
                SetWindowActive(!windowActive);
            }
            else if (toggleClosesDuringEntry == true)
            {
                inputArea.DeactivateInputField();
                SetWindowActive(!windowActive);
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab) == true)
        {
            if (canUseAutoComplete == true)
            {
                inputArea.text = autoCompleteValue;
                inputArea.caretPosition = inputArea.text.Length;
                canUseAutoComplete = false;
                UpdateSuggestions();
            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) == true)
        {
            if (string.IsNullOrEmpty(lastCommand) == false && string.IsNullOrWhiteSpace(lastCommand) == false)
            {
                inputArea.text = lastCommand;
                inputArea.caretPosition = inputArea.text.Length;
                UpdateSuggestions();
            }
        }

        //ensure window is within viewport
        if (windowActive == true)
        {
            CheckWindowBounds();
        }
    }

    public void SetWindowActive(bool argNewState)
    {
        windowActive = argNewState;
        if (argNewState == true)
        {
            windowRectTransform.anchoredPosition = windowLastPosition;

            if (autoFocusEntry == true)
            {
                inputArea.Select();
                inputArea.ActivateInputField();
            }
        }
        else
        {
            windowLastPosition = windowRectTransform.anchoredPosition;
            windowRectTransform.anchoredPosition = windowInactivePosition;

            //remove trailing character caused by toggle-during-entry
            if (toggleClosesDuringEntry == true && (inputArea.text.EndsWith("`") || inputArea.text.EndsWith("~")))
            {
                inputArea.text = inputArea.text.Substring(0, inputArea.text.Length - 1);
            }

            if (autoClearEntry == true)
            {
                inputArea.text = string.Empty;
            }
        }
    }

    private void CheckWindowBounds()
    {
        if (windowRectTransform.sizeDelta.x <= Screen.width && windowRectTransform.sizeDelta.y <= Screen.height)
        {
            if (windowRectTransform.anchoredPosition.x < 0)
            {
                windowRectTransform.anchoredPosition = new Vector2(0, windowRectTransform.anchoredPosition.y);
            }
            else if (windowRectTransform.anchoredPosition.x + windowRectTransform.sizeDelta.x > Screen.width)
            {
                windowRectTransform.anchoredPosition = new Vector2(Screen.width - windowRectTransform.sizeDelta.x, windowRectTransform.anchoredPosition.y);
            }
            if (windowRectTransform.anchoredPosition.y > 0)
            {
                windowRectTransform.anchoredPosition = new Vector2(windowRectTransform.anchoredPosition.x, 0);
            }
            else if (windowRectTransform.anchoredPosition.y - windowRectTransform.sizeDelta.y < -Screen.height)
            {
                windowRectTransform.anchoredPosition = new Vector2(windowRectTransform.anchoredPosition.x, -Screen.height + windowRectTransform.sizeDelta.y);
            }
        }
        else
        {
            if (windowRectTransform.sizeDelta.x <= Screen.width)
            {
                windowRectTransform.anchoredPosition = new Vector2(windowRectTransform.anchoredPosition.x, 0);
            }
            if (windowRectTransform.sizeDelta.y <= Screen.height)
            {
                windowRectTransform.anchoredPosition = new Vector2(0, windowRectTransform.anchoredPosition.y);
            }
        }
    }

    public void Log(string argTitle, string argContent, LogMode argType)
    {
        //do nothing if this is a verbose log and verbose logging is disabled
        if ((bool)Ascalon.GetConVar("con_verbose") == false)
        {
            if (argType == LogMode.InfoVerbose || argType == LogMode.WarningVerbose || argType == LogMode.ErrorVerbose || argType == LogMode.AssertionVerbose || argType == LogMode.ExceptionVerbose)
            {
                return;
            }
        }

        //store whether the scroll was at the bottom
        wasAtBottom = entryScrollBar.value == 0;

        GameObject newEntry = null;

        //determine if we need to pop an old entry to account for size limit
        if (entryParent.childCount >= maxEntries)
        {
            newEntry = entryParent.GetChild(0).gameObject;

            //move to bottom
            newEntry.transform.SetAsLastSibling();
        }
        else
        {
            newEntry = GameObject.Instantiate(entryTemplate, entryParent);
            newEntry.name = entryTemplate.name;
        }

        //set entry icon and content
        UnityUIFeedEntry entryController = newEntry.GetComponent<UnityUIFeedEntry>();
        entryController.SetType(argType);
        entryController.SetText(argTitle);
        entryController.SetTooltip(argContent);
        entryController.tooltipController = this.tooltipController;

        //hide the entry if a filter is active for it
        switch (entryController.logType)
        {
            case LogMode.Info:
                if (infoVisible == false)
                {
                    newEntry.SetActive(false);
                }
                break;

            case LogMode.Warning:
                if (warningVisible == false)
                {
                    newEntry.SetActive(false);
                }
                break;

            case LogMode.Error:
                if (errorVisible == false)
                {
                    newEntry.SetActive(false);
                }
                break;
        }

        if (stayAtBottom == true && wasAtBottom == true)
        {
            StartCoroutine(ScrollToBottomDelayed());
        }
    }

    private IEnumerator ScrollToBottomDelayed()
    {
        //gotta wait for the scroll view to update
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        entryScrollBar.value = 0;
    }

    public void Call()
    {
        if (string.IsNullOrWhiteSpace(inputArea.text) == false)
        {
            lastCommand = inputArea.text;
            Ascalon.Call(inputArea.text, new AscalonCallContext(AscalonCallSource.User));
        }
        inputArea.text = string.Empty;
    }

    #region UI Buttons
    public void CloseButtonPressed()
    {
        SetWindowActive(false);
    }

    public void ClearEntries()
    {
        foreach (Transform getChild in entryParent)
        {
            GameObject.Destroy(getChild.gameObject);
        }
    }

    public void ScrollToBottomButtonPressed()
    {
        entryScrollBar.value = 0;
    }

    public void StayAtBottomCheckBoxChanged()
    {
        stayAtBottom = stayAtBottomToggle.isOn;
    }

    public void ToggleInfoEntries()
    {
        wasAtBottom = entryScrollBar.value == 0;
        foreach (Transform getChild in entryParent)
        {
            if ((getChild.GetComponent<UnityUIFeedEntry>()).logType == LogMode.Info)
            {
                getChild.gameObject.SetActive(!infoVisible);
            }
        }
        FilterToggleScrollCheck();

        infoFilterImage.color = (infoVisible == true ? new Color(0.5f, 0.5f, 0.5f) : new Color(1.0f, 1.0f, 1.0f));
        infoVisible = !infoVisible;
    }

    public void ToggleWarningEntries()
    {
        wasAtBottom = entryScrollBar.value == 0;
        foreach (Transform getChild in entryParent)
        {
            if ((getChild.GetComponent<UnityUIFeedEntry>()).logType == LogMode.Warning)
            {
                getChild.gameObject.SetActive(!warningVisible);
            }
        }
        FilterToggleScrollCheck();

        warningFilterImage.color = (warningVisible == true ? new Color(0.5f, 0.5f, 0.5f) : new Color(1.0f, 1.0f, 1.0f));
        warningVisible = !warningVisible;
    }

    public void ToggleErrorEntries()
    {
        wasAtBottom = entryScrollBar.value == 0;
        foreach (Transform getChild in entryParent)
        {
            if ((getChild.GetComponent<UnityUIFeedEntry>()).logType == LogMode.Error)
            {
                getChild.gameObject.SetActive(!errorVisible);
            }
        }
        FilterToggleScrollCheck();

        errorFilterImage.color = (errorVisible == true ? new Color(0.5f, 0.5f, 0.5f) : new Color(1.0f, 1.0f, 1.0f));
        errorVisible = !errorVisible;
    }

    private void FilterToggleScrollCheck()
    {
        if (stayAtBottom == true && wasAtBottom == true)
        {
            StartCoroutine(ScrollToBottomDelayed());
        }
    }
    #endregion

    #region Suggestions
    public void UpdateSuggestions()
    {
        if (inputArea.text.Length > 0)
        {
            if (inputArea.text.TrimStart().Contains(" "))
            {
                string firstSegment = inputArea.text.Split()[0];
                inputSuggestionLabel.text = firstSegment;

                if (Ascalon.ConCommandExists(firstSegment))
                {
                    if (!Ascalon.GetConCommandEntry(firstSegment).flags.HasFlag(ConFlags.Hidden))
                    {
                        inputSuggestionLabel.text += " (";
                        ConCommand passedCommand = Ascalon.GetConCommandEntry(firstSegment);
                        if (passedCommand.parms.Length > 0)
                        {
                            foreach (ParameterInfo parm in passedCommand.parms)
                            {
                                inputSuggestionLabel.text += "" + parm.Name + ":" + parm.ParameterType + "  ";
                            }
                            inputSuggestionLabel.text = inputSuggestionLabel.text.Remove(inputSuggestionLabel.text.Length - 2);
                            inputSuggestionLabel.text += ")";
                        }
                        else
                        {
                            inputSuggestionLabel.text = firstSegment + " (none)";
                        }
                    }
                }
                else if (Ascalon.ConVarExists(firstSegment))
                {
                    if (Ascalon.GetConVarEntry(firstSegment).flags.HasFlag(ConFlags.Hidden))
                    {
                        ConVar passedConVar = Ascalon.GetConVarEntry(firstSegment);
                        inputSuggestionLabel.text += " " + passedConVar.GetData() + " (" + passedConVar.cvarDataType.ToString() + ")";
                    }
                }

                canUseAutoComplete = false;
            }
            else //we don't have a complete first segment yet
            {
                string inputName = "";
                for (int i = 0; i < inputArea.text.Length; i++)
                {
                    if (char.IsLetterOrDigit(inputArea.text[i]) || "_\"\'".Contains(inputArea.text[i]))
                    {
                        inputName += inputArea.text[i];
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
                    if (inputSuggestionPanel.activeSelf == false)
                    {
                        inputSuggestionPanel.SetActive(true);
                    }
                    inputSuggestionLabel.text = "";

                    int addedSuggestions = 0;
                    for (int i = 0; i < matchingEntries.Count; i++)
                    {
                        if (addedSuggestions == maxSuggestions)
                        {
                            break;
                        }

                        if (i == 0)
                        {
                            inputSuggestionLabel.text += matchingEntries[i];
                            
                            if (matchingEntries[i].Contains(" "))
                            {
                                autoCompleteValue = matchingEntries[i].Substring(0, matchingEntries[i].IndexOf(" "));
                            }
                            else
                            {
                                autoCompleteValue = matchingEntries[i];
                            }
                        }
                        else
                        {
                            inputSuggestionLabel.text += "\n" + matchingEntries[i];
                        }
                        addedSuggestions++;
                    }

                    canUseAutoComplete = true;
                }
                else
                {
                    inputSuggestionLabel.text = "";
                    inputSuggestionPanel.SetActive(false);
                    
                    canUseAutoComplete = false;
                }
            }

        }
        else
        {
            inputSuggestionLabel.text = "";
            inputSuggestionPanel.SetActive(false);
                    
            canUseAutoComplete = false;
        }
    }
    #endregion

    [ConVar("con_entry_autofocus", "If enabled, the command entry area will be automatically focused upon activating the console UI.")]
    static ConVar cvar_con_entry_autofocus = new ConVar(true)
    {
        DataChanged = (object oldData, object newData) =>
        {
            autoFocusEntry = (bool)newData;
        }
    };

    [ConVar("con_entry_autoclear", "If enabled, the command entry area will be cleared upon closing the console UI.", ConFlags.Save)]
    static ConVar cvar_con_entry_autoclear = new ConVar(false)
    {
        DataChanged = (object oldData, object newData) =>
        {
            autoClearEntry = (bool)newData;
        }
    };

    [ConVar("con_toggle_close_during_entry", "If enabled, pressing the toggle key will close the console UI even if actively entering a command.", ConFlags.Save)]
    static ConVar cvar_con_toggle_close_during_entry = new ConVar(true)
    {
        DataChanged = (object oldData, object newData) =>
        {
            toggleClosesDuringEntry = (bool)newData;
        }
    };
}
#endif
