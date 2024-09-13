using UnityEngine;
using UnityEngine.UI;

public class PopupButton : Button
{
    protected PopupManager obj_manager = null;
    public PopupButtonType enm_type = PopupButtonType.None;
    public PopupButtonType Type
    {
        get { return enm_type; }
        set { enm_type = value; }
    }
    public string Name
    {
        get { return GetComponentInChildren<Text>(true).text; }
        set { GetComponentInChildren<Text>(true).text = value; }
    }
    public Color Color
    {
        get { return GetComponent<Image>().color; }
        set { GetComponent<Image>().color = value; }
    }
    protected override void Start()
    {
        base.Start();
        if(obj_manager == null)
        {
            obj_manager = this.GetComponentInParent<PopupManager>();
        }
    }
    public override void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        if(obj_manager != null)
        {
            obj_manager.OnClosePopup(enm_type);
        }
    }
}
