using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    public RectTransform contents;
    public ShopListItem shopListItemPrefab;


    void Start()
    {
        //데이터 로드
        DataManager.Instance.LoadData<ItemData>("Data/item_data");
        for (int i = 0; i < 15; i++)
        {
            var listItem = this.CreateListItem();
            var data = DataManager.Instance.GetData<ItemData>(200+i);
            listItem.Init(data.id, data.sprite_name, data.item_name, data.cost);
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
