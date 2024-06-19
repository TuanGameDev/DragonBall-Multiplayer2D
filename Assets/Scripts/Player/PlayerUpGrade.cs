using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUpGrade : MonoBehaviour
{
    [Header("Tiềm Năng Sức Mạnh")]
    public int strengthPotential;
    [Header("InfoText")]
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI mpText;
    public TextMeshProUGUI defText;
    public TextMeshProUGUI critText;
    [Header("Popup")]
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI potentialText;
    public GameObject inventortHUD;
    public GameObject inventoryPopup;
    public GameObject upgradePopup;
    public GameObject missionPopup;
    [Header("Button Nâng cấp")]
    public Button attackButton;
    public Button hpButton;
    public Button kiButton;
    public Button defButton;
    public Button crtButton;
    public PlayerController controller;
    private void Start()
    {
        attackButton.onClick.AddListener(UpGradeAttack);
        hpButton.onClick.AddListener(UpGradeHp);
        kiButton.onClick.AddListener(UpGradeMp);
        defButton.onClick.AddListener(UpGradeDef);
        crtButton.onClick.AddListener(UpGradeCrt);
        if (PlayerPrefs.HasKey("Potential"))
        {
            strengthPotential = PlayerPrefs.GetInt("Potential");
        }
    }
    private void Update()
    {
        if (controller.damageMax >= 5000)
        {
            attackButton.interactable = false;
        }
        if (controller.currentHP >= 1000000)
        {
            hpButton.interactable = false;
        }
        if (controller.currentMP >= 1000000)
        {
            kiButton.interactable = false;
        }
        if (controller.def >= 100)
        {
            defButton.interactable = false;
        }
        if (controller.criticalDamage >= 5)
        {
            crtButton.interactable = false;
        }
        UpdateTextPotential(strengthPotential);
        UpdateTextAttack(controller.damageMax);
        UpdateTextHp(controller.maxHP);
        UpdateTextMp(controller.maxMP);
        UpdateTextDf(controller.def);
        UpdateTextCRT(controller.criticalDamage);
    }
    #region Upgrade
    public void UpGradeAttack()
    {
        int point = 1;

        if (strengthPotential >= point)
        {
            PlayerController.me.damageMin += 20;
            PlayerController.me.damageMax += 20;
            PlayerPrefs.SetInt("DamageMin", PlayerController.me.damageMin);
            PlayerPrefs.SetInt("DamageMax", PlayerController.me.damageMax);
            strengthPotential -= point;
            PlayerPrefs.SetInt("Potential", strengthPotential);
            messageText.text = "Bạn đã nâng câp tấn công thành công";
            messageText.color = Color.green;
            StartCoroutine(HideMessageAfterDelay(2f));
        }
        else
        {
            messageText.text = "Bạn không đủ điểm tiềm năng";
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
            messageText.text = "Bạn đã nâng câp hp thành công";
            messageText.color = Color.green;
            StartCoroutine(HideMessageAfterDelay(2f));
        }
        else
        {
            messageText.text = "Bạn không đủ điểm tiềm năng";
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
            messageText.text = "Bạn đã nâng câp ki thành công";
            messageText.color = Color.green;
            StartCoroutine(HideMessageAfterDelay(2f));
        }
        else
        {
            messageText.text = "Bạn không đủ điểm tiềm năng";
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
            messageText.text = "Bạn đã nâng câp giáp thành công";
            messageText.color = Color.green;
            StartCoroutine(HideMessageAfterDelay(2f));
        }
        else
        {
            messageText.text = "Bạn không đủ điểm tiềm năng";
            messageText.color = Color.red;
            StartCoroutine(HideMessageAfterDelay(2f));
        }
    }
    public void UpGradeCrt()
    {
        int point = 5;

        if (strengthPotential >= point)
        {
            PlayerController.me.criticalDamage += 1;
            PlayerPrefs.SetInt("CritDamage", PlayerController.me.criticalDamage);
            strengthPotential -= point;
            PlayerPrefs.SetInt("Potential", strengthPotential);
            messageText.text = "Bạn đã nâng câp chí mạng thành công";
            messageText.color = Color.green;
            StartCoroutine(HideMessageAfterDelay(2f));
        }
        else
        {
            messageText.text = "Bạn không đủ điểm tiềm năng";
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
        string attackString = "Tấn công: " + attack;
        string potentialString = "\n<size=20>Bạn cần 1 điểm tiềm năng để nâng cấp và cộng 20 tấn công</size>";
        attackText.text = attackString + potentialString;
    }
    public void UpdateTextHp(int health)
    {
        string hpString = "HP: " + health;
        string potentialString = "\n<size=20>Bạn cần 1 điểm tiềm năng để nâng cấp cộng 20 HP</size>";
        hpText.text = hpString + potentialString;
    }
    public void UpdateTextMp(int ki)
    {
        string mpString = "KI: " + ki;
        string potentialString = "\n<size=20>Bạn cần 1 điểm tiềm năng để nâng cấp cộng 20 KI</size>";
        mpText.text = mpString + potentialString;
    }
    public void UpdateTextDf(int df)
    {
        string dfString = "Giáp: " + df;
        string potentialString = "\n<size=20>Bạn cần 5 điểm tiềm năng để nâng cấp và cộng 5 giáp</size>";
        defText.text = dfString + potentialString;
    }
    public void UpdateTextCRT(int crt)
    {
        string crtString = "Chí mạng: " + crt;
        string potentialString = "\n<size=20>Bạn cần 5 điểm tiềm năng để nâng cấp và cộng 1 chí mạng</size>";
        critText.text = crtString + potentialString;
    }
    public void ShowUI()
    {
        inventortHUD.SetActive(true);
    }
    public void ShowInventory()
    {
        inventoryPopup.SetActive(true);
        upgradePopup.SetActive(false);
        missionPopup.SetActive(false);
    }
    public void ShowUpgrade()
    {
        upgradePopup.SetActive(true);
        inventoryPopup.SetActive(false);
        missionPopup.SetActive(false);
    }
    public void ShowMission()
    {
        missionPopup.SetActive(true);
        upgradePopup.SetActive(false);
        inventoryPopup.SetActive(false);
    }
    private IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        messageText.text = string.Empty;
    }
    #endregion
}
