#if GODOT
using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

public partial class GodotConsoleUIProxy : Control
{
    public static GodotConsoleUIProxy instance;


    //storage
    private bool windowActive = true;
    private Vector2 windowLastPosition = Vector2.Zero;
    private Vector2 windowInactivePosition = new Vector2(-9999.0f, -9999.0f);
    private Rect2 currentViewport;
    private VScrollBar entryScrollBar;
    private static bool toggleClosesDuringEntry = true;
    private static bool autoFocusEntry = true;
    private static bool autoClearEntry = false;
    private bool delayedAutoFocus = false;
    private bool wasAtBottom = false;
    private bool infoVisible = true;
    private bool warningVisible = true;
    private bool errorVisible = true;

    //autocomplete
    private bool canUseAutoComplete = false;
    private string autoCompleteValue = string.Empty;

    //command memory
    private string lastCommand = string.Empty;


    //settings
    [Export] public int maxEntries = 100;
    [Export] public int maxSuggestions = 20;
    [Export] public bool stayAtBottom = true;


    [ExportSubgroup("External References")]
    [Export] private PackedScene entryTemplate;


    [ExportSubgroup("Scene References")]
    [Export] private LineEdit inputArea;
    [Export] private Label inputSuggestionLabel;
    [Export] private ScrollContainer entryScrollContainer;
    [Export] private CheckBox stayAtBottomCheckBox;
    [Export] private Control entryParent;
    [Export] private CanvasItem infoFilterButton;
    [Export] private CanvasItem warningFilterButton;
    [Export] private CanvasItem errorFilterButton;


    
    public override void _Ready()
    {
        //initialize, connect to Ascalon instance
        instance = this;
        Ascalon.instance.uiModule = new GodotConsoleUI();

        //get references
        entryScrollBar = entryScrollContainer.GetVScrollBar();

        //setup UI
        stayAtBottomCheckBox.ButtonPressed = stayAtBottom;

        //set initial visibility
        inputSuggestionLabel.Visible = false;
        SetWindowActive(false);
    }

