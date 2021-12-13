using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCity : SerializedMonoBehaviour
{
    private Building[] buildings;

    [SerializeField]
    public Button buyBtn;

    [SerializeField]
    public TextMeshProUGUI buyBtnText;

    [OdinSerialize]
    private Dictionary<string, CityBuilding> cityBuildings;

    public CityBuilding selected;

    private static PlayerCity instance;
    public static PlayerCity Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;

        buildings = GameManager.Instance.buildings;
    }

    void Start()
    {
        RefreshBuildings();
    }

    void RefreshBuildings()
    {
        if (buildings != null)
            foreach (Building built in buildings)
                EnableBuilding(built.type);
    }

    void EnableBuilding(string type)
    {
        if (cityBuildings.ContainsKey(type))
            cityBuildings[type].EnableBuilding();
    }

    public void AddBuilding()
    {
        AddPlayerBuilding();
    }

    public async UniTask AddPlayerBuilding()
    {
        await ApiManager.Instance.AddBuilding(GameManager.Instance.currentBase.id, selected.buildingName);

        string buildingData = await ApiManager.Instance.GetBuildings(GameManager.Instance.currentBase.id);
        buildingData = JsonHelper.FixJson(buildingData);
        buildings = JsonHelper.FromJson<Building>(buildingData);
        GameManager.Instance.buildings = buildings;
        RefreshBuildings();
    }
}
