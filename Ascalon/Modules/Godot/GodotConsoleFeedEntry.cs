#if GODOT
using Godot;
using System;

public partial class GodotConsoleFeedEntry : Control
{
    //storage
    public LogMode logType;


    [Export] private Control infoIcon;
    [Export] private Control warningIcon;
    [Export] private Control errorIcon;
    [Export] private Label contentLabel;



    public void SetType(LogMode argType)
    {
        infoIcon.Visible = false;
        warningIcon.Visible = false;
        errorIcon.Visible = false;

        if (argType == LogMode.Info || argType == LogMode.InfoVerbose)
        {
            infoIcon.Visible = true;
            logType = LogMode.Info;
        }
        else if (argType == LogMode.Warning || argType == LogMode.WarningVerbose)
        {
            warningIcon.Visible = true;
            logType = LogMode.Warning;
        }
        else
        {
            errorIcon.Visible = true;
            logType = LogMode.Error;
        }
    }

    public void SetText(string argContent)
    {
        contentLabel.Text = argContent;
    }

    public void SetTooltip(string argContent)
    {
        this.TooltipText = argContent;
    }
}
#endif
