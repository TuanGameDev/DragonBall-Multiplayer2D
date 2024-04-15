using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class RoomSwitcher : MonoBehaviour
{
    public string newRoomName;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PhotonNetwork.LoadLevel(newRoomName);
            Destroy(collision.gameObject);
            PhotonNetwork.LeaveRoom();
        }
    }
}