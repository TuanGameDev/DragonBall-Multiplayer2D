using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TimerDestroy : MonoBehaviourPun
{
    public float timer;
    private void Start()
    {
        photonView.RPC("DestroyObject",RpcTarget.All);
    }
    [PunRPC]
    void DestroyObject()
    {
        Destroy(gameObject,timer);
    }
}
