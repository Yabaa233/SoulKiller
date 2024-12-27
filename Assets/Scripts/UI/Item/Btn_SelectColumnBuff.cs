using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// ��ʾСbuffͼ��
/// </summary>
public class Btn_SelectColumnBuff : BasePanel
{
    static readonly string path = "UI/Item/Btn_SelectColumnBuff";

    public Btn_SelectColumnBuff() : base(new UIType(path)) { }

    //buffͼƬ
    Image image;
    //��ǰbuff������
    //E_BuffKind buffKind;
    //��ǰbuff��Ϣ
    BuffItemData itemData;
    //buff�ĵȼ�
    int buffLevel;
    //buff�ĵȼ���ʾ
    GameObject buffLevelList;
    //buff�ĵȼ��б�
    GameObject buffLevelListContent;

    //��ͣ�򿪽�������Э��
    Coroutine openBuffInfo;
    //��ͣ�򿪽�������ѹ��
    GameObject leftFocusGO;

    //�Ƿ��ǵ���
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

        //�����Ƿ������ʾ
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

    //���ص�ǰbuff������
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

    //��ʾ��ǰbuff������
    public void OpenLevelUp()
    {
        buffLevelList.SetActive(true);
        buffLevelList.transform.SetParent(UITool.GetUI().transform.parent.parent.parent);
    }
    //�رյ�ǰbuff������
    public void CloseLevelUp()
    {
        leftFocusGO.SetActive(false);
        buffLevelList.SetActive(false);
        buffLevelList.transform.SetParent(UITool.GetUI().transform);
    }
}
