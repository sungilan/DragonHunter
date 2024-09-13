using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupInfo
{
    private PopupInfo(Builder builder)
    {
        this.Title = builder.Title;
        this.Content = builder.Content;
        this.Buttons = builder.Buttons;
        this.Listener = builder.Listener;
        this.Animation = builder.Animation;
        this.PauseScene = builder.PauseScene;
    }
    public bool PauseScene { get; private set; }
    public string Title { get; private set; }
    public string Content { get; private set; }
    public PopupButtonType[] Buttons { get; private set; }
    public System.Action<PopupButtonType> Listener { get; private set; }
    public PopupAnimationType Animation { get; private set; }


    public class Builder
    {
        public Builder()
        {
            this.Title = string.Empty;
            this.Content = string.Empty;
            this.Buttons = null;
            this.Listener = null;
            this.Animation = PopupAnimationType.None;
            this.PauseScene = false;
        }
        internal string Title { get; private set; }
        internal string Content { get; private set; }
        internal bool PauseScene { get; private set; }
        internal PopupButtonType[] Buttons { get; private set; }
        internal System.Action<PopupButtonType> Listener { get; private set; }
        internal PopupAnimationType Animation { get; private set; }

        public Builder SetTitle(string title)
        {
            this.Title = title;
            return this;
        }
        public Builder SetContent(string content)
        {
            this.Content = content;
            return this;
        }
        public Builder SetButtons(params PopupButtonType[] buttons)
        {
            this.Buttons = buttons;
            return this;
        }
        public Builder SetListiner(System.Action<PopupButtonType> listiner)
        {
            this.Listener = listiner;
            return this;
        }
        public Builder SetAnimation(PopupAnimationType animation)
        {
            this.Animation = animation;
            return this;
        }
        public Builder SetPauseScene(bool isPause)
        {
            this.PauseScene = isPause;
            return this;
        }
        public PopupInfo Build()
        {
            return new PopupInfo(this);
        }
    }
}

