using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class City : MonoBehaviour
{

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite basicSprite;
    [SerializeField] private Sprite playerSprite;
    [SerializeField] private Vector2 position;

    [SerializeField] public Base cityBase;

    public async UniTask OpenCity()
    {
        GameManager.Instance.currentBase = cityBase;

        string buildingData = await ApiManager.Instance.GetBuildings(cityBase.id);
        buildingData = JsonHelper.FixJson(buildingData);
        Building[] building = JsonHelper.FromJson<Building>(buildingData);
        GameManager.Instance.buildings = building;

        string citizenData = await ApiManager.Instance.GetCitizens(cityBase.id);

        string workerData = await ApiManager.Instance.GetWorkers(cityBase.id);
        
        string soldierData = await ApiManager.Instance.GetSoldiers(cityBase.id);

        try { GameManager.Instance.citizens = JsonHelper.FromJson<Citizen>(JsonHelper.FixJson(citizenData))[0]; }
        catch { }

        try { GameManager.Instance.workers = JsonHelper.FromJson<Worker>(JsonHelper.FixJson(workerData))[0]; }
        catch { }

        try { GameManager.Instance.soldiers = JsonHelper.FromJson<Soldier>(JsonHelper.FixJson(soldierData))[0]; }
        catch { }

        SceneManager.LoadScene(2, LoadSceneMode.Single);
    }

    private void OnMouseDown()
    {
        OpenCity();
    }

    public void SetPlayer()
    {
        spriteRenderer.sprite = playerSprite;
    }

}
