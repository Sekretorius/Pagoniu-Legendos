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
        return await GetTextAsync(WebRequests.Login + $"{username}/{password}");
    }

    /// <summary>
    /// Safest register form in the east
    /// </summary>
    /// <param name="username">username</param>
    /// <param name="password">encrypted password</param>
    /// <returns></returns>
    public async UniTask<string> Register(string username, string password)
    {
        return await GetTextAsync(WebRequests.Register + $"{username}/{password}");
    }

    public async UniTask<string> GetTextAsync(string request)
    {
        UnityWebRequest req = UnityWebRequest.Get(request);
        req.SetRequestHeader(HeaderName, HeaderValue);
        var result = await req.SendWebRequest();

        Console.Instance.Print($"{request} : {result.result}");
        Console.Instance.Print(result.downloadHandler.text,"red");

        return result.downloadHandler.text;

    }


}
