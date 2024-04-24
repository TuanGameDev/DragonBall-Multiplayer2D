using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/* Sits on all InventorySlots. */

public class InventorySlot : MonoBehaviour {

	public Image icon;
	public Image iconRemove;
	public Button removeButton;

	Item item;
	public void AddItem (Item newItem)
	{
		item = newItem;

		icon.sprite = item.icon;
		icon.enabled = true;
        iconRemove.enabled = true;
		removeButton.interactable = true;
	}
	public void ClearSlot ()
	{
		item = null;

		icon.sprite = null;
		icon.enabled = false;
        iconRemove.enabled = false;
        removeButton.interactable = false;
	}
	public void RemoveItemFromInventory ()
	{
		Inventory.instance.Remove(item);
	}
	public void UseItem ()
	{
		if (item != null)
		{
			item.Use();
		}
	}

}
