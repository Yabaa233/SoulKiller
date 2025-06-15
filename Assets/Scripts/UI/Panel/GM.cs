using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// UIテストを行うためのウィンドウ
/// xushi
/// </summary>
public class GM : BasePanel
{
    static readonly string path = "UI/Panel/GM";
    public GM() : base(new UIType(path)) { }


    //GM
    Button openBuffInfo;
    Button openSelectAbandon;
    Button openSelectLevelUp;
    Button openTipsPanel;
    Button openBottonTips;

    //Button initBuff;
    Button removeBuff;

    public override void OnShow(params object[] para)
    {
        base.OnShow(para);
        openBuffInfo = UITool.GetOrAddComponentInChildren<Button>("Btn_Buff");
        openSelectAbandon = UITool.GetOrAddComponentInChildren<Button>("Btn_Buff2");
        openSelectLevelUp = UITool.GetOrAddComponentInChildren<Button>("Btn_Buff3");
        //openTipsPanel = UITool.GetOrAddComponentInChildren<Button>("Btn_Buff4");
        //openBottonTips = UITool.GetOrAddComponentInChildren<Button>("Btn_Buff5");
        //initBuff = UITool.GetOrAddComponentInChildren<Button>("Btn_Buff6");
        //removeBuff = UITool.GetOrAddComponentInChildren<Button>("Btn_Buff7");

        openBuffInfo.onClick.AddListener(() => { PanelManager.Instance.Open(new BuffInfoPanel()); });
        openSelectAbandon.onClick.AddListener(() => { PanelManager.Instance.Open(new SelectPanel(), null, "Abandon"); });
        openSelectLevelUp.onClick.AddListener(() => { PanelManager.Instance.Open(new SelectPanel(), null, "Advanced"); });
        //openTipsPanel.onClick.AddListener(() => { PanelManager.Instance.Open(new TipsPanel()); });
        //openBottonTips.onClick.AddListener(() => { PanelManager.Instance.Open(new BottomTips()); });

        //initBuff.onClick.AddListener(() => { Test1.Instance.InitPlayerBuff(); });
        //openTipsPanel.onClick.AddListener(() => { PanelManager.Instance.Close(PanelManager.Instance.GetPanel("BattleMainPanel").UIType); });
        // Test1.Instance.InitPlayerBuff();
    }
}
