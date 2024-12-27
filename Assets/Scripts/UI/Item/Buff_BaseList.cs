using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// ���Ժ�������ص�Buff����ʾ��ǰbuff��buff���������
/// xushi
/// </summary>
public class Buff_BaseList : BasePanel
{
    //static readonly string path = "";

    public Buff_BaseList(UIType uIType) : base(uIType) { }

    //��ǰBuff����
    GameObject buff;
    //��ǰBuffͼƬ
    Image buffImage;
    //��ǰbuff��������б�
    GameObject buffLevelList;
    //ͼ����ϸ��Ϣ
    GameObject buffInfo;
    //ͼ����ϸ��Ϣ����
    Text buffInfoT;

    //�������Ϣ
    BuffItemData itemData;
    int buffLevel;

    //��ǰ�Ƿ����չʾ����
    bool canShow=true;

    //��ͣ�򿪽�������Э��
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

        //����������붯��
        UITool.addTriggersListener(EventTriggerType.PointerEnter, PointerEnter, buff);
        UITool.addTriggersListener(EventTriggerType.PointerExit, PointerExit, buff);

        buffInfo.SetActive(false);

        //��ʼ����ǰbuff����Ϣ
        itemData = (BuffItemData)para[0];
        buffLevel = (int)para[1];

        buffImage.sprite = itemData.buffSprite;

        //buffImage.material = new Material("Slash/Blended");
        material=buffImage.material;

        material= Object.Instantiate(material);

        //��ʼ������Сͼ��
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

    //����canShow
    public void SwitchShow(bool a)
    {
        canShow = a;
    }

    //���ص�ǰbuff������
    public void InitBuffLevelIcon()
    {

        //����
        for (int i = 0; i < buffLevel; i++)
        {
            PanelManager.Instance.Open(new Btn_Buff(), buffLevelList.transform, buffInfo,itemData.buffLevelDatas[i] ,UITool.GetUI());
        }
        
    }
    //��ʾ�����ͣͼ�����ϸ��Ϣ
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
    //�ر������ͣͼ�����ϸ��Ϣ
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
    //�������ͣͼ�����ϸ��Ϣ
    public void OpenInfo()
    {

        buffInfoT.text = itemData.buffDescribe;
        //��ʾ��ϸ��Ϣ
        buffInfo.transform.SetParent(buffInfo.transform.parent.parent.parent.parent);
        buffInfo.SetActive(true);

    }
    //�ر������ͣͼ�����ϸ��Ϣ
    public  void CloseInfo()
    {
        buffInfo.transform.SetParent(UITool.GetUI().transform);
        buffInfo.SetActive(false);
    }

}
