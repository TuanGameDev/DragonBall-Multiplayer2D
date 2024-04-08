using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.EventSystems;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    private string playerName;
    public TextMeshProUGUI playerNameText;
    public GameObject nameInput;
    void Start()
    {
        if (PlayerPrefs.HasKey("Name"))
        {
            playerName = PlayerPrefs.GetString("Name");
            PhotonNetwork.NickName = playerName;
            nameInput.SetActive(false);
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
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room");
        ChangeScene();
    }
   /* public override void OnRoomListUpdate(List<RoomInfo> updatedRoomList)
    {
        Debug.Log("Room list updated");
        roomList = updatedRoomList;
        UpdateRoomListUI();
    }*/
    public void JoinSever()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;

        PhotonNetwork.JoinOrCreateRoom("Sever1", roomOptions, TypedLobby.Default);
    }
    void ChangeScene()
    {
        PhotonNetwork.LoadLevel("Map");
    }
    /*void UpdateRoomListUI()
    {
        foreach (GameObject button in roomButtons)
        {
            Destroy(button);
        }
        roomButtons.Clear();
        foreach (RoomInfo room in roomList)
        {
            GameObject button = Instantiate(roomButtonPrefab, roomListContainer);
            RoomButton joinRoomButton = button.GetComponent<RoomButton>();
            joinRoomButton.Initialize(room.Name, room.PlayerCount);
            roomButtons.Add(button);
        }
    }*/
}