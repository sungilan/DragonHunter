using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopListItem : MonoBehaviour
{
    public int id;
    public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI costText;
    public Button btnBuy;

    public void Init(int id, string sprite_name,string item_name, int cost)
    {
        this.id = id;
        this.icon.sprite = AssetManager.Instance.atlas.GetSprite(sprite_name);
        this.nameText.text = item_name;
        this.costText.text = cost.ToString();
    }
}
