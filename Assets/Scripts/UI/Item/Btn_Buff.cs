using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// 在角色buff展示面板，显示buff的升级情况
/// xushi
/// </summary>
public class Btn_Buff : BasePanel
{
    static readonly string path = "UI/Item/Btn_Buff";

    public Btn_Buff() : base(new UIType(path)) { }

    //buff图片
    Image img;

    //图标详细信息
    GameObject buffInfo;
    //图标详细信息文字
    Text buffInfoT;

    //传入的悬停展示信息 父物体
    GameObject buffInfoParent;

    //当前UI用到的协程
    Coroutine openBuffInfo;

    //当前buff等级信息
    BuffLevelData levelData;

    //当前是否可以展示详情
    bool canShow = true;

    public override void OnShow(params object[] para)
    {
        base.OnShow(para);
        img = UITool.GetOrAddComponentInChildren<Image>("Image");

        UITool.addTriggersListener(EventTriggerType.PointerEnter, PointerEnter);
        UITool.addTriggersListener(EventTriggerType.PointerExit, PointerExit);

        //传递父ui的显示组件
        if (para.Length!=0)
        {
            if (para.Length == 3)
            {
                buffInfo = (GameObject)para[0];
                levelData = (BuffLevelData)para[1];
                buffInfoParent = (GameObject)para[2];

                buffInfoT = buffInfo.transform.GetChild(0).GetComponent<Text>();
            }
            else
            {
                levelData = (BuffLevelData)para[0];
            }
            
        }
        
        img.sprite = levelData.levelSprite;

    }

    public override void PointerEnter(BaseEventData data)
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/smallItemSelect");
        img.color = new Color(1, 1, 1, 1);
        
        if (canShow)
        {
            base.PointerEnter(data);
            openBuffInfo = MonoHelper.Instance.StartCoroutine(IE_OpenInfo());
            coroutines.Add(openBuffInfo);
        }
        
    }

    public override void PointerExit(BaseEventData data)
    {
        img.color =new Color(1f, 1f, 1f, 0.5f);
        if (openBuffInfo != null)
            MonoHelper.Instance.StopCoroutine(openBuffInfo);
        CloseInfo();
        
    }

    IEnumerator IE_OpenInfo()
    {
        float firstTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - firstTime < 0.5f)
        {
            yield return null;
        }
        OpenInfo();
    }

    //悬停打开详细信息
    public void OpenInfo()
    {
        buffInfoT.text = levelData.levelDescribe;
        //buffInfo.transform.GetComponent<Image>().sprite = img.sprite;
        //显示详细信息
        buffInfo.transform.SetParent(buffInfo.transform.parent.parent.parent.parent);
        buffInfo.SetActive(true);
    }
    //关闭详细信息
    public void CloseInfo()
    {
        buffInfo.transform.SetParent(buffInfoParent.transform);
        buffInfo.SetActive(false);
    }

    //更改canShow
    public void SwitchShow(bool a)
    {
        canShow = a;
    }
}
