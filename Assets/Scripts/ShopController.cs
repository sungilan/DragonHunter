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
            SoundManager.Instance.PlaySE("구매");
            Debug.Log(itemData.item_name + "를 구매했습니다!");

            // 아이템 SO를 Resources 폴더에서 로드합니다.
            string itemPath = $"Items/Item_{itemid}";
            Item item = Resources.Load<Item>(itemPath);
            if (item == null)
            {
                Debug.LogError("아이템 SO를 로드할 수 없습니다. 경로: " + itemPath);
                return;
            }

            // 데이터 업데이트
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
            Debug.Log("골드가 부족하여 구매할 수 없습니다.");
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
