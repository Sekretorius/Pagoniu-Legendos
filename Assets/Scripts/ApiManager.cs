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
    /// delete base
    /// </summary>
    /// <returns></returns>
    public async UniTask<bool> DeleteBase(int id)
    {
        return await DeleteAsync(WebRequests.Bases + "/" + id);
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


}
