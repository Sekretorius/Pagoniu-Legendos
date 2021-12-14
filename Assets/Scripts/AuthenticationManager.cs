using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
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

    [SerializeField]
    private GameObject authenticateWindow;
    [SerializeField]
    private CreateBaseManager worldSelectionWidow;

    private void Awake()
    {
        worldSelectionWidow.Hide();
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
            FinishPlayerSetUp();
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
            FinishPlayerSetUp();
        }
    }

    private async void FinishPlayerSetUp()
    {
        string result = await ApiManager.Instance.GetUserBase(ApiManager.Instance.token.user.id);
        
        if (result != null && result != "404")
        {
            try
            {
                Base playerBase = JsonUtility.FromJson<Base>(result);
                GameManager.Instance.playerBase = playerBase;

                string worldSectionResult = await ApiManager.Instance.GetWorldSection(playerBase.world_section_id);
                if (worldSectionResult == null) return;

                WorldSection worldSection = JsonUtility.FromJson<WorldSection>(worldSectionResult);
                GameManager.Instance.currentWorld = new World() { id = worldSection.world_id };

                if (playerBase != null)
                {
                    await SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
                    gameObject.SetActive(false);
                }
            }
            catch (System.Exception e)
            {
                Console.Instance.Print(e.Message, "red");
            }
        }
        else
        {
            worldSelectionWidow.Init(() =>
            {
                authenticateWindow.SetActive(false);
                worldSelectionWidow.Show(async (newBase) =>
                {
                    if (newBase != null)
                    {
                        GameManager.Instance.playerBase = newBase;
                        await SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
                        gameObject.SetActive(false);
                    }
                });
            });
        }
    }


}
