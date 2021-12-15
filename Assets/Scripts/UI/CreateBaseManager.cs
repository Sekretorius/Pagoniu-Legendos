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
    private int maxSectionBaseCount = 1;
    [SerializeField]
    private TMP_Dropdown dropdown;
    private Action<World, Base> onSubmit;
    private Action onInitDone;
    private List<World> worlds = new List<World>();

    [SerializeField]
    private Vector2 bounds = new Vector2(20, 20);
    [SerializeField]
    private float baseOffset = 1;
    [SerializeField]
    private int sectionDistance = 4;

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Init(Action onInitDone)
    {
        this.onInitDone = onInitDone;
        SetUpOptions();
    }

    public void Show(Action<World, Base> onSubmit)
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
            onSubmit.Invoke(null, null);
            return;
        }

        worldSectionsRequest = JsonHelper.FixJson(worldSectionsRequest);
        WorldSection[] worldSections = JsonHelper.FromJson<WorldSection>(worldSectionsRequest);

        string gameBasesRequest = await ApiManager.Instance.GetTextAsync(WebRequests.Worlds + "/" + world.id + "/" + "Bases");
        gameBasesRequest = JsonHelper.FixJson(gameBasesRequest);
        Base[] bases = JsonHelper.FromJson<Base>(gameBasesRequest);


        WorldSection selectedSection = null;

        SectionFilterResult filterResult = FilterWorldSections(worldSections, bases);
        if (filterResult.AvailableSections.Count == 0)
        {
            List<Vector2> availbleSectionPoints = GenerateAvailableSectionMap(new List<Vector2>(filterResult.FullSections.Values));
            selectedSection = await CreateNewSection(world.id, availbleSectionPoints);
        }
        else
        {
            List<WorldSection> availbleSections = new List<WorldSection>(filterResult.AvailableSections.Keys);
            selectedSection = availbleSections[UnityEngine.Random.Range(0, availbleSections.Count)];
        }

        float posX = UnityEngine.Random.Range(selectedSection.worldPositionX - baseOffset, selectedSection.worldPositionX + baseOffset + 1);
        float posY = UnityEngine.Random.Range(selectedSection.worldPositionY - baseOffset, selectedSection.worldPositionY + baseOffset + 1);

        Base playerBase = new Base(0, ApiManager.Instance.token.user.id, selectedSection.id, posX, posY);
        string baseCreationResult = await ApiManager.Instance.AddBase(playerBase);

        if (baseCreationResult == null)
        {
            onSubmit.Invoke(null, null);
            return;
        }

        onSubmit.Invoke(world, JsonUtility.FromJson<Base>(baseCreationResult));
    }
    private SectionFilterResult FilterWorldSections(WorldSection[] worldSections, Base[] worldBases)
    {
        SectionFilterResult sectionFilterResult = new SectionFilterResult();
        if (worldSections.Length == 0) return sectionFilterResult;

        foreach (WorldSection section in worldSections)
        {
            if (!sectionFilterResult.WorldSections.ContainsKey(section)) sectionFilterResult.WorldSections.Add(section, new List<Base>());

            foreach(Base worldBase in worldBases)
            {
                if (section.id == worldBase.world_section_id)
                    sectionFilterResult.WorldSections[section].Add(worldBase);
            }

            if (sectionFilterResult.WorldSections[section].Count < maxSectionBaseCount)
            {
                sectionFilterResult.AvailableSections.Add(section, new Vector2(section.worldPositionX, section.worldPositionY));
            }
            else
            {
                sectionFilterResult.FullSections.Add(section, new Vector2(section.worldPositionX, section.worldPositionY));
            }
        }
        return sectionFilterResult;
    }

    private async UniTask<WorldSection> CreateNewSection(int worldId, List<Vector2> availableWorldSections)
    {
        Vector2 position = availableWorldSections[UnityEngine.Random.Range(0, availableWorldSections.Count)];

        WorldSection newWorldSection = new WorldSection()
        {
            worldPositionX = position.x,
            worldPositionY = position.y,
            world_id = worldId,
        };

        string worldSectionResult = await ApiManager.Instance.CreateWorldSection(newWorldSection);

        if (worldSectionResult == null)
        {
            onSubmit.Invoke(null, null);
            return null;
        }

        return JsonUtility.FromJson<WorldSection>(worldSectionResult);
    }

    private List<Vector2> GenerateAvailableSectionMap(List<Vector2> worldSections)
    {
        List<Vector2> worldSectionMap = new List<Vector2>();
        for (int x = -(int)bounds.x / 2; x < bounds.x / 2; x += sectionDistance)
        {
            for (int y = -(int)bounds.y / 2; y < bounds.x / 2; y += sectionDistance)
            {
                Vector2 newPoint = new Vector2(x, y);
                if (!worldSections.Contains(newPoint))
                    worldSectionMap.Add(newPoint);
            }
        }
        return worldSectionMap;
    }


    internal class SectionFilterResult
    {
        public Dictionary<WorldSection, List<Base>> WorldSections { get; set; } = new Dictionary<WorldSection, List<Base>>();
        public Dictionary<WorldSection, Vector2> AvailableSections { get; set; } = new Dictionary<WorldSection, Vector2>();
        public Dictionary<WorldSection, Vector2> FullSections { get; set; } = new Dictionary<WorldSection, Vector2>();
    }
}
