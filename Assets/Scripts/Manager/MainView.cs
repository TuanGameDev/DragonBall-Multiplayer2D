using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainView : MonoBehaviour
{
    [Header("Màn hình hiển thị")]
    public GameObject severPopup;
    public GameObject createseverPopup;
    public GameObject listseverPopup;
    public GameObject loginPopup;
    public GameObject selectionPopup;
    public void ShowSeverPopup()
    {
        severPopup.SetActive(true);
    }
    public void ShowListSeverPopup()
    {
        listseverPopup.SetActive(true);
        createseverPopup.SetActive(false);
    } 
    public void ShowCreateSeverPopup()
    {
        createseverPopup.SetActive(true);
        listseverPopup.SetActive(false);
    }
    public void ShowLoginPopup()
    {
        loginPopup.SetActive(true);
        selectionPopup.SetActive(false);
    }
}
