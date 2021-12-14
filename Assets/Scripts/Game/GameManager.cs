using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Base playerBase;

    public Base currentBase;

    public Building[] buildings;

    public Citizen citizens;
    public Worker workers;
    public Soldier soldiers;

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

        DontDestroyOnLoad(this);
    }

}
