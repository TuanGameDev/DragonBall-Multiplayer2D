using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera virtualCamera;

    void Start()
    {
        virtualCamera = GameObject.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>();
        if (virtualCamera == null)
        {
            Debug.LogError("Virtual camera reference is missing!");
        }
    }

    void Update()
    {
        if (PlayerController.me != null && !PlayerController.me.dead)
        {
            Vector3 targetPosition = PlayerController.me.transform.position;
            targetPosition.z = -10;
            transform.position = targetPosition;
            if (virtualCamera != null)
            {
                virtualCamera.Follow = PlayerController.me.transform;
            }
        }
    }
}