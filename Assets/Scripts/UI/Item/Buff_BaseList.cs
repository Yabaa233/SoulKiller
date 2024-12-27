using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 属性和武器相关的Buff，显示当前buff和buff的升级情况
/// xushi
/// </summary>
public class Buff_BaseList : BasePanel
{
    //static readonly string path = "";

    public Buff_BaseList(UIType uIType) : base(uIType) { }

    //当前Buff物体
    GameObject buff;
    //当前Buff图片
    Image buffImage;
    //当前buff升级情况列表
    GameObject buffLevelList;
    //图标详细信息
    GameObject buffInfo;
    //图标详细信息文字
    Text buffInfoT;

    //传入的信息
    BuffItemData itemData;
    int buffLevel;

    //当前是否可以展示详情
    bool canShow=true;

    //悬停打开介绍面板的协程
    Coroutine openBuffInfo;

    Material material;

    public override void OnShow(params object[] para)
    {
        base.OnShow(para);
        buff = UITool.FindChildGameObject("Btn_Weapon");
        buffImage = buff.GetComponent<Image>();
        buffLevelList = UITool.FindChildGameObject("BuffChild(Scroll View").transform.Find("Viewport").Find("Content").gameObject;
        buffInfo = UITool.FindChildGameObject("Img_Info");
        buffInfoT = buffInfo.GetComponentInChildren<Text>();

        //监听鼠标移入动画
        UITool.addTriggersListener(EventTriggerType.PointerEnter, PointerEnter, buff);
        UITool.addTriggersListener(EventTriggerType.PointerExit, PointerExit, buff);

        buffInfo.SetActive(false);

        //初始化当前buff的信息
        itemData = (BuffItemData)para[0];
        buffLevel = (int)para[1];

        buffImage.sprite = itemData.buffSprite;

        //buffImage.material = new Material("Slash/Blended");
        material=buffImage.material;

        material= Object.Instantiate(material);

        //初始化升级小图标
        InitBuffLevelIcon();
    }

    public override void OnClose()
    {
        base.OnClose();
        foreach (var item in PanelManager.Instance.GetAllPanel("Btn_Buff"))
        {
            item.Close();
        }

    }

    //更改canShow
    public void SwitchShow(bool a)
    {
        canShow = a;
    }

    //加载当前buff的升级
    public void InitBuffLevelIcon()
    {

        //加载
        for (int i = 0; i < buffLevel; i++)
        {
            PanelManager.Instance.Open(new Btn_Buff(), buffLevelList.transform, buffInfo,itemData.buffLevelDatas[i] ,UITool.GetUI());
        }
        
    }
    //显示鼠标悬停图标的详细信息
    public override void PointerEnter(BaseEventData data)
    {
        material.SetFloat("_TintColorIntensity", 2.5f);
        buffImage.material = material; 

        if (canShow)
        {
            openBuffInfo= MonoHelper.Instance.StartCoroutine(IE_OpenInfo());
            coroutines.Add(openBuffInfo);
        }

        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/buffItemSelect");
    }
    //关闭鼠标悬停图标的详细信息
    public override void PointerExit(BaseEventData data)
    {

        material.SetFloat("_TintColorIntensity", 1f);
        buffImage.material = material;
        if (openBuffInfo != null)
            MonoHelper.Instance.StopCoroutine(openBuffInfo);
        CloseInfo();
    }

    IEnumerator IE_OpenInfo()
    {
        float firstTime = Time.realtimeSinceStartup;
        while(Time.realtimeSinceStartup - firstTime < 0.5f)
        {
            yield return null;
        }
        OpenInfo();

    }
    //打开鼠标悬停图标的详细信息
    public void OpenInfo()
    {

        buffInfoT.text = itemData.buffDescribe;
        //显示详细信息
        buffInfo.transform.SetParent(buffInfo.transform.parent.parent.parent.parent);
        buffInfo.SetActive(true);

    }
    //关闭鼠标悬停图标的详细信息
    public  void CloseInfo()
    {
        buffInfo.transform.SetParent(UITool.GetUI().transform);
        buffInfo.SetActive(false);
    }

}
