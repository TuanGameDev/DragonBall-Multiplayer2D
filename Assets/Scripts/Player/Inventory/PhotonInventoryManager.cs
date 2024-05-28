using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class PhotonInventoryManager : MonoBehaviourPunCallbacks
{
    public int maxInventorySize = 10; // Kích thước tối đa của inventory
    public List<Item> inventory = new List<Item>();
    public Image[] icon;
    public int item;
    [PunRPC]
     void testitem(int Item)
    {
        item += Item;
    }
    public void AddItem(Item item)
    {
        if (inventory.Count < maxInventorySize) // Kiểm tra nếu inventory chưa đạt đến kích thước tối đa
        {
            inventory.Add(item);
            photonView.RPC("AddItemRPC", RpcTarget.AllBuffered, item.itemName);
        }
        else
        {
            Debug.Log("Inventory is full!"); // Hiển thị thông báo khi inventory đã đầy
        }
    }

    [PunRPC]
    private void AddItemRPC(string itemName)
    {
        Item itemToAdd = inventory.Find(x => x.itemName == itemName);
        if (itemToAdd != null)
        {
            inventory.Add(itemToAdd);
            UpdateIcons();
        }
    }

    private void UpdateIcons()
    {
        for (int i = 0; i < icon.Length; i++)
        {
            if (i < inventory.Count && inventory[i].itemImage != null)
            {
                icon[i].sprite = inventory[i].itemImage;
                icon[i].gameObject.SetActive(true);
            }
            else
            {
                icon[i].gameObject.SetActive(false);
            }
        }
    }
}