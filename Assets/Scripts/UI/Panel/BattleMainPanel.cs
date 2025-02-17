using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ����壬����ս������ʹ��
/// xushi
/// </summary>
public class BattleMainPanel : BasePanel
{
    static readonly string path = "UI/Panel/BattleMainPanel";
    public BattleMainPanel() : base(new UIType(path)) { }
    //移动端操作面板
    GameObject androidControl;

    //�������ص�UI��
    GameObject fadeLayer;
    //�ٶȵȼ�
    Text speed;
    //�˺��ȼ�
    Text damage;
    //�����ȼ�
    Text health;
    //����ֵ������
    Slider healthSlider;
    //��ǰ����ֵ
    Text curHp;
    //�������ֵ
    Text maxHp;
    //����ֵ
    GameObject shieldGO;
    //����ֵ������
    Slider shieldSlider;
    //��ǰ����ֵ
    Text curSh;
    //��󻤶�ֵ
    Text maxSh;

    //Boss���
    GameObject bossInfoGO;
    //Boss��Ѫ��
    Image bossHp;
    //Boss����Ѫ��
    Image bossHpBuffer;
    //Boss����
    GameObject bossShieldGo;
    //Boss����
    Image bossShield;
    //Boss当前血条数量
    Text bossHpNumberText;
    //Boss当前血量
    //float bossCurSliderHp;

    //��ͣ��ť
    Button pauseButton;
    Button buffButton;

    public override void OnInit()
    {
        base.OnInit();
        //PanelManager.Instance.Open(new Weapen(),UITool.GetUI().transform);
        //PanelManager.Instance.Open(new GM());
        PanelManager.Instance.Open(new Area_MainBuff());

        //test
        //PanelManager.Instance.Open(new TipsPanel(), null, Resources.Load<TipsPanelSO>("UI/StaticDataSO/New Tips Panel SO"));
        ifNeedUpdate = true;
    }
    public override void Update()
    {
        base.Update();
        RefreshHp(PanelManager.Instance.GetPlayerCurrentHP(), PanelManager.Instance.GetPlayerMaxHp());
        RefreshShield(PanelManager.Instance.GetPlayerShieldHp(), PanelManager.Instance.GetPlayerShieldMaxHp());
        if (GameManager.Instance.currentBoss != null)
        {
            RefreshBossHp(PanelManager.Instance.GetBossCurrentHp(), PanelManager.Instance.GetBossMaxHp(), GameManager.Instance.currentBoss.stage);
            RefreshBossShield(PanelManager.Instance.GetBossShield(), PanelManager.Instance.GetBossShieldMax());
        }
    }
    public override void OnClose()
    {
        base.OnClose();
        PanelManager.Instance.Close(PanelManager.Instance.GetPanel("Area_MainBuff").UIType);
    }

    
    public override void OnShow(params object[] para)
    {
        base.OnShow(para);
        //�����
        androidControl = UITool.FindChildGameObject("AndroidControl");
        fadeLayer = UITool.FindChildGameObject("FadeLayer");
        //speed = UITool.FindChildGameObject("Img_speed").GetComponentInChildren<Text>();
        //damage = UITool.FindChildGameObject("Img_Damage").GetComponentInChildren<Text>();
        //health = UITool.FindChildGameObject("Img_Health").GetComponentInChildren<Text>();
        healthSlider = UITool.GetOrAddComponentInChildren<Slider>("Slider_Hp");
        curHp = UITool.GetOrAddComponentInChildren<Text>("Txt_curHp");
        maxHp = UITool.GetOrAddComponentInChildren<Text>("Txt_maxHp");
        pauseButton = UITool.GetOrAddComponentInChildren<Button>("Btn_Pause");
        buffButton = UITool.GetOrAddComponentInChildren<Button>("Btn_Buff");

        shieldGO = UITool.FindChildGameObject("Slider_Shield");
        shieldSlider = UITool.GetOrAddComponentInChildren<Slider>("Slider_Shield");
        maxSh = UITool.GetOrAddComponentInChildren<Text>("Txt_maxSh");
        curSh = UITool.GetOrAddComponentInChildren<Text>("Txt_curSh");

        bossInfoGO = UITool.FindChildGameObject("BossArea");
        bossHp = UITool.GetOrAddComponentInChildren<Image>("Img_Hp");
        bossHpBuffer = UITool.GetOrAddComponentInChildren<Image>("Img_HpBuffer");
        bossShieldGo = UITool.FindChildGameObject("Slider_BossShield");
        bossShield = UITool.GetOrAddComponentInChildren<Image>("Img_bossHp");
        bossHpNumberText = UITool.GetOrAddComponentInChildren<Text>("Txt_curHpNum");
        //���Ӽ���
        pauseButton.onClick.AddListener(Pause);
        buffButton.onClick.AddListener(() => PanelManager.Instance.Open(new BuffInfoPanel()));

        if (PanelManager.Instance.GetPlayerShieldHp() == -1)
            shieldGO.SetActive(false);

        //��ʼ��

        //bossShieldGo.SetActive(false);
        bossHpNumberText.text = "5";
        //BOSS信息栏
        SetBossInfoAreaVisble(false);

        FadeChange(true);

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        androidControl.SetActive(false);
#elif UNITY_ANDROID
        androidControl.SetActive(true);
#endif
    }

