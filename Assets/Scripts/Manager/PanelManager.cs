using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 面板管理器
/// xushi
/// </summary>
public class PanelManager : singleton<PanelManager>
{
    [Header("挂载位置")] public Transform battleUIParent;

    [Header("buff显示信息")] public BuffInfoListSO buffInfoListSO;
    [Header("当前关卡类型")] public E_BuffKind currentE_BuffKind;
    [Header("SkipButton绑定标识")]public bool isTip = false;
    //层级关系
    public enum Layer
    {
        Panel,
        Tip,
    }

    //按键委托
    public Action<KeyCode> KeyBoardUpdateAction;

    //层级列表
    private Dictionary<Layer, Transform> layers = new Dictionary<Layer, Transform>();
    //UI对象列表
    private Dictionary<UIType, BasePanel> dicUIO;
    //UI GameObject列表
    private Dictionary<UIType, GameObject> dicUIG ;
    //UI对象栈
    //private Stack<BasePanel> stackUI;
    //需要Update分发的UI
    private List<BasePanel> UpdateUIList;

    //结构
    public static Transform root;
    public static Transform canvas;

    Camera m_mainCam;
    public Camera MainCam
    {
        get
        {
            if (m_mainCam == null || m_mainCam.gameObject.activeInHierarchy == false)
            {
                m_mainCam = Camera.main;
            }
            return m_mainCam;
        }

    }

    public Camera UICamera;

