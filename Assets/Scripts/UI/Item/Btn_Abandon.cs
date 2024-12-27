using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// �����壬���������ѡ��Ҫ����������
/// xushi
/// </summary>
public class Btn_Abandon : BasePanel
{
    static readonly string path = "UI/Item/Btn_Abandon";
    public Btn_Abandon() : base(new UIType(path)) { }

    //���������ƣ�ԭ�
    Text title;
    //����ͼ��
    Image buffIcon;
    //�������
    Text buffInfo;
    //一级的能力展示
    Text buffLevel1Info;

    //�����buff����
    BuffItemData buffItem;

    //普通背景
    GameObject back;
    //选中状态背景
    GameObject selectBack;
    //选中状态发光
    GameObject light;
    //表面压黑（悬停后移除）
    GameObject topBack;
    
    //动画
    Animator anim;

    //长按协程
    Coroutine pointerDown;


    public override void OnShow(params object[] para)
    {
        base.OnShow(para);
        title = UITool.GetOrAddComponentInChildren<Text>("Txt_BuffName");
        buffIcon = UITool.GetOrAddComponentInChildren<Image>("Img_Buff");
        buffInfo = UITool.GetOrAddComponentInChildren<Text>("Txt_BuffInfo");
        buffLevel1Info = UITool.GetOrAddComponentInChildren<Text>("Txt_BuffRealInfo");

        back = UITool.FindChildGameObject("Img_Back");
        selectBack = UITool.FindChildGameObject("Img_Selected");
        topBack = UITool.FindChildGameObject("Img_TopBlack");
        light = UITool.FindChildGameObject("Img_LIght");
        anim = UITool.GetOrAddComponent<Animator>();

        //����
        //UITool.addTriggersListener(EventTriggerType.PointerClick, PointerClick);
        UITool.addTriggersListener(EventTriggerType.PointerEnter, PointerEnter);
        UITool.addTriggersListener(EventTriggerType.PointerExit, PointerExit);
        UITool.addTriggersListener(EventTriggerType.PointerDown, PointerDown);
        UITool.addTriggersListener(EventTriggerType.PointerUp, PointerUp);


        //��ʾ��Ƭ��Ϣ
        buffItem = (BuffItemData)para[0];
        title.text = buffItem.buffName;
        buffInfo.text = buffItem.buffDescribe;
        buffIcon.sprite = buffItem.buffSprite;
        buffLevel1Info.text = buffItem.buffLevelDatas[0].levelDescribe;

        //表现相关
        selectBack.SetActive(false);
        light.SetActive(false);

        //UITool.GetOrAddComponent<CanvasGroup>().alpha = 0f;
        UITool.GetOrAddComponent<Animator>().SetTrigger("Enter");
    }

    public override void PointerEnter(BaseEventData data)
    {
        base.PointerEnter(data);

        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/buffItemSelect");

        SelectPanel selectPanel = (SelectPanel)PanelManager.Instance.GetPanel("SelectPanel");
        selectPanel.HighLight(buffItem.buffKind);

        selectBack.SetActive(true);
        light.SetActive(true);
        topBack.SetActive(false) ;
        buffInfo.color = Color.black;
    }

    public override void PointerExit(BaseEventData data)
    {
        base.PointerExit(data);

        selectBack.SetActive(false);
        light.SetActive(false);
        topBack.SetActive(true);

        buffInfo.color = Color.white;
    }

    public void PointerDown(BaseEventData data)
    {
        pointerDown = MonoHelper.Instance.StartCoroutine(IE_PointerDown());
        coroutines.Add(pointerDown);

    }
    public void PointerUp(BaseEventData data)
    {
        if(pointerDown != null)
        {
            MonoHelper.Instance.StopCoroutine(pointerDown);
        }
        anim.SetTrigger("CancelAbondon");
    }

    //长按逻辑
    IEnumerator IE_PointerDown()
    {
        anim.SetTrigger("StartAbondon");

        float firstTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - firstTime < 1.6f)
        {
            yield return null;
        }
        I_BuffBase buffBase = GameManager.Instance.currentPlayer.characterBuffManager.indexDictionary[buffItem.buffKind];
        E_ChararcterType chararcterType = E_ChararcterType.enemy;
        int level = buffBase.GetLevel();
        BuffDataManager.Instance.enemyBuffList.Add(BuffDataManager.Instance.GenerateBuff(buffItem.buffKind, chararcterType, level));
        BuffDataManager.Instance.RefreshEnemyBuff();
        //�������buff�ĵȼ�
        int buffLevel = GameManager.Instance.currentPlayer.characterBuffManager.indexDictionary[buffItem.buffKind].GetLevel();
        GameManager.Instance.currentPlayer.characterBuffManager.RemoveBuff(buffItem.buffKind);
        GameManager.Instance.currentPlayer.characterBuffManager.RefreshData();
        BuffDataManager.Instance.RefreshBuff();

        PanelManager.Instance.Close(PanelManager.Instance.GetPanel("SelectPanel").UIType);
        //Time.timeScale = 1.0f;
        Debug.Log("������buff");

        //���ݹ�������������
        PanelManager.Instance.Open(new SelectPanel(), null, "进阶", buffLevel+1);

        //刷新主界面按钮
        Area_MainBuff area=PanelManager.Instance.GetPanel("Area_MainBuff") as Area_MainBuff;
        area.RefreshBuffButton();
    }
}
