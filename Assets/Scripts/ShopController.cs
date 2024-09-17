using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ShopController : MonoBehaviour
{
    public RectTransform contents;
    public ShopListItem shopListItemPrefab;


    void Start()
    {
        DataManager.Instance.gold = 1000000;

        //데이터 로드
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
        var item = DataManager.Instance.GetData<ItemData>(itemid);
        var gold = DataManager.Instance.gold;
        var cost = item.cost;
        

        if (gold > cost)
        {
            SoundManager.Instance.PlaySE("구매");
            Debug.Log(item.item_name + "를 구매했습니다!");
            gold -= cost;
            DataManager.Instance.gold = gold;
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
