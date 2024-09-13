using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : Singleton<PopupManager>
{
    [SerializeField] protected GameObject dim = null;
    [SerializeField] protected Popup popup = null;
    protected PopupAnimator obj_Animator = null;
    protected System.Action<PopupButtonType> OnClosedPopupListener;
    private void Awake()
    {
        if (popup == null)
            popup = GetComponentInChildren<Popup>(true);
        obj_Animator = GetComponent<PopupAnimator>();
    }
    public void ShowPopup(string contents)
    {
        ShowPopup(string.Empty, contents);
    }

    public void ShowPopup(string title, string contents)
    {
        PopupInfo info = new PopupInfo.Builder()
            .SetTitle(title)
            .SetContent(contents)
            .SetButtons(PopupButtonType.Confirm)
            .Build();
        ShowPopup(info);
    }
    public void ShowPopup(PopupInfo info)
    {
        if (popup.IsShow)
        {
            return;
        }
        if (info.PauseScene)
            Time.timeScale = 0;

        popup.OnInitialize(info);
        OnClosedPopupListener = info.Listener;
        dim.SetActive(true);
        popup.Show();
        obj_Animator.startAnimation(info.Animation);
    }
    public void HidePopup()
    {
        Time.timeScale = 1;
        dim.SetActive(false);
        popup.Hide();
    }    
    public virtual void OnClosePopup(PopupButtonType type)
    {
        if (OnClosedPopupListener != null)
        {
            OnClosedPopupListener(type);
            OnClosedPopupListener = null;
        }
        HidePopup();
    }
}
