using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleButton : MonoBehaviour
{
    [SerializeField]
    public TMP_Text text;

    [SerializeField]
    public Button button;

    [SerializeField]
    public TMP_InputField inputField;

    private ButtonInfo btnInfo;

    public void Init(ButtonInfo info)
    {
        btnInfo = info;
        text.text = btnInfo.name;
        inputField.gameObject.SetActive(btnInfo.textField);

        if (!btnInfo.textField)
            button.onClick.AddListener( async () => await ApiManager.Instance.GetTextAsync(btnInfo.request));
        else
            button.onClick.AddListener( async () => await ApiManager.Instance.GetTextAsync($"{btnInfo.request}{inputField.text}"));
    }
}
