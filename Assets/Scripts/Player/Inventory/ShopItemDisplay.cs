using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemDisplay : MonoBehaviour
{
    [Header("Cửa hàng nâng cấp")]
    public GameObject itemDisplayPanel;
    public Image itemImage;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;
    public TextMeshProUGUI itemPriceText;
    public TextMeshProUGUI messageText;

    private Item currentItem;

    public void DisplayItemInfo(Item item)
    {
        currentItem = item;

        itemImage.sprite = item.itemImage;
        itemNameText.text = "Tên Vật Phẩm: "+ item.itemName;
        itemDescriptionText.text = "Thông tin: " + item.itemDescription;
        itemPriceText.text = "Giá: " + item.itemPrice.ToString("N0")+" Vàng";

        itemDisplayPanel.SetActive(true);
    }

    public void HideItemInfo()
    {
        itemDisplayPanel.SetActive(false);
    }

    public void PurchaseItem()
    {
        if (PlayerController.me.coin >= currentItem.itemPrice)
        {
            // Trừ số vàng của người chơi
            PlayerController.me.coin -= (int)currentItem.itemPrice;
            PlayerController.me.AddHP(currentItem.hp);
            PlayerController.me.AddMP(currentItem.mp);
            PlayerController.me.AddAttack(currentItem.damage);
            PlayerController.me.AddDef(currentItem.def);
            PlayerPrefs.SetInt("Coin", PlayerController.me.coin);
            PlayerPrefs.SetInt("maxHP", PlayerController.me.maxHP);
            PlayerPrefs.SetInt("maxMP", PlayerController.me.maxMP);
            PlayerPrefs.SetInt("DamageMax", PlayerController.me.damageMax);
            PlayerPrefs.SetInt("DF", PlayerController.me.def);
            messageText.color = Color.green;
            messageText.text = "Bạn đã mua vật phẩm thành công";
            StartCoroutine(HideMessageAfterDelay(2f));
        }
        else
        {
            messageText.color = Color.red;
            messageText.text = "Bạn không đủ vàng để mua vật phẩm";
            StartCoroutine(HideMessageAfterDelay(2f));
        }
    }
    private IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        messageText.text = string.Empty;
    }
}