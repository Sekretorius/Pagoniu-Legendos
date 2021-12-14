using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerCity : SerializedMonoBehaviour
{
    private Building[] buildings;

    public Citizen citizens;
    public Worker workers;
    public Soldier soldiers;

    [SerializeField]
    public CityBuilding house;

    [SerializeField]
    public CityBuilding barracks;

    [SerializeField]
    public CityBuilding blacksmith;

    [SerializeField]
    public Button buyBtn;

    [SerializeField]
    public TextMeshProUGUI buyBtnText;

    [SerializeField]
    public Button delBtn;

    [SerializeField]
    public TextMeshProUGUI delBtnText;

    [SerializeField]
    public Button editBtn;

    [SerializeField]
    public TextMeshProUGUI editBtnText;

    [SerializeField]
    public Button addBtn;

    [SerializeField]
    public TextMeshProUGUI addBtnText;

    [SerializeField]
    public Button deletePeopleBtn;

    [SerializeField]
    public TextMeshProUGUI deletePeopleBtnText;

    [SerializeField]
    public InfoBox info;

    [SerializeField]
    public GameObject edit;

    [SerializeField]
    public TMP_InputField editText;

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
        citizens = GameManager.Instance.citizens;
        workers = GameManager.Instance.workers;
        soldiers = GameManager.Instance.soldiers;
    }

    void Start()
    {
        RefreshBuildings();
    }

    void RefreshBuildings()
    {
        RefreshInfo();
        foreach (CityBuilding city in cityBuildings.Values)
            city.DisableBuilding();
        if (buildings != null)
            foreach (Building built in buildings)
                EnableBuilding(built);
    }

    void RefreshInfo()
    {
        house.customInfo = citizens != null ? "Citizens: " + citizens.count : "Citizens: " + 0;
        blacksmith.customInfo = workers != null ? "Workers: " + workers.count : "Workers: " + 0;
        barracks.customInfo = soldiers != null ? "Soldiers: " + soldiers.count : "Soldiers: " + 0;
        if(selected != null)
            info.SetInfo(selected.building.name, selected.building.type, selected.customInfo);
    }

    void EnableBuilding(Building built)
    {
        if (cityBuildings.ContainsKey(built.type))
        {
            cityBuildings[built.type].building = built;
            cityBuildings[built.type].EnableBuilding();
        }
    }

    public void AddBuilding()
    {
        buyBtn.gameObject.SetActive(false);
        AddPlayerBuilding();
    }

    public async UniTask AddPlayerBuilding()
    {
        selected.EnableBuilding();
        await ApiManager.Instance.AddBuilding(GameManager.Instance.currentBase.id, selected.buildingName, editText.text);
        string buildingData = await ApiManager.Instance.GetBuildings(GameManager.Instance.currentBase.id);
        buildingData = JsonHelper.FixJson(buildingData);
        buildings = JsonHelper.FromJson<Building>(buildingData);
        GameManager.Instance.buildings = buildings;
        RefreshBuildings();
    }

    public void DeleteBuilding()
    {
        delBtn.gameObject.SetActive(false);
        editBtn.gameObject.SetActive(false);
        addBtn.gameObject.SetActive(false);
        DeletePlayerBuilding();
    }

    public async UniTask DeletePlayerBuilding()
    {
        selected.DisableBuilding();
        await ApiManager.Instance.DeleteBuilding(GameManager.Instance.currentBase.id, selected.building.id);
        string buildingData = await ApiManager.Instance.GetBuildings(GameManager.Instance.currentBase.id);
        buildingData = JsonHelper.FixJson(buildingData);
        buildings = JsonHelper.FromJson<Building>(buildingData);
        GameManager.Instance.buildings = buildings;
        RefreshBuildings();
    }

    public void EditBuilding()
    {
        editBtn.gameObject.SetActive(false);
        delBtn.gameObject.SetActive(false);
        EditPlayerBuilding();
    }

    public async UniTask EditPlayerBuilding()
    {
        selected.building.name = PlayerCity.instance.editText.text;
        edit.SetActive(false);
        await ApiManager.Instance.EditBuilding(selected.building);

        string buildingData = await ApiManager.Instance.GetBuildings(GameManager.Instance.currentBase.id);
        buildingData = JsonHelper.FixJson(buildingData);
        buildings = JsonHelper.FromJson<Building>(buildingData);
        GameManager.Instance.buildings = buildings;
        RefreshBuildings();
    }

    public void DeletePeople()
    {
        DeletePlayerPeople();
    }

    public async void DeletePlayerPeople()
    {
        string type = selected.building.type;
        switch (type)
        {
            case "House":
                await ApiManager.Instance.DeleteCitizens(citizens.id);
                citizens.count = 0;
                break;
            case "Blacksmith":
                await ApiManager.Instance.DeleteWorkers(workers.id);
                workers.count = 0;
                break;
            case "Barracks":
                await ApiManager.Instance.DeleteSoldiers(soldiers.id);
                soldiers.count = 0;
                break;
        }
        RefreshInfo();
    }

    public void AddPeople()
    {
        AddPlayerPeople();
    }

    public async UniTask AddPlayerPeople()
    {
        citizens.base_id = selected.building.base_id;
        workers.base_id = selected.building.base_id;
        soldiers.base_id = selected.building.base_id;
        string type = selected.building.type;
        switch (type)
        {
            case "House":
                if (citizens != null)
                {
                    citizens.count += 10;
                    await ApiManager.Instance.AddCitizen(citizens);
                }
                else
                {
                    Citizen temp = new Citizen();
                    temp.count = 10;
                    await ApiManager.Instance.UpdateCitizen(temp);
                }
                break;
            case "Blacksmith":
                if (workers != null)
                {
                    workers.count += 10;
                    await ApiManager.Instance.AddWorkers(workers);
                }
                else
                {
                    Worker temp = new Worker();
                    temp.count = 10;
                    await ApiManager.Instance.UpdateWorkers(temp);
                }
                break;
            case "Barracks":
                if (soldiers != null)
                {
                    soldiers.count += 10;
                    await ApiManager.Instance.AddSoldiers(soldiers);
                }
                else
                {
                    Soldier temp = new Soldier();
                    temp.count = 10;
                    await ApiManager.Instance.UpdateSoldiers(temp);
                }
                break;
        }
        RefreshInfo();
    }

    public void OpenMap()
    {
        SceneManager.LoadSceneAsync(1,LoadSceneMode.Single);
    }
}
