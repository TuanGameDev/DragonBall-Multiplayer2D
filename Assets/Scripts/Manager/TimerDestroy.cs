using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TimerDestroy : MonoBehaviour
{
    public float destroyTime;
    void Start()
    {
        Invoke("DestroyObject", destroyTime);
    }
    void DestroyObject()
    {
        Destroy(gameObject);
    }
}
