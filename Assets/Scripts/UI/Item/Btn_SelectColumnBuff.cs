using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// 显示小buff图标
/// </summary>
public class Btn_SelectColumnBuff : BasePanel
{
    static readonly string path = "UI/Item/Btn_SelectColumnBuff";

    public Btn_SelectColumnBuff() : base(new UIType(path)) { }

    //buff图片
    Image image;
    //当前buff类型名
    //E_BuffKind buffKind;
    //当前buff信息
    BuffItemData itemData;
    //buff的等级
    int buffLevel;
    //buff的等级显示
    GameObject buffLevelList;
    //buff的等级列表
    GameObject buffLevelListContent;

    //悬停打开介绍面板的协程
    Coroutine openBuffInfo;
    //悬停打开介绍面板的压黑
    GameObject leftFocusGO;

    //是否是敌人
    bool isEnemy = false;

    Material material;

    public override void OnShow(params object[] para)
    {
        base.OnShow(para);
        image = UITool.GetOrAddComponentInChildren<Image>("Image");

        buffLevelList = UITool.FindChildGameObject("BuffChild(Scroll View");
        buffLevelListContent = UITool.FindChildGameObject("Content");

        UITool.addTriggersListener(UnityEngine.EventSystems.EventTriggerType.PointerEnter, PointerEnter);
        UITool.addTriggersListener(UnityEngine.EventSystems.EventTriggerType.PointerExit, PointerExit);

        material = image.material;

        material = Object.Instantiate(material);

        //传入是否高亮显示
        if (para.Length != 0)
        {
            if (para.Length == 1)
            {
                image.color = new Color(1, 1, 1, (float)para[0]);
            }
            else
            {
                itemData = (BuffItemData)para[3];
                image.color = new Color(1, 1, 1, (float)para[0]);
                buffLevel = (int)para[1];
                leftFocusGO = (GameObject)para[2];
            }


        }
        if((float)para[0] == 0.4f)
            isEnemy= true;

        material.SetFloat("_TintColorIntensity", 2*(float)para[0]);
        image.material = material;

        image.sprite = itemData.buffSprite;

        buffLevelList.SetActive(false);
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

    //加载当前buff的升级
    public void InitBuffLevelIcon()
    {
        if (isEnemy)
            return;
        for (int i = 0; i < buffLevel; i++)
        {
            PanelManager.Instance.Open(new Btn_LevelColumn(), buffLevelListContent.transform,itemData.buffLevelDatas[i]);
        }
    }

    public override void PointerEnter(BaseEventData data)
    {
        if (isEnemy)
            return;

        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/smallItemSelect");

        base.PointerEnter(data);

        material.SetFloat("_TintColorIntensity", 2.5f);
        image.material = material;

        openBuffInfo = MonoHelper.Instance.StartCoroutine(IE_OpenLevelUp());
        coroutines.Add(openBuffInfo);
    }
    public override void PointerExit(BaseEventData data)
    {
        if (isEnemy)
            return;
        base.PointerExit(data);
        material.SetFloat("_TintColorIntensity", 1f);
        if (openBuffInfo != null)
            MonoHelper.Instance.StopCoroutine(openBuffInfo);
        CloseLevelUp();
    }

    IEnumerator IE_OpenLevelUp()
    {
        leftFocusGO.SetActive(true);
        float firstTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - firstTime < 0.5f)
        {
            yield return null;
        }
        OpenLevelUp();
    }

    //显示当前buff的升级
    public void OpenLevelUp()
    {
        buffLevelList.SetActive(true);
        buffLevelList.transform.SetParent(UITool.GetUI().transform.parent.parent.parent);
    }
    //关闭当前buff的升级
    public void CloseLevelUp()
    {
        leftFocusGO.SetActive(false);
        buffLevelList.SetActive(false);
        buffLevelList.transform.SetParent(UITool.GetUI().transform);
    }
}
