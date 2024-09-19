using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class SlotToolTip : MonoBehaviour {

    // 필요한 컴포넌트
    [SerializeField] private GameObject go_Base; // 툴팁 베이스
    [SerializeField] private TextMeshProUGUI txt_ItemName; // 아이템 이름
    [SerializeField] private TextMeshProUGUI txt_ItemDesc; // 아이템 설명
    [SerializeField] private TextMeshProUGUI txt_ItemHowtoUsed; // 아이템 사용방법
    [SerializeField] private TextMeshProUGUI txt_type;
    [SerializeField] private TextMeshProUGUI txt_cost;
    [SerializeField] private TextMeshProUGUI txt_value;
    [SerializeField] private Image item_Image;


    public void ShowToolTip(Item _item, Vector3 _pos)
    {
        go_Base.SetActive(true);
        _pos += new Vector3(go_Base.GetComponent<RectTransform>().rect.width * 0.5f, -go_Base.GetComponent<RectTransform>().rect.height, 0f);
        go_Base.transform.position = _pos;

        txt_ItemName.text = _item.item_name;
        txt_type.text = "타입 : " + _item.item_type;
        txt_cost.text = "가격 : " + _item.cost.ToString();
        txt_value.text = "효과 : " + _item.stat + " " + _item.value + " 상승";
        item_Image.sprite = AssetManager.Instance.atlas.GetSprite(_item.sprite_name);
        //txt_ItemDesc.text = _item.itemDesc;

        //if (_item.item_type == Item.ItemType.Equipment)
        //    txt_ItemHowtoUsed.text = "우클릭 - 장착";
        //else if (_item.item_type == Item.ItemType.Used)
        //    txt_ItemHowtoUsed.text = "우클릭 - 먹기";
        //else
        //    txt_ItemHowtoUsed.text = "";
    }

    public void HideToolTip()
    {
        go_Base.SetActive(false);
    }
}