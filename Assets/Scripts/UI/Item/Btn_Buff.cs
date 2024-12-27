using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// �ڽ�ɫbuffչʾ��壬��ʾbuff���������
/// xushi
/// </summary>
public class Btn_Buff : BasePanel
{
    static readonly string path = "UI/Item/Btn_Buff";

    public Btn_Buff() : base(new UIType(path)) { }

    //buffͼƬ
    Image img;

    //ͼ����ϸ��Ϣ
    GameObject buffInfo;
    //ͼ����ϸ��Ϣ����
    Text buffInfoT;

    //�������ͣչʾ��Ϣ ������
    GameObject buffInfoParent;

    //��ǰUI�õ���Э��
    Coroutine openBuffInfo;

    //��ǰbuff�ȼ���Ϣ
    BuffLevelData levelData;

    //��ǰ�Ƿ����չʾ����
    bool canShow = true;

    public override void OnShow(params object[] para)
    {
        base.OnShow(para);
        img = UITool.GetOrAddComponentInChildren<Image>("Image");

        UITool.addTriggersListener(EventTriggerType.PointerEnter, PointerEnter);
        UITool.addTriggersListener(EventTriggerType.PointerExit, PointerExit);

        //���ݸ�ui����ʾ���
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

    //��ͣ����ϸ��Ϣ
    public void OpenInfo()
    {
        buffInfoT.text = levelData.levelDescribe;
        //buffInfo.transform.GetComponent<Image>().sprite = img.sprite;
        //��ʾ��ϸ��Ϣ
        buffInfo.transform.SetParent(buffInfo.transform.parent.parent.parent.parent);
        buffInfo.SetActive(true);
    }
    //�ر���ϸ��Ϣ
    public void CloseInfo()
    {
        buffInfo.transform.SetParent(buffInfoParent.transform);
        buffInfo.SetActive(false);
    }

    //����canShow
    public void SwitchShow(bool a)
    {
        canShow = a;
    }
}
