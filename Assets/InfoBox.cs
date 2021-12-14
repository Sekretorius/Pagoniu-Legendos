using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoBox : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI name;
    [SerializeField] public TextMeshProUGUI type;
    [SerializeField] public TextMeshProUGUI custom;


    public void SetInfo(string nameInfo = "", string typeInfo = "", string customInfo = "")
    {
        name.text = nameInfo;
        type.text = typeInfo;
        custom.text = customInfo;
    }
}
