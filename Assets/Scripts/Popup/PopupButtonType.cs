using System;
using System.ComponentModel;

public enum PopupButtonType
{
    None = 0,
    [Description("��")]
    Yes = 1,
    [Description("�ƴϿ�")]
    No = 2,
    [Description("Ȯ��")]
    Confirm = 3,
    [Description("�ݱ�")]
    Close = 4,
    [Description("�ٽ��ϱ�")]
    RePlay = 5,
    [Description("Ȩ����")]
    GoHome = 6,
    [Description("��������")]
    FinishGame = 7,
    [Description("����")]
    Start = 8
}
