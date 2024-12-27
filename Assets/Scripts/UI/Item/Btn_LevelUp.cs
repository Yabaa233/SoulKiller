using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// ��ʾ���������壬���������ѡ������
/// xushi
/// </summary>
public class Btn_LevelUp : BasePanel
{
    static readonly string path = "UI/Item/Btn_LevelUp";
    public Btn_LevelUp() : base(new UIType(path)) { }

    //buff�ȼ�����
    Text levelInfo;
    //buff�ȼ�ͼ��
    Image levelIcon;
    //��ǰ����buffͼ��
    Image buffIcon;
    //��ǰ����buff����
    Text buffName;

    //�����buff����
    S_BuffKindAndLevel buffItem;
    BuffItemData itemData;

    //��һ������buff�ĵȼ��������Ƿ��ٴ�������
    int abandonBuffLevel;

    //底部发光
    GameObject light;
    //背景选中状态
    GameObject selectBack;
    //背景
    GameObject back;

    public override void OnShow(params object[] para)
    {
        base.OnShow(para);
        levelInfo = UITool.GetOrAddComponentInChildren<Text>("Txt_BuffInfo");
        levelIcon = UITool.GetOrAddComponentInChildren<Image>("Img_LevelBuff");
        buffIcon = UITool.GetOrAddComponentInChildren<Image>("Img_Buff");
        buffName = UITool.GetOrAddComponentInChildren<Text>("Txt_BuffName");

        UITool.addTriggersListener(EventTriggerType.PointerClick, PointerClick);
        UITool.addTriggersListener(EventTriggerType.PointerEnter, PointerEnter);
        UITool.addTriggersListener(EventTriggerType.PointerExit, PointerExit);

        //��ʼ����ʾ
        buffItem = (S_BuffKindAndLevel)para[0];
        itemData = (BuffItemData)para[1];

        if(para.Length==3)
            abandonBuffLevel=(int)para[2];

        buffName.text = itemData.buffName;
        //levelInfo.text = $"������ʾbuff{buffName.text}�ڵȼ�{buffItem.level+1}�ȼ�������";
        levelInfo.text = itemData.buffLevelDatas[buffItem.level].levelDescribe;
        levelIcon.sprite = itemData.buffLevelDatas[buffItem.level].levelSprite;
        buffIcon.sprite = itemData.buffSprite;

        light = UITool.FindChildGameObject("Img_Light");
        selectBack = UITool.FindChildGameObject("Img_BackSelceted");
        back = UITool.FindChildGameObject("Img_BackGround");

        light.SetActive(false);
        selectBack.SetActive(false);
        levelInfo.color = Color.white;
    }

    public override void PointerClick(BaseEventData data)
    {
        base.PointerEnter(data);

        GameManager.Instance.currentPlayer.characterBuffManager.BuffLevelTo(buffItem.buffKind, buffItem.level + 1,GameManager.Instance.currentPlayer.gameObject);

        #region Rebuild����
        //List<I_BuffBase> tempBuffList = new List<I_BuffBase>();
        //foreach (var item in Test1.Instance.playerC.characterBuffManager.characterKeepBuffList)
        //{
        //    if (item.GetBuffType() == buffItem.buffKind)
        //    {
        //        switch (buffItem.buffKind)
        //        {
        //            case E_BuffKind.Damage:
        //                tempBuffList.Add( new Damage(E_ChararcterType.player, buffItem.level+1));
        //                break;
        //            case E_BuffKind.HpUp:
        //                tempBuffList.Add(new HpUp(E_ChararcterType.player, buffItem.level+1));
        //                break;
        //            case E_BuffKind.SpeedBuff:
        //                tempBuffList.Add(new SpeedBuff(E_ChararcterType.player, buffItem.level+1));
        //                break;
        //                //TODO:ʣ���buff����
        //        }

        //    }
        //    else
        //    {
        //        tempBuffList.Add(item);
        //    }     
        //}
        //Test1.Instance.playerC.characterBuffManager.BuffReBuild(tempBuffList,Test1.Instance.player);
        #endregion


        GameManager.Instance.currentPlayer.characterBuffManager.RefreshData();
        BuffDataManager.Instance.RefreshBuff();//新增刷新Buff

        PanelManager.Instance.Close(PanelManager.Instance.GetPanel("SelectPanel").UIType);
        // Debug.Log("buff������");

        if (abandonBuffLevel - 2 > 0)
        {
            abandonBuffLevel--;
            PanelManager.Instance.Open(new SelectPanel(), null, "进阶", abandonBuffLevel);
        }
        //Time.timeScale = 1.0f;
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/ItemSelect");
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/levelUP");

    }

    //�����ͣ���Ҳ����չʾ
    public override void PointerEnter(BaseEventData data)
    {
        base.PointerEnter(data);

        SelectPanel selectPanel = (SelectPanel)PanelManager.Instance.GetPanel("SelectPanel");
        selectPanel.HighLight(buffItem.buffKind);

        light.SetActive(true);
        selectBack.SetActive(true);
        levelInfo.color = Color.black;

        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/buffItemSelect");
    }

    public override void PointerExit(BaseEventData data)
    {
        light.SetActive(false);
        selectBack.SetActive(false);

        levelInfo.color=Color.white;
    }
}
