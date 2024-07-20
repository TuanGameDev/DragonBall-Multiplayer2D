using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HeroUI : MonoBehaviour
{
    public TextMeshProUGUI nameTxt;
    public TextMeshProUGUI levelTxt;
    public void UpdateInfo(string name,int level)
    {
        nameTxt.text = name;
        levelTxt.text = "Level: "+ level;
    }
}
