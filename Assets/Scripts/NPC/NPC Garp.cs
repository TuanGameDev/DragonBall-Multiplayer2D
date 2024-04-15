using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPCGarp : MonoBehaviourPun
{
    [Header("Call")]
    public GameObject callShow;
    [Header("Upgrade")]
    public GameObject upgradeUI;
    [Header("Shop")]
    public GameObject shopUI;
    [Header("Code")]
    public TMP_InputField inputcode;
    public GameObject codeUI;
    [Header("UI-Upgrade")]
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI mpText;
    public TextMeshProUGUI defText;

    public TextMeshProUGUI _coinupgradeText;
    public TextMeshProUGUI _diamondupgradeText;
    [Header("UI-Shop")]
    public TextMeshProUGUI _coinshopText;
    public TextMeshProUGUI _diamondshopText;



    private const string correctCode1 = "TAMDEN";
    private const string correctCode2 = "PHUONG";
    private const string correctCode3 = "ADMIN";
    private const string correctCodeVip = "HUYCOLD";

    private const string codeUsedKey1 = "CodeUsed-1";
    private const string codeUsedKey2 = "CodeUsed-2";
    private const string codeUsedKey3 = "CodeUsed-3";
    private const string codeUsedKeyVip = "CodeUsed-Vip";

    [Header("MessesText")]
    public TextMeshProUGUI messageText;
    private bool isUpgradeUIVisible = false;
    private void Update()
    {
        Upgrade();
        Shop();
        if (isUpgradeUIVisible)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                BuyAttack();
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                BuyHealth();
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                BuyMana();
            }
            else if (Input.GetKeyDown(KeyCode.V))
            {
                BuyDF();
            }
        }
    }
    public void ReceiveCoin()
    {
        int coin = 1000;
        string maCode = inputcode.text;
        bool codeUsed = false;

        if (maCode == correctCode1 && PlayerPrefs.GetInt(codeUsedKey1) != 1)
        {
            PlayerPrefs.SetInt(codeUsedKey1, 1);
            codeUsed = true;
        }
        else if (maCode == correctCode2 && PlayerPrefs.GetInt(codeUsedKey2) != 2)
        {
            PlayerPrefs.SetInt(codeUsedKey2, 2);
            codeUsed = true;
        }
        else if (maCode == correctCode3 && PlayerPrefs.GetInt(codeUsedKey3) != 3)
        {
            PlayerPrefs.SetInt(codeUsedKey3, 3);
            codeUsed = true;
        }

        if (codeUsed)
        {
            if (PlayerController.me != null)
            {
                PlayerController.me.coin += coin;
                PlayerPrefs.SetInt("Coin", PlayerController.me.coin);
                messageText.text = "You have received " + coin.ToString("N0") + " coin.";
                messageText.color = Color.green;
                StartCoroutine(HideMessageAfterDelay(3f));
            }
        }
        else
        {
            messageText.text = "The code has been used or is incorrect.";
            messageText.color = Color.red;
            StartCoroutine(HideMessageAfterDelay(3f));
        }
    }
    public void ReceiveCoinVip()
    {
        int coin = 2000;
        string maCode = inputcode.text;

        if (maCode == correctCodeVip)
        {
            bool codeUsed = PlayerPrefs.GetInt(codeUsedKeyVip) == 4;

            if (codeUsed)
            {
                messageText.text = "The code has been used.";
                messageText.color = Color.red;
                StartCoroutine(HideMessageAfterDelay(3f));
            }
            else
            {
                if (PlayerController.me != null)
                {
                    PlayerController.me.coin += coin;
                    PlayerPrefs.SetInt("Coin", PlayerController.me.coin);
                    PlayerPrefs.SetInt(codeUsedKeyVip, 4);
                    messageText.text = " You have received " + coin.ToString("N0") + " coin.";
                    messageText.color = Color.green;
                    StartCoroutine(HideMessageAfterDelay(3f));
                }
            }
        }
    }
    public void ShowUpGrade()
    {
        callShow.SetActive(false);
        upgradeUI.SetActive(true);
        shopUI.SetActive(false);
        codeUI.SetActive(false);
        isUpgradeUIVisible = true;
    }
    public void ShopShop()
    {
        callShow.SetActive(false);
        upgradeUI.SetActive(false);
        shopUI.SetActive(true);
        codeUI.SetActive(false);
    }
    public void ShowCode()
    {
        callShow.SetActive(false);
        upgradeUI.SetActive(false);
        shopUI.SetActive(false);
        codeUI.SetActive(true);
    }
    public void ResetPlayerPrefs()
    {
        PlayerPrefs.DeleteKey("CodeUsed-1");
        PlayerPrefs.DeleteKey("CodeUsed-2");
        PlayerPrefs.DeleteKey("CodeUsed-3");
        PlayerPrefs.DeleteKey("CodeUsed-Vip");
        messageText.text = "Đã xóa trạng thái mã code.";
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
            messageText.text = "You have successfully upgraded Attack!"+"-"+ coin+ " Coin ";
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
            messageText.text = "You have successfully upgraded Health!" + "-" + coin + " Coin ";
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
    public void BuyMana()
    {
        int coin = 200;
        if (PlayerController.me.coin >= coin)
        {
            PlayerController.me.maxMP += 50;
            PlayerPrefs.SetInt("maxMP", PlayerController.me.maxMP);
            PlayerController.me.coin -= coin;
            PlayerPrefs.SetInt("Coin", PlayerController.me.coin);
            messageText.text = "You have successfully upgraded Mana!";
            StartCoroutine(HideMessageAfterDelay(2f));
        }
        else
        {
            messageText.text = "You don't have enough coins to upgrade Mana!";
            messageText.color = Color.red;
            StartCoroutine(HideMessageAfterDelay(2f));
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
            messageText.text = "You have successfully upgraded DF!";
            StartCoroutine(HideMessageAfterDelay(2f));
        }
        else
        {
            messageText.text = "You don't have enough coins to upgrade DF!" + "-" + coin + " Coin ";
            messageText.color = Color.red;
            StartCoroutine(HideMessageAfterDelay(2f));
        }
    }
    public void HideUpgradeUI()
    {
        upgradeUI.SetActive(false);
        isUpgradeUIVisible = false;
    }
     void Upgrade()
    {
        if(PlayerController.me !=null)
        {
            attackText.text ="Attack: "+ "" + PlayerController.me.damageMax.ToString("N0");
            hpText.text = "HP: " + "" + PlayerController.me.maxHP.ToString("N0");
            mpText.text = "MP: " + "" + PlayerController.me.maxMP.ToString("N0");
            defText.text = "Defense: " + "" + PlayerController.me.def.ToString("N0");
            _coinupgradeText.text = "" + PlayerController.me.coin.ToString("N0");
            _diamondupgradeText.text = "" + PlayerController.me.diamond.ToString("N0");

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
                HideUpgradeUI();
            }
        }
    }
    private IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        messageText.text = string.Empty;
    }
}
