using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEditor;
using TMPro;
using UnityEngine.UI;

public class Enemy : MonoBehaviourPun
{
    public SpriteRenderer sr;
    public Rigidbody2D rb;
    [Header("EnemyManager")]
    public string enemyName;
    public int currentHP;
    public int maxHP;
    private float maxHealthValue;
    public int curAttackerID;
    public int xpToGive;
    [Header("UI")]
    public TextMeshProUGUI playernametagText;
    public TextMeshProUGUI hpText;
    public Slider healthBar;
    private void Start()
    {
        UpdateValue(maxHP);
        photonView.RPC("UpdateNametag", RpcTarget.All, enemyName);
        photonView.RPC("UpdateHpText", RpcTarget.All, currentHP);
        photonView.RPC("UpdateHealthBar", RpcTarget.All, currentHP);
    }
    [PunRPC]
    public void TakeDamage(int attackerId, int damageAmount)
    {
        currentHP -= damageAmount;
        curAttackerID = attackerId;
        photonView.RPC("UpdateHpText", RpcTarget.All, currentHP);
        photonView.RPC("UpdateHealthBar", RpcTarget.All, currentHP);
        if (currentHP <= 0)
        {
            Die();
        }
    }
    void Die()
    {
      
    }
    public void UpdateValue(int maxVal)
    {
        maxHealthValue = maxVal;
        healthBar.value = 1.0f;
    }
    [PunRPC]
    void UpdateNametag(string nametag)
    {
        playernametagText.text = "" + nametag;
    }
    [PunRPC]
    void UpdateHpText(int curHP)
    {
        hpText.text = curHP.ToString();
    }
    [PunRPC]
    void UpdateHealthBar(int value)
    {
        healthBar.value = (float)value / maxHealthValue;
    }
}