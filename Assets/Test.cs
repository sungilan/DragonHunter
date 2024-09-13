using UnityEngine;

public class Test : MonoBehaviour
{
    public KeyCode key_popupConfirm;
    public KeyCode key_popupYesNo;

    void Update()
    {
        if(Input.GetKeyUp(key_popupConfirm)) 
        {
            PopupInfo info = new PopupInfo.Builder()
                .SetTitle("���� ����")
                .SetAnimation(PopupAnimationType.Alpha)
                .SetContent("�������� �˾��Դϴ�.")
                .SetButtons(PopupButtonType.Confirm)
                .SetListiner(OnClickedPopupButton)
                .Build();
            PopupManager.Instance.ShowPopup(info);
        }
        if (Input.GetKeyUp(key_popupConfirm))
        {
            PopupInfo info = new PopupInfo.Builder()
                .SetTitle("���� �˾�")
                .SetAnimation(PopupAnimationType.Alpha)
                .SetContent("���� �˾��Դϴ�.")
                .SetButtons(PopupButtonType.Confirm)
                .SetListiner(OnClickedPopupButton)
                .Build();
            PopupManager.Instance.ShowPopup(info);
        }
    }
    private void OnClickedPopupButton(PopupButtonType type)
    {
        Debug.Log("OnClicked_PopupButton");
        PopupManager.Instance.HidePopup();

        switch (type) 
        {
            case PopupButtonType.Yes:
                {
                    Debug.Log("Yes Button");
                    break;
                }
            case PopupButtonType.Confirm:
                {
                    Debug.Log("Confirm Button");
                    break;
                }
            case PopupButtonType.No:
                {
                    Debug.Log("No Button");
                    break;
                }
        }
    }
}
