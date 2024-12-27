using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RoomTipPanel : BasePanel
{
    static readonly string path = "UI/Panel/RoomTipPanel";
    
    public RoomTipPanel() : base(new UIType(path)){}

    public Text title;
    public Text storyText;
    public Image mainImage;
    public Text scribleText;
    public Button returnButton;

    public override void OnInit()
    {
        base.OnInit();
    }

    public override void OnShow(params object[] para)
    {
        base.OnShow(para);
        Time.timeScale = 0;
        // title = UITool.GetOrAddComponentInChildren<Text>("Title");
        storyText = UITool.GetOrAddComponentInChildren<Text>("StoryText");
        mainImage = UITool.GetOrAddComponentInChildren<Image>("MainImage");
        scribleText = UITool.GetOrAddComponentInChildren<Text>("ScribleText");
        returnButton = UITool.GetOrAddComponentInChildren<Button>("Return");

        foreach (var buffItem in PanelManager.Instance.buffInfoListSO.buffItems)
        {
            if (buffItem.buffKind == PanelManager.Instance.currentE_BuffKind)
            {
                mainImage.sprite = buffItem.buffSpriteText;
                // title.text = buffItem.buffName;
                storyText.text = buffItem.Levelcondition;
                scribleText.text = buffItem.LevelTips;
            }
        }
        returnButton.onClick.AddListener(CloseMySelf);

        //暂时隐藏主面板
        SetMainUIActive(false);
    }


    public void CloseMySelf()
    {
        Time.timeScale = 1;
        //显示主面板
        SetMainUIActive(true);
        Close();
    }

    public override void OnClose()
    {
        base.OnClose();
        (PanelManager.Instance.GetPanel("BattleMainPanel") as BattleMainPanel).FadeChange(true);
    }
    public void SetMainUIActive(bool state)
    {
        // Debug.Log("运行了");
        BattleMainPanel battleMainPanel = PanelManager.Instance.GetPanel("BattleMainPanel") as BattleMainPanel;
        battleMainPanel.UITool.GetUI().SetActive(state);

        Area_MainBuff area_MainBuff = PanelManager.Instance.GetPanel("Area_MainBuff") as Area_MainBuff;
        area_MainBuff.UITool.GetUI().SetActive(state);
    }
}
