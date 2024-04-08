using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomButton : MonoBehaviour
{
    public TextMeshProUGUI roomNameText;
    public TextMeshProUGUI playerCountText;

    private string roomName;

    // Khởi tạo button với thông tin phòng
    public void Initialize(string name, int playerCount)
    {
        roomName = name;
        roomNameText.text = name;
        playerCountText.text = playerCount.ToString();
    }

    // Xử lý sự kiện khi người chơi nhấp vào button
    public void OnJoinRoomButtonClicked()
    {
        PhotonNetwork.JoinRoom(roomName);
    }
}