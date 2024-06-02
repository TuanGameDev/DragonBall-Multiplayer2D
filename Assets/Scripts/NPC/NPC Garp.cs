using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviourPun
{
    [Header("Nói chuyện")]
    public GameObject callShow;
    [Header("Shop")]
    public GameObject shopPopup;
    [Header("BXH")]
    public GameObject bxhPopup;
    [Header("Vàng/Ngọc")]
    public TextMeshProUGUI _coinshopText;
    public TextMeshProUGUI _diamondshopText;
    private void Update()
    {
        Shop();
    }
    public void BuyAttack()
    {
        int coin = 100;

        if (PlayerController.me.coin >= coin)
        {
            PlayerController.me.damageMin += 5;
            PlayerController.me.damageMax += 5;
            PlayerPrefs.SetInt("DamageMin", PlayerController.me.damageMin);
            PlayerPrefs.SetInt("DamageMax", PlayerController.me.damageMax);
            PlayerController.me.coin -= coin;
            PlayerPrefs.SetInt("Coin", PlayerController.me.coin);
        }
    }
    public void BuyHealth()
    {
        int coin = 200;

        if (PlayerController.me.coin >= coin)
        {
            PlayerController.me.currentHP += 10;
            PlayerController.me.maxHP += 10;
            PlayerPrefs.SetInt("currentHP", PlayerController.me.currentHP);
            PlayerPrefs.SetInt("maxHP", PlayerController.me.maxHP);
            PlayerController.me.coin -= coin;
            PlayerPrefs.SetInt("Coin", PlayerController.me.coin);
            PlayerController.me.UpdateValue(PlayerController.me.maxHP);
        }
    }
    public void BuyMana()
    {
        int coin = 200;
        if (PlayerController.me.coin >= coin)
        {
            PlayerController.me.maxMP += 50;
            PlayerPrefs.SetInt("maxMP", PlayerController.me.maxMP);
            PlayerController.me.coin -= coin;
            PlayerPrefs.SetInt("Coin", PlayerController.me.coin);
        }
    }
    public void BuyDF()
    {
        int coin = 300;
        if (PlayerController.me.coin >= coin)
        {
            PlayerController.me.def += 2;
            PlayerPrefs.SetInt("DF", PlayerController.me.def);
            PlayerController.me.coin -= coin;
            PlayerPrefs.SetInt("Coin", PlayerController.me.coin);
        }
    }
    void Shop()
    {
        if (PlayerController.me != null)
        {
            _coinshopText.text = "" + PlayerController.me.coin.ToString("N0");
            _diamondshopText.text = "" + PlayerController.me.diamond.ToString("N0");

        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PhotonView photonView = collision.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                callShow.SetActive(true);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PhotonView photonView = collision.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                callShow.SetActive(false);
            }
        }
    }
    #region Hiển thị Shop + BXH + Code
    public void ShowShop()
    {
        shopPopup.SetActive(true);
        callShow.SetActive(false);
    } 
    public void ShowBXH()
    {
        bxhPopup.SetActive(true);
        callShow.SetActive(false);
    }
    #endregion
}
