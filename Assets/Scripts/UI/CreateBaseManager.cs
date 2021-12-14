using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
public class CreateBaseManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown dropdown;
    private Action<Base> onSubmit;
    private Action onInitDone;

    private List<World> worlds = new List<World>();
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Init(Action onInitDone)
    {
        this.onInitDone = onInitDone;
        SetUpOptions();
    }

    public void Show(Action<Base> onSubmit)
    {
        this.onSubmit = onSubmit;
        gameObject.SetActive(true);
    }

    private async void SetUpOptions()
    {
        dropdown.options.Clear();
        worlds.Clear();

        string requestData = await ApiManager.Instance.GetWorlds();

        if (requestData == null) return;

        requestData = JsonHelper.FixJson(requestData);
        World[] worldArray = JsonHelper.FromJson<World>(requestData);

        foreach (World world in worldArray)
        {
            TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData();
            data.text = "World " + world.id;
            worlds.Add(world);
            dropdown.options.Add(data);
        }
        onInitDone.Invoke();
    }

    public void OnSubmit()
    {
        if(onSubmit != null)
        {
            CreateBase();
        }
    }

    public async void CreateBase()
    {
        World world = worlds[dropdown.value];
        string worldSectionsRequest = await ApiManager.Instance.GetWorldSections(world.id);
        if (worldSectionsRequest == null)
        {
            onSubmit.Invoke(null);
            return;
        }

        worldSectionsRequest = JsonHelper.FixJson(worldSectionsRequest);
        WorldSection[] worldSections = JsonHelper.FromJson<WorldSection>(worldSectionsRequest);

        WorldSection selectedSection = null;

        if (worldSections.Length == 0)
        {
            WorldSection newWorldSection = new WorldSection()
            {
                worldPositionX = UnityEngine.Random.Range(-4, 4),
                worldPositionY = UnityEngine.Random.Range(-4, 4),
                world_id = world.id,
            };
            string worldSectionResult = await ApiManager.Instance.CreateWorldSection(newWorldSection);
            selectedSection = newWorldSection;
            if (worldSectionResult == null)
            {
                onSubmit.Invoke(null);
                return;
            }
        }
        else selectedSection = worldSections[UnityEngine.Random.Range(0, worldSections.Length)];

        int posX = UnityEngine.Random.Range(selectedSection.worldPositionX - 1, selectedSection.worldPositionX + 2);
        int posY = UnityEngine.Random.Range(selectedSection.worldPositionY - 1, selectedSection.worldPositionY + 2);

        Base playerBase = new Base(0, ApiManager.Instance.token.user.id, selectedSection.id, posX, posY);
        string baseCreationResult = await ApiManager.Instance.AddBase(playerBase);

        if (baseCreationResult == null)
        {
            onSubmit.Invoke(null);
            return;
        }

        GameManager.Instance.currentWorld = world;

        onSubmit.Invoke(playerBase);
    }
}
