using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainView : MonoBehaviour
{
    [Header("Màn hình hiển thị")]
    public GameObject playerseletionPopup;
    public GameObject severPopup;
    public GameObject createseverPopup;
    public void ShowSeverPopup()
    {
        severPopup.SetActive(true);
        playerseletionPopup.SetActive(false);
    }
    public void ShowPlayerSelectionPopup()
    {
        severPopup.SetActive(false);
        createseverPopup.SetActive(false);
        playerseletionPopup.SetActive(true);
    } 
    public void ShowCreateSeverPopup()
    {
        createseverPopup.SetActive(true);
        severPopup.SetActive(false);
        playerseletionPopup.SetActive(false);
    }
}
