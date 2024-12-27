using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 提示面板，展示图文提示
/// xushi
/// </summary>
public class TipsPanel : BasePanel
{
    static readonly string path = "UI/Panel/TipsPanel";

    public TipsPanel() : base(new UIType(path)) { }

    //提示标题
    Text title;

    //当前页码
    Text curPage;
    int curPageNum;
    //所有页码
    Text totalPage;
    int totalPageNum;
    //向左滚动按钮
    Button turnLeft;
    //向右滚动按钮
    Button turnRight;

    //返回按钮
    Button backButton;
    //页面列表
    GameObject tipsItemList;

    //当前动画
    Animator animator;

    //当前数据
    TipsPanelSO tipsPanelSO;

    //Content位置组
    //List<int> contentPositionList;

    public override void OnShow(params object[] para)
    {
        base.OnShow(para);

        title = UITool.GetOrAddComponentInChildren<Text>("Txt_Title");
        curPage = UITool.GetOrAddComponentInChildren<Text>("Txt_curPage");
        totalPage = UITool.GetOrAddComponentInChildren<Text>("Txt_totalPage");

        turnLeft = UITool.GetOrAddComponentInChildren<Button>("Btn_TurnLeft");
        turnRight = UITool.GetOrAddComponentInChildren<Button>("Btn_TurnRight");
        backButton = UITool.GetOrAddComponentInChildren<Button>("Btn_Back");
        tipsItemList = UITool.FindChildGameObject("Content");
        
        //animator = UITool.GetOrAddComponent<Animator>();
        PanelManager.Instance.KeyBoardUpdateAction += PanelSwitch;

        //监听
        backButton.onClick.AddListener(Close);
        turnLeft.onClick.AddListener(TurnLeft);
        turnRight.onClick.AddListener(TurnRight);

        //初始化
        if (para.Length != 0)
        {
            //初始化标题与页码
            tipsPanelSO = (TipsPanelSO)para[0];
            title.text = tipsPanelSO.name;
            RefreshPages(1, tipsPanelSO.tipsPanelItems.Count);
        }
        if(totalPageNum!=1)
            backButton.gameObject.SetActive(false);

        //contentPositionList = new List<int>();
        int i = 0;
        //初始化页面列表
        foreach (var item in tipsPanelSO.tipsPanelItems)
        {
            i += 920;
            PanelManager.Instance.Open(new TipsItem(), tipsItemList.transform,item);
            //contentPositionList.Add(i);
        }


        Time.timeScale = 0f;
    }

    public override void OnClose()
    {
        base.OnClose();
        foreach (var item in PanelManager.Instance.GetAllPanel("TipsItem"))
        {
            item.Close();
        }
        PanelManager.Instance.KeyBoardUpdateAction -= PanelSwitch;
    }

    //按键检测方法
    public void PanelSwitch(KeyCode keyCode)
    {
        if (keyCode == KeyCode.Q)
            TurnLeft();
        if (keyCode == KeyCode.E)
            TurnRight();
    }

    //刷新页码
    public void RefreshPages(int curP,int totalP)
    {
        curPageNum = curP;
        totalPageNum = totalP;
        curPage.text = curP.ToString();
        totalPage.text = totalP.ToString();
    }
    //向左翻页(播放翻页动画
    public void TurnLeft()
    {
        if (curPageNum != 1)
        {
            //animator.SetTrigger("turnLeft");
            tipsItemList.transform.DOMoveX(tipsItemList.transform.position.x + 920, 0.4f);
            curPageNum--;
            RefreshPages(curPageNum, totalPageNum);
        }
    }
    //向右翻页(播放翻页动画
    public void TurnRight()
    {
        if (curPageNum != totalPageNum)
        {
            //animator.SetTrigger("turnRight");
            tipsItemList.transform.DOMoveX(tipsItemList.transform.position.x - 920, 0.4f);
            curPageNum++;
            RefreshPages(curPageNum, totalPageNum);
        }
        if (curPageNum == totalPageNum)
            backButton.gameObject.SetActive(true);
    }
    public override void Close()
    {
        base.Close();
        Time.timeScale = 1.0f;
    }

    //IEnumerator IE_PositionChange(bool isLeft)
    //{
    //    float x = tipsItemList.GetComponent<RectTransform>().anchoredPosition.x;
    //    float y = tipsItemList.GetComponent<RectTransform>().anchoredPosition.y;
    //    float firstTime = Time.realtimeSinceStartup;
    //    while (Time.realtimeSinceStartup - firstTime < 0.5f)
    //    {
    //        yield return null;
    //    }
        
    //}
    ////改变Content的位置
    //public void PositionChange(float x,float y,bool isLeft)
    //{
    //    if(isLeft)
    //        tipsItemList.GetComponent<RectTransform>().anchoredPosition = new Vector3(x+920, y);
    //    else
    //        tipsItemList.GetComponent<RectTransform>().anchoredPosition = new Vector3(x - 920, y);
    //}
}
