using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AuthenticationManager : MonoBehaviour
{

    [SerializeField]
    private TMP_InputField registerUsername = default;
    [SerializeField]
    private TMP_InputField registerPassword = default;

    [SerializeField]
    private TMP_InputField loginUsername = default;
    [SerializeField]
    private TMP_InputField loginPassword = default;

    private void Awake()
    {
        DontDestroyOnLoad(transform.parent);
    }

    public async void Login()
    {
        if(loginUsername.text == "")
        {
            Console.Instance.Print("Username is missing!");
            return;
        }
        if (loginPassword.text == "")
        {
            Console.Instance.Print("Password is missing!");
            return;
        }

        string result = await ApiManager.Instance.Login(loginUsername.text, loginPassword.text);

        if (result != null)
        {
            await SceneManager.LoadSceneAsync(1,LoadSceneMode.Single);
            gameObject.SetActive(false);
        }
    }

    public async void Register()
    {
        if (registerUsername.text == "")
        {
            Console.Instance.Print("Username is missing!");
            return;
        }
        if (registerPassword.text == "")
        {
            Console.Instance.Print("Password is missing!");
            return;
        }

        string result = await ApiManager.Instance.Register(registerUsername.text, registerPassword.text);

        if (result != null)
        {
            await SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
            gameObject.SetActive(false);
        }
    }
}
