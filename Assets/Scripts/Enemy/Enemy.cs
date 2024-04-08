using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEditor;
using TMPro;
using UnityEngine.UI;

public class Enemy : MonoBehaviourPun
{
    [Header("Enemy Status")]
    public int currentHP;
    public int maxHP;
    public int xpToGive;
    public SpriteRenderer sr;
    public Rigidbody2D rb;
    [Header("Text UI")]
    private float maxHealthValue;
    public int curAttackerID;
    private void Start()
    {
        EnemyStatusInfo(maxHP);
    }
    [PunRPC]
    public void TakeDamage(int attackerId, int damageAmount)
    {
        currentHP -= damageAmount;
        curAttackerID = attackerId;
        if (currentHP <= 0)
        {
            Die();
        }
    }
    void Die()
    {
      
    }
    public void EnemyStatusInfo(int maxVal)
    {
        maxHealthValue = maxVal;
    }
}