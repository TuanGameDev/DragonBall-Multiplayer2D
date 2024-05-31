using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUpGrade : MonoBehaviour
{
    [Header("Tiềm Năng Sức Mạnh")]
    public int strengthPotential;
    [Header("InfoText")]
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI mpText;
    public TextMeshProUGUI defText;
    [Header("Popup")]
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI potentialText;
    public GameObject inventoryPopup;
    public GameObject upgradePopup;
    public PlayerController _playercontroller;
    private void Start()
    {
        if (PlayerPrefs.HasKey("Potential"))
        {
            strengthPotential = PlayerPrefs.GetInt("Potential");
        }
    }
    private void Update()
    {
        UpdateTextPotential(strengthPotential);
        UpdateTextAttack(_playercontroller.damageMax);
        UpdateTextHp(_playercontroller.maxHP);
        UpdateTextMp(_playercontroller.maxMP);
        UpdateTextDf(_playercontroller.def);
    }
    #region Upgrade
    public void UpGradeAttack()
    {
        int point = 1;

        if (strengthPotential >= point)
        {
            PlayerController.me.damageMin += 10;
            PlayerController.me.damageMax += 10;
            PlayerPrefs.SetInt("DamageMin", PlayerController.me.damageMin);
            PlayerPrefs.SetInt("DamageMax", PlayerController.me.damageMax);
            strengthPotential -= point;
            PlayerPrefs.SetInt("Potential", strengthPotential);
            messageText.text = "You have successfully upgraded Attack!";
            messageText.color = Color.green;
            StartCoroutine(HideMessageAfterDelay(2f));
        }
        else
        {
            messageText.text = "You don't have enough coins to upgrade Attack!";
            messageText.color = Color.red;
            StartCoroutine(HideMessageAfterDelay(2f));
        }
    }
    public void UpGradeHp()
    {
        int point = 1;

        if (strengthPotential >= point)
        {
            PlayerController.me.currentHP += 20;
            PlayerController.me.maxHP += 20;
            PlayerPrefs.SetInt("currentHP", PlayerController.me.currentHP);
            PlayerPrefs.SetInt("maxHP", PlayerController.me.maxHP);
            strengthPotential -= point;
            PlayerPrefs.SetInt("Potential", strengthPotential);
            messageText.text = "You have successfully upgraded Health!";
            messageText.color = Color.green;
            StartCoroutine(HideMessageAfterDelay(2f));
        }
        else
        {
            messageText.text = "You don't have enough coins to upgrade Health!";
            messageText.color = Color.red;
            StartCoroutine(HideMessageAfterDelay(2f));
        }
    }
    public void UpGradeMp()
    {
        int point = 1;

        if (strengthPotential >= point)
        {
            PlayerController.me.currentMP += 20;
            PlayerController.me.maxMP += 20;
            PlayerPrefs.SetInt("currentMP", PlayerController.me.currentMP);
            PlayerPrefs.SetInt("maxMP", PlayerController.me.maxMP);
            strengthPotential -= point;
            PlayerPrefs.SetInt("Potential", strengthPotential);
            messageText.text = "You have successfully upgraded Mana!";
            messageText.color = Color.green;
            StartCoroutine(HideMessageAfterDelay(2f));
        }
        else
        {
            messageText.text = "You don't have enough coins to upgrade Mana!";
            messageText.color = Color.red;
            StartCoroutine(HideMessageAfterDelay(2f));
        }
    }
    public void UpGradeDef()
    {
        int point = 5;

        if (strengthPotential >= point)
        {
            PlayerController.me.def += 5;
            PlayerPrefs.SetInt("DF", PlayerController.me.def);
            strengthPotential -= point;
            PlayerPrefs.SetInt("Potential", strengthPotential);
            messageText.text = "You have successfully upgraded DEF!";
            messageText.color = Color.green;
            StartCoroutine(HideMessageAfterDelay(2f));
        }
        else
        {
            messageText.text = "You don't have enough coins to upgrade DEF!";
            messageText.color = Color.red;
            StartCoroutine(HideMessageAfterDelay(2f));
        }
    }
    #endregion

    #region ShowUI
    public void UpdateTextPotential(int potential)
    {
        potentialText.text = "Điểm tiềm năng: " + potential;
    }
    public void UpdateTextAttack(int attack)
    {
        attackText.text = "Tấn Công: " + attack;
    }
    public void UpdateTextHp(int health)
    {
        hpText.text = "HP: " + health;
    }
    public void UpdateTextMp(int mana)
    {
        mpText.text = "KI: " + mana;
    }
    public void UpdateTextDf(int df)
    {
        defText.text = "Giáp: " + df;
    }
    public void ShowHUD()
    {
        inventoryPopup.SetActive(true);
    }
    public void ShowUpgrade()
    {
        upgradePopup.SetActive(true);
        inventoryPopup.SetActive(false);
    }
    private IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        messageText.text = string.Empty;
    }
    #endregion
}
