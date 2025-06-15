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
        
        tipsText.DOText("前の時間点に戻って、能力を再度放棄してください。", 2).SetEase(Ease.Linear);

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
        PanelManager.Instance.Open(new SelectPanel(), null, "Abandon");
    }

    public override void OnClose()
    {
        base.OnClose();
        Time.timeScale = 1f;
    }

}
