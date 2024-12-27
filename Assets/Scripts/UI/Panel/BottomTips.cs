using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 底部提示UI
/// xushi
/// </summary>
public class BottomTips : BasePanel
{
    static readonly string path = "UI/Panel/BottomTips";

    public BottomTips() : base(new UIType(path)) { }
    //提示图片
    Image image;
    //提示文字
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
    //设置图片
    public void SetImage(Sprite sprite)
    {
        image.sprite = sprite;
    }
    //设置文字
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
