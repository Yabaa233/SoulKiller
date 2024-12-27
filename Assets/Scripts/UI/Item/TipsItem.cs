using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ?tipspanel???tips????
/// xushi
/// </summary>
public class TipsItem : BasePanel
{
    static readonly string path = "UI/Item/TipsItem";

    public TipsItem() : base(new UIType(path)) { }

    //????
    Image background;
    //????
    Image excample;
    //??????
    Text tipsText;

    //????
    Button back;

    //??????
    TipsPanelItem tipsPanelItem;

    public override void OnShow(params object[] para)
    {
        base.OnShow(para);
        Time.timeScale = 0f;
        
        background = UITool.GetOrAddComponentInChildren<Image>("Img_BackGround");
        excample = UITool.GetOrAddComponentInChildren<Image>("Img_Example");
        tipsText = UITool.GetOrAddComponentInChildren<Text>("Txt_Info");

        //???item??
        if (para.Length != 0)
        {
            tipsPanelItem = (TipsPanelItem)para[0];
            excample.sprite = tipsPanelItem.sprite;
        }
        
        tipsText.DOText("回溯到前一个时间节点，请重新舍弃一个能力", 2).SetEase(Ease.Linear);

        back = UITool.GetOrAddComponentInChildren<Button>("Btn_Back");
        back.onClick.AddListener(OpenAbandon);

        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/panelOpen");
    }
    /// <summary>
    /// ??????????????
    /// </summary>
    public void OpenAbandon()
    {
        Close();
        PanelManager.Instance.Open(new SelectPanel(), null, "舍弃");
    }

    public override void OnClose()
    {
        base.OnClose();
        Time.timeScale = 1f;
    }

}
