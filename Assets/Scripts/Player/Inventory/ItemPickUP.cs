using UnityEngine;

public class ItemPickUP : MonoBehaviour
{
    public Item _item;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            AddItemToInventory();
            Destroy(gameObject);
        }
    }
    private void AddItemToInventory()
    {
        Inventory.instance.Add(_item);
    }
}