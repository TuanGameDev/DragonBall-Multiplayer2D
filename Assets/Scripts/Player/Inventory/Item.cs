using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
[System.Serializable]
public class Item:ScriptableObject
{
    public string itemName;
    public int hp;
    public int mp;
    public int damage;
    public int def;
    public string itemDescription;
    public float itemPrice;
    public Sprite itemImage;

    public Item(string name, string description, float price, Sprite image)
    {
        itemName = name;
        itemDescription = description;
        itemPrice = price;
        itemImage = image;
    }
}