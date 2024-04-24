using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Xml.Linq;

public class PlayerInfomation : MonoBehaviourPun
{
    public TextMeshProUGUI playernameText;
    public TextMeshProUGUI playerlevelText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI mpText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI defText;
    public PlayerController _playercontroller;
    private void Update()
    {
        hpText.text = " HP: " +  _playercontroller.currentHP + "/" + _playercontroller.maxHP;
        mpText.text = " MP: " +  _playercontroller.currentMP + "/" + _playercontroller.maxMP;
        attackText.text = " Damage: " + _playercontroller.damageMin + "-" + _playercontroller.damageMax;
        defText.text = " Def: " + _playercontroller.def;
    }
    public void UpdateNametag(string name)
    {
        playernameText.text = " Player Name: " + name;
    }
    public void UpdateLevel(int currentExp, int maxExp, int level)
    {

        float percentage = (float)currentExp / maxExp * 100f;
        string formattedPercentage = " Level: " + level + "+" + percentage.ToString("0.00") + "%";
        playerlevelText.text = formattedPercentage;
    }
}
