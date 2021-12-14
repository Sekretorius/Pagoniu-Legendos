using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Console : MonoBehaviour
{
    private static Console instance;

    public static Console Instance
    {
        get
        {
            return instance;
        }
    }

    [SerializeField]
    private TMP_Text textField;
    [SerializeField]
    private ScrollRect scrollRect;

    [SerializeField]
    private Transform buttonField;

    [SerializeField]
    private GameObject button;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SetupConsoleButtons();
    }

    private void SetupConsoleButtons()
    {
        List<ButtonInfo> buttons = new List<ButtonInfo>()
        {
            new ButtonInfo("GET BASES",WebRequests.GetBases, false),
            new ButtonInfo("GET BASE",WebRequests.GetBase, true),
            new ButtonInfo("GET USERS",WebRequests.GetUsers, false),
            new ButtonInfo("GET USER",WebRequests.GetUser, true),
        };

        foreach (ButtonInfo btn in buttons)
        {
            GameObject tempBtn = Instantiate(button, buttonField);
            ConsoleButton consoleBtn = tempBtn.GetComponent<ConsoleButton>();
            consoleBtn.Init(btn);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(buttonField.GetComponent<RectTransform>());
    }

    public void SetStateConsole(bool state)
    {
        gameObject.SetActive(state);
    }

    public void Print(string text, string color = "white")
    {
        text = $"<color={color}>{text}</color> \n";
        textField.text += text;
        scrollRect.verticalNormalizedPosition = 0;
        Debug.Log(text);
    }
}

public readonly struct ButtonInfo
{
    public ButtonInfo(string _name, string _request, bool _textField)
    {
        name = _name;
        request = _request;
        textField = _textField;
    }

    public string name { get; }
    public string request { get; }

    public bool textField { get; }

}
