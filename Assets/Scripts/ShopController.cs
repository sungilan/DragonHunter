using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    public RectTransform contents;
    public ShopListItem shopListItemPrefab;
    public Inventory inven;

    void Start()
    {
        DataManager.Instance.LoadData<ItemData>("Data/item_data");
        for (int i = 0; i < 15; i++)
        {
            var listItem = this.CreateListItem();
            var data = DataManager.Instance.GetData<ItemData>(200+i);
            listItem.Init(data.id, data.sprite_name, data.item_name, data.cost);

            listItem.btnBuy.onClick.AddListener(() => { this.BuyItem(data.id); });
        }
    }
    private void BuyItem(int itemid)
    {
        var itemData = DataManager.Instance.GetData<ItemData>(itemid);
        var gold = DataManager.Instance.gold;
        var cost = itemData.cost;
        

        if (gold > cost)
        {
            SoundManager.Instance.PlaySE("����");
            Debug.Log(itemData.item_name + "�� �����߽��ϴ�!");

            // ������ SO�� Resources �������� �ε��մϴ�.
            string itemPath = $"Items/Item_{itemid}";
            Item item = Resources.Load<Item>(itemPath);
            if (item == null)
            {
                Debug.LogError("������ SO�� �ε��� �� �����ϴ�. ���: " + itemPath);
                return;
            }

            // ������ ������Ʈ
            item.sprite_name = itemData.sprite_name;
            item.item_name = itemData.item_name;
            item.item_type = itemData.item_type;
            item.stat = itemData.stat;
            item.cost = itemData.cost;
            item.value = itemData.value;

            gold -= cost;
            DataManager.Instance.gold = gold;
            inven.AcquireItem(item, 1);
        }
        else
        {
            Debug.Log("��尡 �����Ͽ� ������ �� �����ϴ�.");
        }
    }

    private ShopListItem CreateListItem()
    {
        var listItemGo = Instantiate<ShopListItem>(this.shopListItemPrefab);
        listItemGo.transform.SetParent(contents);
        listItemGo.transform.localScale = Vector3.one;
        return listItemGo.GetComponent<ShopListItem>();
    }
}
