#if UNITY_2019_1_OR_NEWER
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UnityUIFeedEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //storage
    [HideInInspector] public LogMode logType;
    [HideInInspector] public string tooltip;
    [HideInInspector] public UnityUITooltipController tooltipController;


    [Header("References")]
    [SerializeField] private Image infoIcon;
    [SerializeField] private Image warningIcon;
    [SerializeField] private Image errorIcon;
    [SerializeField] private TextMeshProUGUI contentText;



    public void SetType(LogMode argType)
    {
        infoIcon.enabled = false;
        warningIcon.enabled = false;
        errorIcon.enabled = false;

        if (argType == LogMode.Info || argType == LogMode.InfoVerbose)
        {
            infoIcon.enabled = true;
            logType = LogMode.Info;
        }
        else if (argType == LogMode.Warning || argType == LogMode.WarningVerbose)
        {
            warningIcon.enabled = true;
            logType = LogMode.Warning;
        }
        else
        {
            errorIcon.enabled = true;
            logType = LogMode.Error;
        }
    }

    public void SetText(string argContent)
    {
        contentText.text = argContent;
    }

    public void SetTooltip(string argContent)
    {
        tooltip = argContent;
    }

    public void OnPointerEnter(PointerEventData argEventData)
    {
        if (tooltip != string.Empty)
        {
            tooltipController.Activate(tooltip);
        }
    }

    public void OnPointerExit(PointerEventData argEventData)
    {
        tooltipController.Deactivate();
    }
}
#endif
