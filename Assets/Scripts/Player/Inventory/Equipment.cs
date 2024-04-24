using UnityEngine;
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Equipment")]
public class Equipment : Item {

	public EquipmentSlot equipSlot;
	public int damage;
    public int hp;
    public int mp;
    public int def;
    public override void Use()
    {
        base.Use();
        EquipmentManager.instance.Equip(this);
        RemoveFromInventory();
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.AddDamage(damage, hp, mp, def);

        }
    }
}

public enum EquipmentSlot { Armor,Trouser,Glove,Shoe,Radar,Bet}
