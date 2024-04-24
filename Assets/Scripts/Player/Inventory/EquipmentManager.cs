using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;
using static UnityEditor.Timeline.Actions.MenuPriority;

public class EquipmentManager : MonoBehaviour
{
    #region Singleton
    public static EquipmentManager instance;

    void Awake()
    {
        instance = this;
        currentEquipment = new Equipment[System.Enum.GetValues(typeof(EquipmentSlot)).Length];
    }
    #endregion

    public Equipment[] currentEquipment;
    public delegate void OnEquipmentChanged(Equipment newItem, Equipment oldItem);
    public OnEquipmentChanged onEquipmentChanged;

    Inventory inventory;
    private Equipment gloveEquipment;
    void Start()
    {
        inventory = Inventory.instance;
    }
    public void Equip(Equipment newItem)
    {
        int slotIndex = (int)newItem.equipSlot;

        Equipment oldItem = null;
        if (currentEquipment[slotIndex] != null)
        {
            oldItem = currentEquipment[slotIndex];
            Duplicateequipment(oldItem); // Gỡ bỏ trang bị cũ trước khi trang bị mới
        }

        currentEquipment[slotIndex] = newItem;
        if (onEquipmentChanged != null)
        {
            onEquipmentChanged.Invoke(newItem, oldItem);
        }

        switch (newItem.equipSlot)
        {
            case EquipmentSlot.Armor:
            case EquipmentSlot.Trouser:
            case EquipmentSlot.Glove:
            case EquipmentSlot.Shoe:
            case EquipmentSlot.Radar:
            case EquipmentSlot.Bet:
                gloveEquipment = newItem;
                break;
        }
    }
    public void Duplicateequipment(Equipment item)
    {
        if (item != null)
        {
            int slotIndex = (int)item.equipSlot;

            if (currentEquipment[slotIndex] == item)
            {
                inventory.Add(item);
                currentEquipment[slotIndex] = null;
                if (onEquipmentChanged != null)
                {
                    onEquipmentChanged.Invoke(null, item);
                }

                if (item.equipSlot == EquipmentSlot.Glove)
                {
                    gloveEquipment = null;
                }

                RemoveEffects(item);
            }
        }
    }
    public void Unequip1(int slotIndex)
    {
        if (currentEquipment[slotIndex] != null)
        {
            Equipment oldItem = currentEquipment[slotIndex];
            inventory.Add(oldItem);
            currentEquipment[slotIndex] = null;
            if (onEquipmentChanged != null)
            {
                onEquipmentChanged.Invoke(null, oldItem);
            }
            RemoveEffects(oldItem);
        }
    }
    public void RemoveEffects(Equipment item)
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null && item != null)
        {
            player.RemoveDamage(item.damage);
            player.RemoveHP(item.hp);
            player.RemoveMP(item.mp);
            player.RemoveDefense(item.def);
        }

        switch (item.equipSlot)
        {
            case EquipmentSlot.Armor:
            case EquipmentSlot.Trouser:
            case EquipmentSlot.Glove:
            case EquipmentSlot.Shoe:
            case EquipmentSlot.Radar:
            case EquipmentSlot.Bet:
                gloveEquipment = null;
                break;
        }
    }
}