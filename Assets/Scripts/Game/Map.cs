using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Map : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private float zoomTime = 5;
    [SerializeField] private float orthoSize = 5;

    [SerializeField] private GameObject createBtn;
    [SerializeField] private GameObject deleteBtn;

    [SerializeField] public GameObject cities;
    [SerializeField] public GameObject cityPrefab;
    [SerializeField] public Bounds bounds;

    public Base[] bases { get; private set; }

    public Base playerBase;

    public static Map Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    public async void Init()
    {
        await GetBases();
        DisableIfHasBase();
        SpawnCities();
        StartCoroutine(ZoomIn());
    }


    IEnumerator ZoomIn()
    {
        float size = _camera.orthographicSize;

        float elapsedTime = 0;
        
        while (elapsedTime < zoomTime)
        {
            elapsedTime += Time.deltaTime;
            _camera.orthographicSize = Mathf.Lerp(size, orthoSize, elapsedTime / zoomTime);
            yield return null;
        }     
    }

    public void SpawnCities()
    {

        if(cities.transform.childCount > 0)
        {
            foreach (Transform city in cities.transform)
                Destroy(city.gameObject);
        }

        for (int i = 0; i < bases.Length; i++)
        {
            if (bases[i].localPositionX != 0 && bases[i].localPositionY != 0)
            {
                GameObject cityObject = Instantiate(cityPrefab, cities.transform, false);
                cityObject.transform.localPosition = new Vector3((float)bases[i].localPositionX, (float)bases[i].localPositionY);
                cityObject.GetComponent<City>().cityBase = bases[i];
                if (bases[i].client_id == playerBase.client_id)
                    cityObject.GetComponent<City>().SetPlayer();
            }
        }
    }

    public async UniTask GetBases()
    {
        string gameBases = await ApiManager.Instance.GetTextAsync(WebRequests.GetBases);
        gameBases = JsonHelper.FixJson(gameBases);
        bases = JsonHelper.FromJson<Base>(gameBases); 
    }

    public void DisableIfHasBase()
    {
        int id = ApiManager.Instance.token.user.id;
        foreach (Base pBase in Map.Instance.bases)
            if (pBase.client_id == id)
            {
                createBtn.SetActive(false);
                deleteBtn.SetActive(true);
                playerBase = pBase;
                GameManager.Instance.playerBase = pBase;
                return;
            }
        createBtn.SetActive(true);
        deleteBtn.SetActive(false);
    }
    public void CreateBase()
    {
        InitNewBase();
    }

    public void DeleteBase()
    {
        DeletePlayerBase();
    }

    public async UniTask DeletePlayerBase()
    {
        deleteBtn.SetActive(false);
        await ApiManager.Instance.DeleteBase(Map.Instance.playerBase.id);
        await RefreshBases();
        DisableIfHasBase();
    }

    public async UniTask InitNewBase()
    {
        Bounds bounds = Map.Instance.bounds;
        int x = Mathf.RoundToInt(Random.Range(-bounds.extents.x, bounds.extents.x));
        int y = Mathf.RoundToInt(Random.Range(-bounds.extents.y, bounds.extents.y));

        Base playerBase = new Base(0, ApiManager.Instance.token.user.id, 1, x, y);

        createBtn.SetActive(false);
        await ApiManager.Instance.AddBase(playerBase);
        await RefreshBases();
        DisableIfHasBase();
    }

    public async UniTask RefreshBases()
    {
        await Map.Instance.GetBases();
        Map.Instance.SpawnCities();
    }
    public async UniTask OpenCity()
    {

        string buildingData = await ApiManager.Instance.GetBuildings(playerBase.id);
        buildingData = JsonHelper.FixJson(buildingData);
        Building[] building = JsonHelper.FromJson<Building>(buildingData);
        GameManager.Instance.buildings = building;

        string citizenData = await ApiManager.Instance.GetCitizens(playerBase.id);

        string workerData = await ApiManager.Instance.GetWorkers(playerBase.id);

        string soldierData = await ApiManager.Instance.GetSoldiers(playerBase.id);

        try { GameManager.Instance.citizens = JsonHelper.FromJson<Citizen>(JsonHelper.FixJson(citizenData))[0]; }
        catch { }

        try { GameManager.Instance.workers = JsonHelper.FromJson<Worker>(JsonHelper.FixJson(workerData))[0]; }
        catch { }

        try { GameManager.Instance.soldiers = JsonHelper.FromJson<Soldier>(JsonHelper.FixJson(soldierData))[0]; }
        catch { }

        SceneManager.LoadScene(2, LoadSceneMode.Single);
    }

    public void OpenPlayerCity()
    {
        OpenCity();
    }

}
