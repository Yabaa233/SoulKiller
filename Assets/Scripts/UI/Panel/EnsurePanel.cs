using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 确认弹窗
/// xushi
/// </summary>
public class EnsurePanel : BasePanel
{
    static readonly string path = "UI/Panel/EnsurePanel";

    public EnsurePanel() : base(new UIType(path)) { }

    //弹窗标题
    Text title;
    //弹窗内容
    Text describe;
    //确认按钮
    Button ok;
    //取消按钮
    Button cancel;

    public override void OnShow(params object[] para)
    {
        base.OnShow(para);
        title = UITool.GetOrAddComponentInChildren<Text>("Txt_Title");
        describe = UITool.GetOrAddComponentInChildren<Text>("Txt_Des");
        ok = UITool.GetOrAddComponentInChildren<Button>("Btn_Ok");
        cancel = UITool.GetOrAddComponentInChildren<Button>("Btn_Cancel");

        //监听
        ok.onClick.AddListener(Ensure);
        cancel.onClick.AddListener(Cancel);
    }

    //确认按钮
    public void Ensure()
    {

    }
    //取消按钮
    public void Cancel()
    {
        Close();
    }
}
