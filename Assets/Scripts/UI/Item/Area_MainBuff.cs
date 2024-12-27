using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �����������ťHUD
/// xushi
/// </summary>
public class Area_MainBuff : BasePanel
{
    static readonly string path = "UI/Item/Area_MainBuff";
    public Area_MainBuff() : base(new UIType(path)) { }

    //�����������
     GameObject attackGO;
    //ǹ�������Ҽ���
     GameObject gunGO;
    //�����
    Text maxBullet;
    //��ǰ����
    Text curBullet;
    //���л���
    Image bulletSlider;

    //�ȹ�����E��
     GameObject attackMagicGO;
    //��ǰ������
    Image magicSlider;

    //���ػ���Q��
     GameObject attackSwordGO;
    //��ǰ��������
    Text combo;

    //����
     GameObject healthGO;
    //�˺�
     GameObject damageGO;
    //�ٶ�
     GameObject speedGO;
    //����
     GameObject shieldGO;

    ////是否已经打开能力面板
    //public bool isOpenBuff=false;
    //public bool isOpenPause=false;
    ////是否刚刚关闭能力面板
    //public bool isBuffClosing=false;
    //public bool isOpenClosing = false;

    public override void OnInit()
    {
        base.OnInit();
        ifNeedUpdate = true;
    }
    public override void OnShow(params object[] para)
    {
        base.OnShow(para);
        attackGO = UITool.FindChildGameObject("MainBuff_Weapon");
        gunGO = UITool.FindChildGameObject("MainBuff_Weapon2");
        maxBullet = UITool.GetOrAddComponentInChildren<Text>("Txt_MaxBullet");
        curBullet = UITool.GetOrAddComponentInChildren<Text>("Txt_Bullet");
        bulletSlider = UITool.GetOrAddComponentInChildren<Image>("Img_Slider");
        attackMagicGO = UITool.FindChildGameObject("MainBuff_Weapon3");
        //magicSlider = UITool.GetOrAddComponentInChildren<Image>("Img_Slider");
        magicSlider = attackMagicGO.transform.Find("Img_SliderBack").GetChild(0).GetComponent<Image>();

        attackSwordGO = UITool.FindChildGameObject("MainBuff_Weapon4");
        combo = UITool.GetOrAddComponentInChildren<Text>("Txt_HitNum");

        shieldGO = UITool.FindChildGameObject("MainBuff_Property1");
        speedGO = UITool.FindChildGameObject("MainBuff_Property2");
        damageGO = UITool.FindChildGameObject("MainBuff_Property3");
        healthGO = UITool.FindChildGameObject("MainBuff_Property4");

        RefreshBuffButton();

        PanelManager.Instance.KeyBoardUpdateAction += OpenBuffInfo;
    }

    public override void OnClose()
    {
        base.OnClose();
        PanelManager.Instance.KeyBoardUpdateAction -= OpenBuffInfo;
    }

    /// <summary>
    /// 重新刷新界面按钮显示
    /// </summary>
    public void RefreshBuffButton()
    {
        attackSwordGO.SetActive(false);
        attackMagicGO.SetActive(false);
        gunGO.SetActive(false);
        healthGO.SetActive(false);
        damageGO.SetActive(false);
        speedGO.SetActive(false);
        shieldGO.SetActive(false);


        foreach (var item in BuffDataManager.Instance.playerCurrentBuff)
        {
            switch (item.buffKind)
            {
                case E_BuffKind.SwordBuff:
                    attackSwordGO.SetActive(true);
                    break;
                case E_BuffKind.StaffBuff:
                    attackMagicGO.SetActive(true);
                    break;
                case E_BuffKind.GunBuff:
                    gunGO.SetActive(true);
                    break;
                case E_BuffKind.HpUp:
                    healthGO.SetActive(true);
                    break;
                case E_BuffKind.Damage:
                    damageGO.SetActive(true);
                    break;
                case E_BuffKind.ShieldBuff:
                    shieldGO.SetActive(true);
                    break;
                case E_BuffKind.SpeedBuff:
                    speedGO.SetActive(true);
                    break;
            }
        }
    }

    //ˢ��״̬
    public override void Update()
    {
        base.Update();
        RefreshMagicSlider(PanelManager.Instance.GetPlayerStaffPercent());
        RefreshComboNum(PanelManager.Instance.GetcomboTime());
        RefreshCurBullet(PanelManager.Instance.GetPlayerGunAmmo(), PanelManager.Instance.GetPlayerGunMaxAmmo());

    }

    //�ȣ�ˢ����������
    public void RefreshMagicSlider(float f)
    {
        if(f!=-1)
            magicSlider.fillAmount = f;
        //Debug.Log(f);
    }
    //����ˢ����������
    public void RefreshComboNum(float num)
    {
        if(num!=-1)
            combo.text = num.ToString();    
    }
    //ǹ��ˢ�µ�ǰ�ӵ�������������
    public void RefreshCurBullet(float curNum, float maxNum)
    {
        if (curNum == -1 || maxNum == -1)
            return;
        curBullet.text = ((int)curNum).ToString();
        ChangeMaxBullet(maxNum);
        bulletSlider.fillAmount = curNum / maxNum;
    }
    //ǹ���ı������
    public void ChangeMaxBullet(float num)
    {
        if(num!=-1)
            maxBullet.text = ((int)num).ToString();
    }

    //打开能力面板
    public void OpenBuffInfo(KeyCode keyCode)
    {
        
        if (keyCode == KeyCode.Escape)
        {
            PanelManager.Instance.Open(new PausePanel());
            //打开面板则停止当前的监听
            PanelManager.Instance.KeyBoardUpdateAction -= OpenBuffInfo;
        }
        
        if (keyCode == KeyCode.B)
        {
            PanelManager.Instance.Open(new BuffInfoPanel());

            //打开面板则停止当前的监听
            PanelManager.Instance.KeyBoardUpdateAction -= OpenBuffInfo;
        }
    }
   
}
