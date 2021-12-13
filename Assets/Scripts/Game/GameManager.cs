using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private GameObject createBtn;

    [SerializeField] private GameObject deleteBtn;

    private static GameManager instance;
    public static GameManager Instance
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
    }

    public void DisableIfHasBase()
    {
        int id = ApiManager.Instance.token.user.id;
        foreach (Base pBase in Map.Instance.bases)
            if (pBase.client_id == id)
            {
                createBtn.SetActive(false);
                deleteBtn.SetActive(true);
                Map.Instance.playerBase = pBase;
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
}
