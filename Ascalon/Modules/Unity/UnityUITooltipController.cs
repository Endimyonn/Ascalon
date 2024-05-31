#if UNITY_2019_1_OR_NEWER
using UnityEngine;
using TMPro;

public class UnityUITooltipController : MonoBehaviour
{
    [SerializeField] private RectTransform tooltipObject;
    [SerializeField] private TextMeshProUGUI tooltipText;


    [SerializeField] private Vector3 offset = new Vector3(25.0f, 25.0f);



    private void Awake()
    {
        Deactivate();
    }

    public void Activate(string argTooltip)
    {
        tooltipText.text = argTooltip;
        tooltipObject.gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        tooltipObject.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (tooltipObject.gameObject.activeSelf == true && Application.isFocused == true)
        {
            tooltipObject.anchoredPosition = Input.mousePosition + offset;
        }
    }
}
#endif