    public override void _Process(double delta)
    {
        //ensure window is within current viewport
        if (windowActive == true)
        {
            CheckWindowBounds();
        }

        
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent)
        {
            if (keyEvent.Keycode == Key.Quoteleft && keyEvent.Echo == false)
            {
                if (keyEvent.Pressed == true)
                {
                    if (windowActive == false || inputArea.HasFocus() == false || toggleClosesDuringEntry == true)
                    {
                        SetWindowActive(!windowActive);
                    }
                }
                else if (keyEvent.IsReleased() == true)
                {
                    //process auto-focus delay
                    if (delayedAutoFocus == true)
                    {
                        inputArea.GrabFocus();
                        delayedAutoFocus = false;
                    }
                }
            }
            
            if (keyEvent.Keycode == Key.Tab && keyEvent.Echo == false)
            {
                if (canUseAutoComplete == true)
                {
                    inputArea.Text = autoCompleteValue;
                    inputArea.CaretColumn = autoCompleteValue.Length;
                    canUseAutoComplete = false;
                    UpdateSuggestions(inputArea.Text);
                }
            }
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent)
        {
            if (keyEvent.Keycode == Key.Up && keyEvent.Echo == false)
            {
                if (string.IsNullOrEmpty(lastCommand) == false && string.IsNullOrWhiteSpace(lastCommand) == false)
                {
                    inputArea.Text = lastCommand;
                    inputArea.CaretColumn = inputArea.Text.Length;
                    UpdateSuggestions(inputArea.Text);
                }
            }
        }
    }

    public void SetWindowActive(bool argNewState)
    {
        windowActive = argNewState;
        if (argNewState == true)
        {
            this.Position = windowLastPosition;

            if (autoFocusEntry == true)
            {
                //the LineEdit quickly accepts the toggle key input, so we wait until the key is released to focus it
                delayedAutoFocus = true;
            }
        }
        else
        {
            windowLastPosition = this.Position;
            this.Position = windowInactivePosition;

            //avoid trailing character caused by toggle-during-entry
            if (toggleClosesDuringEntry == true)
            {
                inputArea.ReleaseFocus();
            }

            if (autoClearEntry == true)
            {
                inputArea.Clear();
            }
        }
    }

    private void CheckWindowBounds()
    {
        currentViewport = GetViewportRect();
        if (this.Size.X <= currentViewport.Size.X && this.Size.Y <= currentViewport.Size.Y)
        {
            if (this.Position.X < 0)
            {
                this.Position = new Vector2(0, this.Position.Y);
            }
            else if (this.Position.X + this.Size.X > currentViewport.Size.X)
            {
                this.Position = new Vector2(currentViewport.Size.X - this.Size.X, this.Position.Y);
            }
            if (this.Position.Y < 0)
            {
                this.Position = new Vector2(this.Position.X, 0);
            }
            else if (this.Position.Y + this.Size.Y > currentViewport.Size.Y)
            {
                this.Position = new Vector2(this.Position.X, currentViewport.Size.Y - this.Size.Y);
            }
        }
        else
        {
            if (this.Size.X <= currentViewport.Size.X)
            {
                this.Position = new Vector2(this.Position.X, 0);
            }
            if (this.Size.Y <= currentViewport.Size.Y)
            {
                this.Position = new Vector2(0, this.Position.Y);
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
        wasAtBottom = entryScrollContainer.ScrollVertical == (int)entryScrollBar.MaxValue - (int)entryScrollBar.Page;
        
        Node newEntry = null;

        //determine if we need to pop an old entry to account for size limit
        if (entryParent.GetChildCount() >= maxEntries)
        {
            newEntry = entryParent.GetChild(0);

            //move to bottom
            entryParent.MoveChild(newEntry, -1);
        }
        else
        {
            newEntry = entryTemplate.Instantiate();
            entryParent.AddChild(newEntry);
        }

        //set entry icon and content
        GodotConsoleFeedEntry entryController = newEntry as GodotConsoleFeedEntry;
        entryController.SetType(argType);
        entryController.SetText(argTitle);
        entryController.SetTooltip(argContent);

        //hide the entry if a filter is active for it
        switch (entryController.logType)
        {
            case LogMode.Info:
                if (infoVisible == false)
                {
                    (newEntry as Control).Visible = false;
                }
                break;

            case LogMode.Warning:
                if (warningVisible == false)
                {
                    (newEntry as Control).Visible = false;
                }
                break;
                
            case LogMode.Error:
                if (errorVisible == false)
                {
                    (newEntry as Control).Visible = false;
                }
                break;
        }

        if (stayAtBottom == true && wasAtBottom == true)
        {
            ScrollToBottomDelayed();
        }
    }

    public void Call()
    {
        Call(inputArea.Text);
    }

    public void Call(string argCall)
    {
        if (string.IsNullOrWhiteSpace(inputArea.Text) == false)
        {
            lastCommand = inputArea.Text;
            Ascalon.Call(argCall, new AscalonCallContext(AscalonCallSource.User));
        }
        inputArea.Clear();
    }

    async private void ScrollToBottomDelayed()
    {
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        entryScrollContainer.ScrollVertical = (int)entryScrollBar.MaxValue;
    }

    #region UI Buttons
    public void CloseButtonPressed()
    {
        this.Visible = false;
    }

    public void ClearEntries()
    {
        foreach (Node getChild in entryParent.GetChildren())
        {
            getChild.QueueFree();
        }
    }

    public void ScrollToBottomButtonPressed()
    {
        entryScrollContainer.ScrollVertical = (int)entryScrollBar.MaxValue;
    }

    public void StayAtBottomCheckBoxChanged(bool argNewState)
    {
        stayAtBottom = argNewState;
    }

    public void ToggleInfoEntries()
    {
        wasAtBottom = entryScrollContainer.ScrollVertical == (int)entryScrollBar.MaxValue - (int)entryScrollBar.Page;
        foreach (Node getChild in entryParent.GetChildren())
        {
            if ((getChild as GodotConsoleFeedEntry).logType == LogMode.Info)
            {
                (getChild as Control).Visible = !infoVisible;
            }
        }
        FilterToggleScrollCheck();

        infoFilterButton.Modulate = (infoVisible == true ? new Color(0.5f, 0.5f, 0.5f) : new Color(1.0f, 1.0f, 1.0f));
        infoVisible = !infoVisible;
    }

    public void ToggleWarningEntries()
    {
        wasAtBottom = entryScrollContainer.ScrollVertical == (int)entryScrollBar.MaxValue - (int)entryScrollBar.Page;
        foreach (Node getChild in entryParent.GetChildren())
        {
            if ((getChild as GodotConsoleFeedEntry).logType == LogMode.Warning)
            {
                (getChild as Control).Visible = !warningVisible;
            }
        }
        FilterToggleScrollCheck();

        warningFilterButton.Modulate = (warningVisible == true ? new Color(0.5f, 0.5f, 0.5f) : new Color(1.0f, 1.0f, 1.0f));
        warningVisible = !warningVisible;
    }

    public void ToggleErrorEntries()
    {
        wasAtBottom = entryScrollContainer.ScrollVertical == (int)entryScrollBar.MaxValue - (int)entryScrollBar.Page;
        foreach (Node getChild in entryParent.GetChildren())
        {
            if ((getChild as GodotConsoleFeedEntry).logType == LogMode.Error)
            {
                (getChild as Control).Visible = !errorVisible;
            }
        }
        FilterToggleScrollCheck();

        errorFilterButton.Modulate = (errorVisible == true ? new Color(0.5f, 0.5f, 0.5f) : new Color(1.0f, 1.0f, 1.0f));
        errorVisible = !errorVisible;
    }

    private void FilterToggleScrollCheck()
    {
        if (stayAtBottom == true && wasAtBottom == true)
        {
            ScrollToBottomDelayed();
        }
    }
    #endregion

    #region Suggestions
    public void UpdateSuggestions(string argCurrentInput)
    {
        if (argCurrentInput.Length > 0)
        {
            if (argCurrentInput.TrimStart().Contains(" "))
            {
                string firstSegment = argCurrentInput.Split()[0];
                inputSuggestionLabel.Text = firstSegment;

                if (Ascalon.ConCommandExists(firstSegment))
                {
                    if (!Ascalon.GetConCommandEntry(firstSegment).flags.HasFlag(ConFlags.Hidden))
                    {
                        inputSuggestionLabel.Text += " (";
                        ConCommand passedCommand = Ascalon.GetConCommandEntry(firstSegment);
                        if (passedCommand.parms.Length > 0)
                        {
                            foreach (ParameterInfo parm in passedCommand.parms)
                            {
                                inputSuggestionLabel.Text += "" + parm.Name + ":" + parm.ParameterType + "  ";
                            }
                            inputSuggestionLabel.Text = inputSuggestionLabel.Text.Remove(inputSuggestionLabel.Text.Length - 2);
                            inputSuggestionLabel.Text += ")";
                        }
                        else
                        {
                            inputSuggestionLabel.Text = firstSegment + " (none)";
                        }
                    }
                }
                else if (Ascalon.ConVarExists(firstSegment))
                {
                    if (Ascalon.GetConVarEntry(firstSegment).flags.HasFlag(ConFlags.Hidden))
                    {
                        ConVar passedConVar = Ascalon.GetConVarEntry(firstSegment);
                        inputSuggestionLabel.Text += " " + passedConVar.GetData() + " (" + passedConVar.cvarDataType.ToString() + ")";
                    }
                }

                canUseAutoComplete = false;
            }
            else //we don't have a complete first segment yet
            {
                string inputName = "";
                for (int i = 0; i < argCurrentInput.Length; i++)
                {
                    if (char.IsLetterOrDigit(argCurrentInput[i]) || "_\"\'".Contains(argCurrentInput[i]))
                    {
                        inputName += argCurrentInput[i];
                    }
                    else
                    {
                        return;
                    }
                }
                inputName = inputName.ToLower();

                //todo: compile combined list of entries for efficiency?

                //add all matching commands
                System.Collections.Generic.List<string> matchingEntries = Ascalon.instance.conCommands.FindAll(ConCommand => ConCommand.name.StartsWith(inputName))
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
                    if (inputSuggestionLabel.Visible == false)
                    {
                        inputSuggestionLabel.Visible = true;
                    }
                    inputSuggestionLabel.Text = "";

                    int addedSuggestions = 0;
                    for (int i = 0; i < matchingEntries.Count; i++)
                    {
                        if (addedSuggestions == maxSuggestions)
                        {
                            break;
                        }

                        if (i == 0)
                        {
                            inputSuggestionLabel.Text += matchingEntries[i];
                            
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
                            inputSuggestionLabel.Text += "\n" + matchingEntries[i];
                        }
                        addedSuggestions++;
                    }

                    canUseAutoComplete = true;
                }
                else
                {
                    inputSuggestionLabel.Text = "";
                    inputSuggestionLabel.Visible = false;

                    canUseAutoComplete = false;
                }
            }
            
        }
        else
        {
            inputSuggestionLabel.Text = "";
            inputSuggestionLabel.Visible = false;
                    
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