    /// ////缓存分配区

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);

        //绑定ScrollToolManager脚本
        if (gameObject.GetComponent<ScrollToolManager>() == null)
            gameObject.AddComponent<ScrollToolManager>();

        Init();
    }

    private void Start()
    {
        Open(new LaunchPanel());   
    }

    private void Update() {
        foreach(var item in UpdateUIList)
        {
            item.Update();
        }
    }

    //初始化面板管理器
    public void Init() {
        //初始化UI储存
        dicUIO = new Dictionary<UIType, BasePanel>();
        dicUIG = new Dictionary<UIType, GameObject>();
        UpdateUIList = new List<BasePanel>();

        //stackUI = new Stack<BasePanel>();

        root = GameObject.Find("PanelManager").transform.Find("UIRoot").transform;
        canvas = root.Find("Canvas");
        UICamera = Camera.main;
        Transform panel = canvas.Find("Panel");
        Transform tip = canvas.Find("Tip");
        battleUIParent = canvas.transform.Find("BattleUI");
        layers.Clear();
        layers.Add(Layer.Panel, panel);
        layers.Add(Layer.Tip, tip);
        //InitEventListener();
        KeyBoardUpdateAction = null;

        buffInfoListSO = Resources.Load<BuffInfoListSO>("UI/UIData/Buff Info List SO");
    }

    /// <summary>
    /// 打开一个面板
    /// </summary>
    /// <param name="basePanel">面板对象</param>
    /// <param name="para">需要传递的参数列表</param>
    public void Open(BasePanel basePanel,Transform parent=null,params object[] para) {
        
        if(parent==null)
            parent = layers[basePanel.UIType._layer];
        
        //判断是否已经打开，如果已经打开则获取UI信息
        GameObject panel = GetSingleUI(basePanel,parent);

        basePanel.Init(new UITool(panel));

        //调用面板的初始化方法和进入方法
        basePanel.OnInit();
        basePanel.OnShow(para);

        //如果需要Update方法
        if(basePanel.ifNeedUpdate)
        {
            UpdateUIList.Add(basePanel);
        }
    }

    /// <summary>
    /// 关闭一个面板
    /// </summary>
    /// <param name="type">面板类型</param>
    public void Close(UIType type=null)
    {
        //if (type == null)
        //{
        //    if (stackUI.Count == 0)
        //        return;
        //    UIType typeT = stackUI.Pop().UIType;
        //    dicUIO[typeT].OnClose();
        //    Destroy(dicUIG[typeT]);
        //    dicUIG.Remove(typeT);
        //    dicUIO.Remove(typeT);
        //    return;
        //}
            if (dicUIG.ContainsKey(type))
            {
                if(dicUIO[type].ifNeedUpdate)
                {
                    UpdateUIList.Remove(dicUIO[type]);
                }
                //调用面板的关闭方法
                dicUIO[type].OnClose();

                //销毁面板物体，从各列表中移除
                Destroy(dicUIG[type]);
                dicUIG.Remove(type);
                dicUIO.Remove(type);
                //stackUI.Pop();
            }
    }

    /// <summary>
    /// 获取一个BasePanel对象
    /// </summary>
    /// <param name="name">面板名称</param>
    /// <returns></returns>
    public BasePanel GetPanel(string name)
    {
        UIType type = null;
        foreach (var item in dicUIO)
        {
            if (item.Key._name == name)
                type = item.Key;
        }
        if (type != null)
        {
            if (dicUIO.ContainsKey(type))
            {
                return dicUIO[type];
            }
        }

        Debug.Log($"未找到名为{name}的对象");
        return null;
    }

    /// <summary>
    /// 获取同名的所有BasePanel对象
    /// </summary>
    /// <param name="name">面板名称</param>
    /// <returns></returns>
    public List<BasePanel> GetAllPanel(string name)
    {
        List<BasePanel> basePanels=new List<BasePanel>();
        foreach (var item in dicUIO)
        {
            if (item.Key._name == name)
                basePanels.Add(item.Value);
        }
        //if (basePanels.Count==0)
            //Debug.Log($"未找到名为{name}的对象");
        return basePanels;
    }

    /// <summary>
    /// 获取一个UI物体，如果UI不存在就创建一个
    /// </summary>
    /// <param name="parent">UI的父物体</param>
    /// <param name="basePanel">UI对象</param>
    /// <returns></returns>
    public GameObject GetSingleUI(BasePanel basePanel, Transform parent = null)
    {
        if (dicUIG.ContainsKey(basePanel.UIType))
            return dicUIG[basePanel.UIType];
        if (!parent)
        {
            Debug.LogError("UI的父物体不存在");
            return null;
        }
        GameObject ui = Instantiate(Resources.Load<GameObject>(basePanel.UIType._path), parent);
        ui.name = basePanel.UIType._name;
        dicUIG.Add(basePanel.UIType, ui);
        dicUIO.Add(basePanel.UIType, basePanel);
        //stackUI.Push(basePanel);
        return ui;
    }

    void OnGUI()//有可能每帧不止调用一次
    {
        if (KeyBoardUpdateAction != null)
        {
            Event a = Event.current;
            if (Input.anyKeyDown)
            {
                if (a != null)
                {
                    KeyBoardUpdateAction(a.keyCode);
                }

            }
            if (Input.GetMouseButtonDown(0))
                KeyBoardUpdateAction(KeyCode.Mouse0);
            if (Input.GetMouseButtonDown(1))
                KeyBoardUpdateAction(KeyCode.Mouse1);
        }
    }

    public void CloseAllPanel()
    {
        Dictionary<UIType,BasePanel> allPanel = new Dictionary<UIType,BasePanel>(dicUIO);
        foreach(var panel in allPanel)
        {
            dicUIO.Remove(panel.Key);
            Destroy(dicUIG[panel.Key]);
            dicUIG.Remove(panel.Key);
        }
        dicUIG.Clear();
        dicUIG.Clear();
        UpdateUIList.Clear();
    }

    ///
    /// /////////////////一些特殊面板特殊处理
    /// 

    /// <summary>
    /// 创建对话面板
    /// </summary>
    /// <returns></returns>
    public GameObject CreateDialoguePanel()
    {
        // Transform battleUI = canvas.transform.Find("BattleUI");
        GameObject dialoguePanel = Instantiate(Resources.Load<GameObject>("UI/Panel/DialoguePanel"),canvas.Find("Tip"));
        if(dialoguePanel == null)
        {
            Debug.Log("创建失败");
        }
        return dialoguePanel;
    }

    /// <summary>
    /// 设置BossUI的可见性
    /// </summary>
    /// <param name="state">是否可见</param>
    public void SetBossUIVisble(bool state)
    {
        BattleMainPanel battleMainPanel = GetPanel("BattleMainPanel") as BattleMainPanel;
        if(battleMainPanel==null)
        {
            Debug.LogWarning("没有获取到主UI面板");
            return;
        }
        else
        {
            battleMainPanel.SetBossInfoAreaVisble(state);
        }
    }


    /// <summary>
    /// 设置玩家护盾的可见性
    /// </summary>
    public void SetPlayerShieldVisble(bool state)
    {
        BattleMainPanel battleMainPanel = GetPanel("BattleMainPanel") as BattleMainPanel;
        if(battleMainPanel==null)
        {
            Debug.LogWarning("没有获取到主UI面板");
            return;
        }
        else
        {
            battleMainPanel.SetPlayerShieldVisble(state);
        }
    }

