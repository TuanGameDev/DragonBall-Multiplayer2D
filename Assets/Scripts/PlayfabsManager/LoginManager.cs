using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    [Header("Login/Register")]
    [SerializeField] TMP_InputField loginUsernameInput;
    [SerializeField] TMP_InputField loginPasswordInput;
    [SerializeField] Toggle rememberMeToggle;
    [SerializeField] TMP_InputField registerUsernameInput;
    [SerializeField] TMP_InputField registerPasswordInput;
    [SerializeField] private TextMeshProUGUI messageText;
    public static LoginManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        if (PlayerPrefs.HasKey("RememberMe") && PlayerPrefs.GetInt("RememberMe") == 1)
        {
            string savedUsername = PlayerPrefs.GetString("Username");
            string savedPassword = PlayerPrefs.GetString("Password");
            loginUsernameInput.text = savedUsername;
            loginPasswordInput.text = savedPassword;
            rememberMeToggle.isOn = true;
        }
    }
    public void OnLoginButtonClicked()
    {
        string username = loginUsernameInput.text;
        string password = loginPasswordInput.text;
        var request = new LoginWithPlayFabRequest
        {
            Username = username,
            Password = password
        };

        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnLoginFailure);
    }
    private void OnLoginSuccess(LoginResult result)
    {
        if (rememberMeToggle.isOn)
        {
            PlayerPrefs.SetInt("RememberMe", 1);
            PlayerPrefs.SetString("Username", loginUsernameInput.text);
            PlayerPrefs.SetString("Password", loginPasswordInput.text);
        }
        else
        {
            PlayerPrefs.SetInt("RememberMe", 0);
            PlayerPrefs.DeleteKey("Username");
            PlayerPrefs.DeleteKey("Password");
        }
    }
    private void OnLoginFailure(PlayFabError error)
    {
        messageText.text = "Login failed: " + error.ErrorMessage;
        messageText.color = Color.red;
        StartCoroutine(HideMessageAfterDelay(2f));
    }
    public void OnLogoutButtonClicked()
    {
        PlayerPrefs.DeleteKey("RememberMe");
        PlayerPrefs.DeleteKey("Username");
        PlayerPrefs.DeleteKey("Password");
    }
    public void OnRegisterButtonClicked()
    {
        string username = registerUsernameInput.text;
        string password = registerPasswordInput.text;

        if (username.Length < 5 || username.Length > 8)
        {
            messageText.text = "Username must be from 5 to 8 characters!";
            messageText.color = Color.red;
            StartCoroutine(HideMessageAfterDelay(2f));
            return;
        }
        if (password.Length < 6 || password.Length > 8)
        {
            messageText.text = "Password must be 6 to 8 characters!";
            messageText.color = Color.red;
            StartCoroutine(HideMessageAfterDelay(2f));
            return;
        }

        var request = new RegisterPlayFabUserRequest
        {
            Username = username,
            Password = password,
            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        messageText.text = "Sign Up Success!";
        messageText.color = Color.green;
        StartCoroutine(HideMessageAfterDelay(2f));
    }

    private void OnRegisterFailure(PlayFabError error)
    {
        messageText.text = "Registration failed: " + error.ErrorMessage;
        messageText.color = Color.red;
        StartCoroutine(HideMessageAfterDelay(2f));
    }
    private IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        messageText.text = string.Empty;
    }
    public void DeleteSave()
    {
        PlayerPrefs.DeleteKey("RememberMe");
    }
}