    /// <summary>
    /// 播放顶部UI淡入淡出动画
    /// </summary>
    /// <param name="intoRoom">进入房间=true隐藏，离开房间=false出现</param>
    public void FadeChange(bool intoRoom)
    {
        if (intoRoom)
            UITool.GetOrAddComponent<Animator>().SetTrigger("FadeOut");
        else
            UITool.GetOrAddComponent<Animator>().SetTrigger("FadeIn");
    }

    //bossѪ������
    public void RefreshBossHp(float bossCurHp, float bossMaxHp, int curStage)
    {
        if (bossCurHp == -1 || bossMaxHp == -1)
            return;

        //boss的每管血量
        float itemHp = bossMaxHp / 5;
        //显示boss当前的血条数
        int bossCurHpSlider = 5 - curStage;
        bossHpNumberText.text = bossCurHpSlider.ToString();


        //显示boss当前血条比例
        bossHp.fillAmount = (bossCurHp - itemHp * (bossCurHpSlider - 1)) / itemHp;

        if (bossHpBuffer.fillAmount >= bossHp.fillAmount)
        {
            bossHpBuffer.fillAmount -= 0.003f;
        }
        else
        {
            bossHpBuffer.fillAmount = bossHp.fillAmount;
        }
    }

    //Boss���ܸ���
    public void RefreshBossShield(float bossCurSh, float bossMaxSh)
    {
        if (bossCurSh == -1 || bossMaxSh == -1)
        {
            bossShield.fillAmount = 0;
            return;
        }

        bossShield.fillAmount = bossCurSh / bossMaxSh;

    }

    //��ɫ����
    //Ѫ������
    public void RefreshHp(float curHp, float maxHp)
    {
        if (curHp == -1 || maxHp == -1)
            return;
        this.curHp.text = ((int)curHp).ToString();
        this.maxHp.text = ((int)maxHp).ToString();
        healthSlider.value = (float)curHp / maxHp;
    }
    //���ܸ���
    public void RefreshShield(float curShield, float maxShield)
    {
        if (curShield == -1 || maxShield == -1)
            return;
        curSh.text = ((int)curShield).ToString();
        maxSh.text = ((int)maxShield).ToString();
        shieldSlider.value = (float)curShield / maxShield;
    }

    /// <summary>
    /// 设置Boss血条和护盾条的可见性
    /// </summary>
    /// <param name="state"></param>
    public void SetBossInfoAreaVisble(bool state)
    {
        bossInfoGO.SetActive(state);
        //判断一下Boss有没有护盾
        if(GameManager.Instance.currentBoss == null)
        {
            Debug.LogWarning("当前没有Boss");
            return;
        }
        if(!GameManager.Instance.currentBoss.GetComponent<BossControl>().characterBuffManager.FindBuff(E_BuffKind.ShieldBuff))
        {
            bossShieldGo.SetActive(false);
        }
        else
        {
            bossShieldGo.SetActive(true);
        }
    }

    /// <summary>
    /// 设置玩家护盾的可见性
    /// </summary>
    public void SetPlayerShieldVisble(bool state)
    {
        shieldGO.SetActive(state);
    }


    //��ͣ��Ϸ������ͣ�˵�
    public void Pause()
    {
        PanelManager.Instance.Open(new PausePanel());
    }


}
