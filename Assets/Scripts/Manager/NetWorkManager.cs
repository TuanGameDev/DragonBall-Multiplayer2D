using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Collections;
public class NetworkManager : MonoBehaviourPunCallbacks
{
    [System.Serializable]
    public class MapData
    {
        public string name;
        public int scene;
    }

    [Header("NetworkManager")]
    private string playerName;
    public TMP_InputField roomNameInput;
    public Button startgameButton;
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI mapValue;
    public GameObject nameInput;

    [Header("Popup")]
    public GameObject tabMain;
    public GameObject tabRooms;
    public GameObject tabCreate;
    public GameObject buttonRoom;


    public GameObject networkManagerPopup;
    public GameObject loadingbarPopup;
    public GameObject hanhtinhPopup;

    [Header("Map")]
    public MapData[] maps;
    private int currentmap = 0;
    private List<RoomInfo> roomList;
    public static NetworkManager networkmanager;
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        Connect();
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
    }
    public void Connect()
    {
        Debug.Log("Trying to Connect...");
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
        StartGame();

        base.OnJoinedRoom();
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Create();

        base.OnJoinRandomFailed(returnCode, message);
    }
    public void Create()
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 4;

        options.CustomRoomPropertiesForLobby = new string[] { "map" };

        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
        properties.Add("map", currentmap);
        options.CustomRoomProperties = properties;

        PhotonNetwork.CreateRoom(roomNameInput.text, options);
    }
    public void ChangeMap(int map)
    {
        if (map >= 0 && map < maps.Length)
        {
            currentmap = map;
        }
        mapValue.text = "Chọn Hành Tinh: " + maps[currentmap].name;
    }
    public override void OnRoomListUpdate(List<RoomInfo> p_list)
    {
        roomList = p_list;
        ClearRoomList();

        Transform content = tabRooms.transform.Find("Panel/Tranform");

        foreach (RoomInfo a in roomList)
        {
            GameObject newRoomButton = Instantiate(buttonRoom, content) as GameObject;

            newRoomButton.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = a.Name;
            newRoomButton.transform.Find("Players").GetComponent<TextMeshProUGUI>().text = a.PlayerCount + " / " + a.MaxPlayers;

            if (a.CustomProperties.ContainsKey("map"))
                newRoomButton.transform.Find("Map/Name").GetComponent<TextMeshProUGUI>().text = maps[(int)a.CustomProperties["map"]].name;
            else
                newRoomButton.transform.Find("Map/Name").GetComponent<TextMeshProUGUI>().text = "-----";

            newRoomButton.GetComponent<Button>().onClick.AddListener(delegate { JoinRoom(newRoomButton.transform); });
        }

        base.OnRoomListUpdate(roomList);
    }
    private void ClearRoomList()
    {
        Transform content = tabRooms.transform.Find("Panel/Tranform");
        foreach (Transform a in content) Destroy(a.gameObject);
    }
    public void JoinRoom(Transform p_button)
    {
        string t_roomName = p_button.Find("Name").GetComponent<TextMeshProUGUI>().text;

        RoomInfo roomInfo = null;
        Transform buttonParent = p_button.parent;
        for (int i = 0; i < buttonParent.childCount; i++)
        {
            if (buttonParent.GetChild(i).Equals(p_button))
            {
                roomInfo = roomList[i];
                break;
            }
        }

        if (roomInfo != null)
        {
            PhotonNetwork.JoinRoom(t_roomName);
        }
    }
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }
    public void StartGame()
    {
        PhotonNetwork.LoadLevel(maps[currentmap].scene);
    }
    #region LoginManager
    #endregion
}