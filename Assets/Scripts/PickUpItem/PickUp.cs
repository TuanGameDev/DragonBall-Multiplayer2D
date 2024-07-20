using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickupTypes
{
    Coin,
    Diamond,
}
namespace HoangTuan.Scripts.Scriptable_Objects.Character
{
    public class PickUp : MonoBehaviourPun
    {
        public PickupTypes types;
        public int value;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (photonView.IsMine)
            {
                if (collision.CompareTag("Player"))
                {
                    HeroController player = collision.gameObject.GetComponent<HeroController>();
                    if (player != null)
                    {
                        if (types == PickupTypes.Coin)
                        {
                            player.photonView.RPC("GetCoin", RpcTarget.All, value);
                        }
                        else if (types == PickupTypes.Diamond)
                        {
                            player.photonView.RPC("Diamond", RpcTarget.All, value);
                        }
                        photonView.RPC("DestroyPickup", RpcTarget.All);
                    }
                }
            }
        }

        [PunRPC]
        private void DestroyPickup()
        {
            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}