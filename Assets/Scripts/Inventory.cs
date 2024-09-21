using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] private string sound_Inventory_Open;
    public static bool inventoryActivated = false; 

    public SlotToolTip slotToolTip;
    [SerializeField] private Button invenBtn;
    [SerializeField] private GameObject go_SlotsParent;
    [SerializeField] private GameObject go_InventoryBase;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Slot[] slots;

    void Start()
    {
        invenBtn.onClick.AddListener(() => { OpenInventory(); });
        slots = go_SlotsParent.GetComponentsInChildren<Slot>();
    }

    
    private void OpenInventory()
    {
        go_InventoryBase.SetActive(true);
    }


    public void AcquireItem(Item _item, int _count = 1)
    {
        if(_item.item_type != "장비")
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if(slots[i].item != null)
                {
                    if (slots[i].item.item_name == _item.item_name) 
                    {
                        slots[i].SetSlotCount(_count);
                        return;
                    }
                }
            }
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].AddItem(_item, _count);
                return;
            }
        }
    }
}
