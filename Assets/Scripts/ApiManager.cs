using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ApiManager : MonoBehaviour
{

    const string HeaderName = "Authorization";
    const string HeaderValue = "Bearer " + "test";

    public string playerID { get; private set; }

    public Token token { get; private set; }


    private static ApiManager instance;


    public static ApiManager Instance
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

        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Safest login form in the west
    /// </summary>
    /// <param name="username">username</param>
    /// <param name="password">encrypted password</param>
    /// <returns></returns>
    public async UniTask<string> Login(string username, string password)
    {
        Dictionary<string, string> form = new Dictionary<string, string>();
        form.Add("username", username);
        form.Add("password", password);

        return await GetTextAsync(WebRequests.Login, form);
    }

    /// <summary>
    /// Safest register form in the east
    /// </summary>
    /// <param name="username">username</param>
    /// <param name="password">encrypted password</param>
    /// <returns></returns>
    public async UniTask<string> Register(string username, string password)
    {
        Dictionary<string, string> form = new Dictionary<string, string>();
        form.Add("username", username);
        form.Add("password", password);

        return await GetTextAsync(WebRequests.Register, form);
    }

    /// <summary>
    /// add base
    /// </summary>
    /// <returns></returns>
    public async UniTask<string> AddBase(Base playerBase)
    {
        Dictionary<string,string> form = new Dictionary<string, string>();

        form.Add("localPositionX", playerBase.localPositionX.ToString());
        form.Add("localPositionY", playerBase.localPositionY.ToString());
        form.Add("client_id", playerBase.client_id.ToString());
        form.Add("world_section_id", playerBase.world_section_id.ToString());


        return await GetTextAsync(WebRequests.Bases, form);
    }

    /// <summary>
    /// add base
    /// </summary>
    /// <returns></returns>
    public async UniTask<string> GetUserBase(int playerId)
    {
        return await GetTextAsync(WebRequests.GetUsers + "/" + playerId + "/Base");
    }

    public void GetWorldSections(int worldId, System.Action<string> onDone)
    {
        StartCoroutine(GetText(WebRequests.Worlds + "/" + worldId + "/WorldSections", onDone));
    }

    public async UniTask<string> GetWorldSections(int worldId)
    {
        return await GetTextAsync(WebRequests.Worlds + "/" + worldId + "/WorldSections");
    }

    public async UniTask<string> GetWorldSection(int sectionId)
    {
        return await GetTextAsync(WebRequests.WorldSections + "/" + sectionId);
    }

    public async UniTask<string> CreateWorldSection(WorldSection worldSection)
    {
        Dictionary<string, string> form = new Dictionary<string, string>();

        form.Add("worldPositionX", worldSection.worldPositionX.ToString());
        form.Add("worldPositionY", worldSection.worldPositionY.ToString());
        form.Add("world_id", worldSection.world_id.ToString());
        form.Add("baseCount", 0.ToString());

        return await GetTextAsync(WebRequests.WorldSections, form);
    }

    public void GetWorldsSync(System.Action<string> onDone)
    {
        StartCoroutine(GetText(WebRequests.Worlds, onDone));
    }

    public async UniTask<string> GetWorlds()
    {
        return await GetTextAsync(WebRequests.Worlds);
    }

    public void AddBaseSync(Base playerBase, System.Action<string> onDone)
    {
        Dictionary<string, string> form = new Dictionary<string, string>();

        form.Add("localPositionX", playerBase.localPositionX.ToString());
        form.Add("localPositionY", playerBase.localPositionY.ToString());
        form.Add("client_id", playerBase.client_id.ToString());
        form.Add("world_section_id", playerBase.world_section_id.ToString());

        StartCoroutine(GetText(WebRequests.Bases, form, onDone));
    }

    /// <summary>
    /// delete base
    /// </summary>
    /// <returns></returns>
    public async UniTask<bool> DeleteBase(int id)
    {
        return await DeleteAsync(WebRequests.Bases + "/" + id);
    }

    /// <summary>
    /// add base
    /// </summary>
    /// <returns></returns>
    public async UniTask<string> GetBuildings(int baseId)
    {
        return await GetTextAsync(WebRequests.GetBase + baseId + "/Buildings");
    }

    public async UniTask<string> AddBuilding(int baseId, string type)
    {

        Dictionary<string, string> form = new Dictionary<string, string>();

        form.Add("base_id", baseId.ToString());
        form.Add("type", type);

        return await GetTextAsync(WebRequests.GetBase + baseId + "/Buildings",form);
    }

    public async UniTask<string> GetTextAsync(string request)
    {
        UnityWebRequest req = UnityWebRequest.Get(request);

        req.SetRequestHeader("Bearer", token != null ? token.access_token : string.Empty);
        req.SetRequestHeader("Username", token != null ? token.user.username : string.Empty);
        try
        {
            await req.SendWebRequest();
 
            if (req.result == UnityWebRequest.Result.Success)
            {
                Console.Instance.Print($"{request} : {req.result}");
                Console.Instance.Print(req.downloadHandler.text, "green");

                return req.downloadHandler.text;
            }

            Console.Instance.Print($"{request} : {req.result}");
            Console.Instance.Print(req.downloadHandler.text, "red");
        }
        catch (UnityWebRequestException e)
        {
            Console.Instance.Print(request);
            Console.Instance.Print(e.Text, "red");
        }

        return null;
    }

    public async UniTask<string> GetTextAsync(string request, Dictionary<string,string> form)
    {

        UnityWebRequest req = UnityWebRequest.Post(request, form);

        req.SetRequestHeader("Bearer", token != null ? token.access_token : string.Empty);
        req.SetRequestHeader("Username", token != null ? token.user.username : string.Empty);

        try
        {
            await req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                Console.Instance.Print($"{request} : {req.result}");
                Console.Instance.Print(req.downloadHandler.text, "green");

                if (token == null)
                    token = JsonUtility.FromJson<Token>(req.downloadHandler.text);

                return req.downloadHandler.text;
            }

            Console.Instance.Print($"{request} : {req.result}");
            Console.Instance.Print(req.downloadHandler.text, "red");
        }
        catch (UnityWebRequestException e)
        {
            Console.Instance.Print(request);
            Console.Instance.Print(e.Text, "red");
        }
        return null;
    }

    public async UniTask<bool> DeleteAsync(string request)
    {

        UnityWebRequest req = UnityWebRequest.Delete(request);

        req.SetRequestHeader("Bearer", token != null ? token.access_token : string.Empty);
        req.SetRequestHeader("Username", token != null ? token.user.username : string.Empty);

        try
        {
            await req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                Console.Instance.Print($"{request} : {req.result}");
                return true;
            }

            Console.Instance.Print($"{request} : {req.result}");
            Console.Instance.Print(req.downloadHandler.text, "red");
        }
        catch (UnityWebRequestException e)
        {
            Console.Instance.Print(request);
            Console.Instance.Print(e.Text, "red");
        }
        return false;
    }

    public IEnumerator GetText(string request, System.Action<string> onDone)
    {
        if (onDone == null) yield break;
        UnityWebRequest req = UnityWebRequest.Get(request);

        req.SetRequestHeader("Bearer", token != null ? token.access_token : string.Empty);
        req.SetRequestHeader("Username", token != null ? token.user.username : string.Empty);
        try
        {
            req.SendWebRequest();
            while (!req.isDone) ;
        }
        catch (UnityWebRequestException e)
        {
            Console.Instance.Print(request);
            Console.Instance.Print(e.Text, "red");
        }

        if (req.result == UnityWebRequest.Result.Success)
        {
            Console.Instance.Print($"{request} : {req.result}");
            Console.Instance.Print(req.downloadHandler.text, "green");

            onDone.Invoke(req.downloadHandler.text);
        }
        else 
        {
            Console.Instance.Print($"{request} : {req.result}");
            Console.Instance.Print(req.downloadHandler.text, "red");

            onDone.Invoke(null);
        }
    }

    public IEnumerator GetText(string request, Dictionary<string, string> form, System.Action<string> onDone)
    {
        if (onDone == null) yield break;
        UnityWebRequest req = UnityWebRequest.Post(request, form);

        req.SetRequestHeader("Bearer", token != null ? token.access_token : string.Empty);
        req.SetRequestHeader("Username", token != null ? token.user.username : string.Empty);
        try
        {
            req.SendWebRequest();
            while (!req.isDone) ;
        }
        catch (UnityWebRequestException e)
        {
            Console.Instance.Print(request);
            Console.Instance.Print(e.Text, "red");
        }

        if (req.result == UnityWebRequest.Result.Success)
        {
            Console.Instance.Print($"{request} : {req.result}");
            Console.Instance.Print(req.downloadHandler.text, "green");

            onDone.Invoke(req.downloadHandler.text);
        }
        else
        {
            Console.Instance.Print($"{request} : {req.result}");
            Console.Instance.Print(req.downloadHandler.text, "red");

            onDone.Invoke(null);
        }
    }
}
