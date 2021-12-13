using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private float zoomTime = 5;
    [SerializeField] private float orthoSize = 5;

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
        GameManager.Instance.DisableIfHasBase();
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
            if(bases[i].localPositionX != 0 && bases[i].localPositionY != 0)
                Instantiate(cityPrefab, cities.transform,false).transform.localPosition =
                    new Vector3((float)bases[i].localPositionX, (float)bases[i].localPositionY);
        }
    }

    public async UniTask GetBases()
    {
        string gameBases = await ApiManager.Instance.GetTextAsync(WebRequests.GetBases);
        gameBases = JsonHelper.FixJson(gameBases);
        bases = JsonHelper.FromJson<Base>(gameBases); 
    }


}
