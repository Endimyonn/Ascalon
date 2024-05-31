#if UNITY_2019_1_OR_NEWER
using TMPro;
using UnityEngine;

public class UnityEntryFieldHandler : MonoBehaviour
{
    [SerializeField] private UnityConsoleUIProxy uiProxy;
    private TMP_InputField localInputField;



    private void OnValidate()
    {
        if (localInputField == null)
        {
            TryGetComponent<TMP_InputField>(out localInputField);
        }
    }

    private void Awake()
    {
        if (localInputField == null)
        {
            TryGetComponent<TMP_InputField>(out localInputField);
        }
    }

    public void CheckKey()
    {
        if (Input.GetKey(KeyCode.Return)) //submit
        {
            uiProxy.Call();
            localInputField.ActivateInputField();
        }
    }
}
#endif
