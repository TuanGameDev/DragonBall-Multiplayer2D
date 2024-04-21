using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("NetworkManager")]
    public string nameMap;
    private string playerName;
    public Button startgameButton;
    public TextMeshProUGUI playerNameText;
    public GameObject nameInput;
    [Header("Popup")]
    public GameObject networkManagerPopup;
    public GameObject loadingbarPopup;
    public GameObject hanhtinhPopup;
    public static NetworkManager networkmanager;
    private void Awake()
    {
        if (networkmanager != null && networkmanager != this)
            gameObject.SetActive(false);
        else
        {
            networkmanager = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        startgameButton.interactable = false;
        if (PlayerPrefs.HasKey("Name"))
        {
            playerName = PlayerPrefs.GetString("Name");
            PhotonNetwork.NickName = playerName;
            nameInput.SetActive(false);
            hanhtinhPopup.SetActive(false);
            playerNameText.text = playerName;
        }
        else
        {
            nameInput.SetActive(true);
        }
        PhotonNetwork.ConnectUsingSettings();
    }

    public void OnPlayerNameChanged(TMP_InputField playerNameInput)
    {
        playerName = playerNameInput.text;
        int maxNameLength = 5;
        if (playerName.Length > maxNameLength)
        {
            playerName = playerName.Substring(0, maxNameLength);
            playerNameInput.text = playerName;
        }
        PlayerPrefs.SetString("Name", playerName);
        PhotonNetwork.NickName = playerName;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master server");
        PhotonNetwork.JoinLobby();
        startgameButton.interactable = true;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room");
        ChangeScene();
    }

    public void JoinSever()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.JoinOrCreateRoom("Sever1", roomOptions, TypedLobby.Default);
    }

    public  void ChangeScene()
    {
        PhotonNetwork.LoadLevel(nameMap);
    }

    public void StartGame()
    {
        networkManagerPopup.SetActive(false);
        loadingbarPopup.SetActive(true);
    }

    #region LoginManager
    #endregion
}