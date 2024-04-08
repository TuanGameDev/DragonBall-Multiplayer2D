using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HanhTinhSelection : MonoBehaviour
{
    public GameObject traidatPopup;
    public GameObject xaydaPopup;
    public GameObject namecPopup;
    public void ShowTraiDat()
    {
        traidatPopup.SetActive(true);
        xaydaPopup.SetActive(false);
        namecPopup.SetActive(false);
    } public void ShowXayda()
    {
        traidatPopup.SetActive(false);
        xaydaPopup.SetActive(true);
        namecPopup.SetActive(false);
    } public void ShowNamec()
    {
        traidatPopup.SetActive(false);
        xaydaPopup.SetActive(false);
        namecPopup.SetActive(true);
    }
}
