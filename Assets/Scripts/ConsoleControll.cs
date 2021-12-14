using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleControll : MonoBehaviour
{
    [SerializeField]
    private GameObject console;

    [SerializeField]
    private RectTransform consoleLayout;

    private Vector2 originalSize;


    private void Start()
    {
        originalSize = consoleLayout.sizeDelta;
    }

    public void SetConsoleState()
    {
        bool state = console.activeSelf;
        console.SetActive(!state);

        consoleLayout.sizeDelta = state ? new Vector2(consoleLayout.sizeDelta.x, 0) : originalSize;
    }
}