///
/// //////////////////UI帮助函数部分
/// 
    //世界空间转UI空间
    public Vector3 WorldPointToUILocalPoint(Vector3 point)
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(point);

        Vector2 uiPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(),screenPoint,canvas.GetComponent<Canvas>().worldCamera,out uiPosition);
        // Debug.Log(uiPosition);
        return uiPosition;
    }

    /// <summary>
    /// 产生伤害数字
    /// </summary>
    /// <param name="damage">该次受到伤害</param>
    /// <param name="who">产生在谁身上</param>
    public void GenerateDamageNum(float damage,Transform who,bool isCritical = false,bool isPlayer = false)
    {
        GameObject nowText = ObjectPool.Instance.GetObject("DamageText",true,true);
        //现在使用的是TMP字体
        TMPDamageText tMPDamageText = nowText.GetComponent<TMPDamageText>();
        tMPDamageText.SetDamage(damage,isCritical);
        nowText.transform.localPosition = WorldPointToUILocalPoint(who.position);
        nowText.transform.position += UnityEngine.Random.insideUnitSphere * 2f;
        tMPDamageText.SetStorePos(nowText.transform.position);
        tMPDamageText.PlayerPreset(isPlayer);
    }

    /// <summary>
    /// 生成通用血量面板，一般针对机关，返回StateBar脚本
    /// </summary>
    /// <returns></returns>
    public StateBar GenerateCommonStatePanel(Transform who)
    {
        GameObject commonStatePanel = Instantiate(Resources.Load<GameObject>("UI/Panel/StatePanel"),battleUIParent);
        StateBar stateBar = new StateBar(commonStatePanel,who.gameObject);
        return stateBar;
    }

    /// <summary>
    /// 返回跟随目标的UI空间坐标
    /// </summary>
    /// <returns></returns>
    public Vector3 UIFollow(Transform who,float biasY = 0f)
    {
        Vector3 realPos = who.position + new Vector3(0f,biasY,0f);
        return WorldPointToUILocalPoint(realPos);
    }


