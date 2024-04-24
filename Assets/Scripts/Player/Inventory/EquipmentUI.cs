using UnityEngine;
using UnityEngine.UI;

public class EquipmentUI : MonoBehaviour
{
    public Image[] equipmentSlots; // Các biểu tượng trang bị sẽ được hiển thị

    private EquipmentManager equipmentManager;

    private void Start()
    {
        equipmentManager = EquipmentManager.instance;
        equipmentManager.onEquipmentChanged += UpdateEquipmentUI;
        UpdateEquipmentUI(null, null);
    }

    private void UpdateEquipmentUI(Equipment newItem, Equipment oldItem)
    {
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            if (i >= equipmentManager.currentEquipment.Length)
            {
                equipmentSlots[i].gameObject.SetActive(false);
                continue;
            }

            Equipment equipment = equipmentManager.currentEquipment[i];

            if (equipment != null)
            {
                equipmentSlots[i].sprite = equipment.icon;
                equipmentSlots[i].gameObject.SetActive(true);
            }
            else
            {
                equipmentSlots[i].gameObject.SetActive(false);
            }
        }
    }
}