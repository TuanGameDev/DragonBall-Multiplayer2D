using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingBar : MonoBehaviour
{
    [Header("LoadingBar")]
    public GameObject loadingbarPopup;
    public TextMeshProUGUI loadingText;
    public Slider loadingSlider;
    public float loadSpeed = 0.5f;
    private float targetProgress = 0f;
    void Update()
    {
        if (loadingSlider.value < targetProgress)
        {
            loadingSlider.value += loadSpeed * Time.deltaTime;
        }
        else
        {
            loadingSlider.value = targetProgress;

            if (targetProgress >= 1f)
            {
                loadingbarPopup.SetActive(false);
                NetworkManager.networkmanager.JoinSever();
            }
        }

        loadingText.text = " Loading:... " + (loadingSlider.value * 100f).ToString("F0") + "%";
    }

    public void SetProgress(float progress)
    {
        targetProgress = Mathf.Clamp01(progress);
    }
}
