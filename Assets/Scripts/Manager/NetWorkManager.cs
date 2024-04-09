﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("NetworkManager")]
    private string playerName;
    public Button startgameButton;
    public TextMeshProUGUI playerNameText;
    public GameObject nameInput;
    void Start()
    {
        startgameButton.interactable = false;
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
    void ChangeScene()
    {
        PhotonNetwork.LoadLevel("Map");
    }
}