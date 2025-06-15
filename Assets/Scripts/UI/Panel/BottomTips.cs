using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ボトムヒントUI
/// xushi
/// </summary>
public class BottomTips : BasePanel
{
    static readonly string path = "UI/Panel/BottomTips";

    public BottomTips() : base(new UIType(path)) { }
    //ヒント画像
    Image image;
    //ヒントテキスト
    Text text;

    Coroutine closePanel;

    public override void OnShow(params object[] para)
    {
        base.OnShow(para);
        base.OnInit();
        image = UITool.GetOrAddComponentInChildren<Image>("Img");
        text = UITool.GetOrAddComponentInChildren<Text>("Txt");

        if(para.Length > 0)
            text.text=(string)para[0];


        closePanel = MonoHelper.Instance.StartCoroutine(IE_Close());
        coroutines.Add(closePanel); 
    }
    //画像を設定する
    public void SetImage(Sprite sprite)
    {
        image.sprite = sprite;
    }
    //テキストを設定する
    public void SetText(string str)
    {
        text.text = str;
    }

    IEnumerator IE_Close()
    {
        float firstTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - firstTime < 2f)
        {
            yield return null;
        }

        Close();
    }
}
