using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum PickupTypes
{
    Coin,
    Diamond,
}
public class PickUp : MonoBehaviourPun
{
    public PickupTypes types;
    public int value;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (types == PickupTypes.Coin)
                player.photonView.RPC("GetCoin", player.photonPlayer, value);
            else if (types == PickupTypes.Diamond)
                player.photonView.RPC("Diamond", player.photonPlayer, value);
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
