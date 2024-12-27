using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ȷ�ϵ���
/// xushi
/// </summary>
public class EnsurePanel : BasePanel
{
    static readonly string path = "UI/Panel/EnsurePanel";

    public EnsurePanel() : base(new UIType(path)) { }

    //��������
    Text title;
    //��������
    Text describe;
    //ȷ�ϰ�ť
    Button ok;
    //ȡ����ť
    Button cancel;

    public override void OnShow(params object[] para)
    {
        base.OnShow(para);
        title = UITool.GetOrAddComponentInChildren<Text>("Txt_Title");
        describe = UITool.GetOrAddComponentInChildren<Text>("Txt_Des");
        ok = UITool.GetOrAddComponentInChildren<Button>("Btn_Ok");
        cancel = UITool.GetOrAddComponentInChildren<Button>("Btn_Cancel");

        //����
        ok.onClick.AddListener(Ensure);
        cancel.onClick.AddListener(Cancel);
    }

    //ȷ�ϰ�ť
    public void Ensure()
    {

    }
    //ȡ����ť
    public void Cancel()
    {
        Close();
    }
}
