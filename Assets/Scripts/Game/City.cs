using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class City : MonoBehaviour
{

    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Vector2 position;

    [SerializeField] public Base cityBase;

    public async UniTask OpenCity()
    {
        GameManager.Instance.currentBase = cityBase;

        string buildingData = await ApiManager.Instance.GetBuildings(cityBase.id);
        buildingData = JsonHelper.FixJson(buildingData);
        Building[] building = JsonHelper.FromJson<Building>(buildingData);
        GameManager.Instance.buildings = building;

        await SceneManager.LoadSceneAsync(2, LoadSceneMode.Single);
    }

    private void OnMouseDown()
    {
        OpenCity();
    }

}
