using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
namespace HoangTuan.Scripts.Scriptable_Objects.Character
{
    public class CameraFollow : MonoBehaviour
    {
        public static CameraFollow instance { get; private set; }
        [SerializeField]
        private CinemachineVirtualCamera virtualCamera;
        private float shakeTimer;
        private float shakeTimerTotal;
        private float startingIntensity;

        void Awake()
        {
            instance = this;
            virtualCamera = GetComponent<CinemachineVirtualCamera>();

        }

        void Update()
        {
            if (HeroController.me != null && !HeroController.me.dead)
            {
                Vector3 targetPosition = HeroController.me.transform.position;
                targetPosition.z = -10;
                transform.position = targetPosition;
                if (virtualCamera != null)
                {
                    virtualCamera.Follow = HeroController.me.transform;
                }
            }
            if (shakeTimer > 0)
            {
                shakeTimer -= Time.deltaTime;
                CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(startingIntensity, 0f, shakeTimer / shakeTimerTotal);
            }
            else
            {
                CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
            }
        }
        public void ShakeCamera(float intensity, float time)
        {
            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChanlPerlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            cinemachineBasicMultiChanlPerlin.m_AmplitudeGain = intensity;
            shakeTimer = time;
            shakeTimerTotal = time;
            startingIntensity = intensity;
        }
    }
}