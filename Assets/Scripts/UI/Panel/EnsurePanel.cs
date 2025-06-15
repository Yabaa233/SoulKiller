using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
///<summary>

////// 確認ポップアップ
/// xushi

///</summary>
public class EnsurePanel : BasePanel
{
    static readonly string path = "UI/Panel/EnsurePanel";

    public EnsurePanel() : base(new UIType(path)) { }

    //ポップアップタイトル
    Text title;
    //ポップアップコンテンツ
    Text describe;
    //確認ボタン
    Button ok;
    //キャンセルボタン
    Button cancel;

    public override void OnShow(params object[] para)
    {
        base.OnShow(para);
        title = UITool.GetOrAddComponentInChildren<Text>("Txt_Title");
        describe = UITool.GetOrAddComponentInChildren<Text>("Txt_Des");
        ok = UITool.GetOrAddComponentInChildren<Button>("Btn_Ok");
        cancel = UITool.GetOrAddComponentInChildren<Button>("Btn_Cancel");

        //監視
        ok.onClick.AddListener(Ensure);
        cancel.onClick.AddListener(Cancel);
    }

    //確認ボタン
    public void Ensure()
    {

    }
    //キャンセルボタン
    public void Cancel()
    {
        Close();
    }
}