////////////////////对外接口函数部分

    /// <summary>
    /// 返回当前角色剩余的血量
    /// </summary>
    /// <returns></returns>
    public float GetPlayerCurrentHP()
    {
        return GameManager.Instance.currentPlayer.characterData.currentHealth;
    }

    /// <summary>
    /// 返回当前boss剩余的血量
    /// </summary>
    /// <returns></returns>
    public float GetBossCurrentHp()
    {
        if (GameManager.Instance.currentBoss != null)
            return GameManager.Instance.currentBoss.bossData.currentHealth;
        return -1;
    }

    /// <summary>
    /// 返回当前玩家的最大生命值
    /// </summary>
    /// <returns></returns>
    public float GetPlayerMaxHp()
    {
        return GameManager.Instance.currentPlayer.characterData.maxHealth;
    }

    /// <summary>
    /// 返回当前boss的最大生命值
    /// </summary>
    /// <returns></returns>
    public float GetBossMaxHp()
    {
        return GameManager.Instance.currentBoss.bossData.maxHealth;
    }

    /// <summary>
    /// 返回当前玩家的当前护盾值，-1代表没有护盾
    /// </summary>
    /// <returns></returns>
    public float GetPlayerShieldHp()
    {
        if(GameManager.Instance.currentPlayer.characterBuffManager.FindBuff(E_BuffKind.ShieldBuff))
        {
            return GameManager.Instance.currentPlayer.characterBuffManager.shieldRipples.currentHealth;
        }
        else
        {
            return -1;
        }
    }

    /// <summary>
    /// 返回当前玩家的最大护盾值，-1代表没有护盾或者Boss不存在
    /// </summary>
    /// <returns></returns>
    public float GetPlayerShieldMaxHp()
    {
        if(GameManager.Instance.currentPlayer == null)
        {
            return -1;
        }
        if(GameManager.Instance.currentPlayer.characterBuffManager.FindBuff(E_BuffKind.ShieldBuff))
        {
            return GameManager.Instance.currentPlayer.characterBuffManager.shieldRipples.maxHealth;
        }
        else
        {
            return -1;
        }
    }
    /// <summary>
    /// 返回当前玩家的当前护盾值，-1代表没有护盾或者Boss不存在
    /// </summary>
    /// <returns></returns>
    public float GetBossShield()
    {
        if(GameManager.Instance.currentBoss == null)
        {
            return -1;
        }
        if (GameManager.Instance.currentBoss.characterBuffManager.FindBuff(E_BuffKind.ShieldBuff))
        {
            return GameManager.Instance.currentBoss.characterBuffManager.shieldRipples.currentHealth;
        }
        else
        {
            return -1;
        }
    }

    /// <summary>
    /// 返回当前boss的最大护盾值，-1代表没有护盾,或者Boss不存在
    /// </summary>
    /// <returns></returns>
    public float GetBossShieldMax()
    {
        if(GameManager.Instance.currentBoss == null)
        {
            return -1;
        }
        if (GameManager.Instance.currentBoss.characterBuffManager.FindBuff(E_BuffKind.ShieldBuff))
        {
            return GameManager.Instance.currentBoss.characterBuffManager.shieldRipples.maxHealth;
        }
        else
        {
            return -1;
        }
    }

    /// <summary>
    /// 获得连击次数,暂时无法使用
    /// </summary>
    /// <returns></returns>
    public float GetcomboTime()
    {
        return GameManager.Instance.CurrentComboCount;
    }

    /// <summary>
    /// 获得玩家当前子弹数,-1代表没有这个Buff或者玩家不存在
    /// </summary>
    /// <returns></returns>
    public float GetPlayerGunAmmo()
    {
        if(GameManager.Instance.currentPlayer == null)
        {
            return -1;
        }
        if(GameManager.Instance.currentPlayer.characterBuffManager.FindBuff(E_BuffKind.GunBuff))
        {
            return GameManager.Instance.currentPlayer.gunControl.curAmmunition;
        }
        else
        {
            return -1;
        }
    }

    /// <summary>
    /// 获得玩家当前最大子弹数，-1代表没有这个Buff或者玩家不存在
    /// </summary>
    /// <returns></returns>
    public float GetPlayerGunMaxAmmo()
    {
        if(GameManager.Instance.currentPlayer == null)
        {
            return -1;
        }
        if(GameManager.Instance.currentPlayer.characterBuffManager.FindBuff(E_BuffKind.GunBuff))
        {
            return GameManager.Instance.currentPlayer.gunControl.maxAmmunition;
        }
        else
        {
            return -1;
        }
    }

    /// <summary>
    /// 返回玩家当前蓄力比例，-1代表没有这个Buff或者玩家不存在
    /// </summary>
    /// <returns></returns>
    public float GetPlayerStaffPercent()
    {
        if(GameManager.Instance.currentPlayer == null)
        {
            return -1;
        }
        if(GameManager.Instance.currentPlayer.characterBuffManager.FindBuff(E_BuffKind.StaffBuff))
        {
            float percent = GameManager.Instance.currentPlayer.curHoldTime / GameManager.Instance.currentPlayer.staffHoldTime;
            return percent;
        }
        else
        {
            return -1;
        }
    }

    /// <summary>
    /// 设置主相机
    /// </summary>
    public void SetMainCamera(Camera _mainCamera)
    {
        canvas.GetComponent<Canvas>().worldCamera = _mainCamera;
        _mainCamera.nearClipPlane = 2;
    }


    /// <summary>
    /// 设置跳过Button
    /// </summary>
    /// <param name="state"></param>
    public void SetSkipButton(bool state)
    {
        canvas.Find("BattleUI").Find("SkipButton").gameObject.SetActive(state);
    }

}
