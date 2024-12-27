using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// ��ʾ��壬չʾͼ����ʾ
/// xushi
/// </summary>
public class TipsPanel : BasePanel
{
    static readonly string path = "UI/Panel/TipsPanel";

    public TipsPanel() : base(new UIType(path)) { }

    //��ʾ����
    Text title;

    //��ǰҳ��
    Text curPage;
    int curPageNum;
    //����ҳ��
    Text totalPage;
    int totalPageNum;
    //���������ť
    Button turnLeft;
    //���ҹ�����ť
    Button turnRight;

    //���ذ�ť
    Button backButton;
    //ҳ���б�
    GameObject tipsItemList;

    //��ǰ����
    Animator animator;

    //��ǰ����
    TipsPanelSO tipsPanelSO;

    //Contentλ����
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

        //����
        backButton.onClick.AddListener(Close);
        turnLeft.onClick.AddListener(TurnLeft);
        turnRight.onClick.AddListener(TurnRight);

        //��ʼ��
        if (para.Length != 0)
        {
            //��ʼ��������ҳ��
            tipsPanelSO = (TipsPanelSO)para[0];
            title.text = tipsPanelSO.name;
            RefreshPages(1, tipsPanelSO.tipsPanelItems.Count);
        }
        if(totalPageNum!=1)
            backButton.gameObject.SetActive(false);

        //contentPositionList = new List<int>();
        int i = 0;
        //��ʼ��ҳ���б�
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

    //������ⷽ��
    public void PanelSwitch(KeyCode keyCode)
    {
        if (keyCode == KeyCode.Q)
            TurnLeft();
        if (keyCode == KeyCode.E)
            TurnRight();
    }

    //ˢ��ҳ��
    public void RefreshPages(int curP,int totalP)
    {
        curPageNum = curP;
        totalPageNum = totalP;
        curPage.text = curP.ToString();
        totalPage.text = totalP.ToString();
    }
    //����ҳ(���ŷ�ҳ����
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
    //���ҷ�ҳ(���ŷ�ҳ����
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
    ////�ı�Content��λ��
    //public void PositionChange(float x,float y,bool isLeft)
    //{
    //    if(isLeft)
    //        tipsItemList.GetComponent<RectTransform>().anchoredPosition = new Vector3(x+920, y);
    //    else
    //        tipsItemList.GetComponent<RectTransform>().anchoredPosition = new Vector3(x - 920, y);
    //}
}
