using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// パネルマネージャー
/// xushi
/// </summary>
public class PanelManager : singleton<PanelManager>
{
    [Header("マウント位置")] public Transform battleUIParent;

    [Header("バフ表示情報")] public BuffInfoListSO buffInfoListSO;
    [Header("現在のステージタイプ")] public E_BuffKind currentE_BuffKind;
    [Header("SkipButtonバインド識別子")]public bool isTip = false;
    //階層関係
    public enum Layer
    {
        Panel,
        Tip,
    }

    //キー委託
    public Action<KeyCode> KeyBoardUpdateAction;

    //階層リスト
    private Dictionary<Layer, Transform> layers = new Dictionary<Layer, Transform>();
    //UIオブジェクトリスト
    private Dictionary<UIType, BasePanel> dicUIO;
    //UIゲームオブジェクトリスト
    private Dictionary<UIType, GameObject> dicUIG ;
    //UIオブジェクトスタック
    //private Stack<BasePanel> stackUI;
    //UIの更新が必要です
    private List<BasePanel> UpdateUIList;

    //構造
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

    /// キャッシュ割り当て区域

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);

        //ScrollToolManagerスクリプトをバインドする
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

    //パネルマネージャーの初期化
    public void Init() {
        //UIストレージの初期化
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
    /// パネルを開く
    /// </summary>
    /// <param name="basePanel">パネルオブジェクト</param>
    /// <param name="para">渡す必要があるパラメータのリスト</param>
    public void Open(BasePanel basePanel,Transform parent=null,params object[] para) {
        
        if(parent==null)
            parent = layers[basePanel.UIType._layer];
        
        //開いているかどうかを判断し、開いていたらUI情報を取得します。
        GameObject panel = GetSingleUI(basePanel,parent);

        basePanel.Init(new UITool(panel));

        //パネルの初期化メソッドとエンターメソッドを呼び出す
        basePanel.OnInit();
        basePanel.OnShow(para);

        //Updateメソッドが必要な場合
        if(basePanel.ifNeedUpdate)
        {
            UpdateUIList.Add(basePanel);
        }
    }

    /// <summary>
    /// パネルを閉じる
    /// </summary>
    /// <param name="type">パネルタイプ</param>
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
                //パネルのクローズメソッドを呼び出す
                dicUIO[type].OnClose();

                //パネルオブジェクトを破壊し、各リストから削除します。
                Destroy(dicUIG[type]);
                dicUIG.Remove(type);
                dicUIO.Remove(type);
                //stackUI.Pop();
            }
    }

    /// <summary>
    /// BasePanelオブジェクトを取得する
    /// </summary>
    /// <param name="name">パネル名</param>
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

        Debug.Log($"名前が{name}のオブジェクトが見つかりませんでした");
        return null;
    }

    ///<summary>


    ////// 同名のすべてのBasePanelオブジェクトを取得します


    ///</summary>
    /// <param name="name">パネル名</param>
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
            //Debug.Log($"{name}という名前のオブジェクトが見つかりませんでした");
        return basePanels;
    }

    /// <summary>
    /// UIオブジェクトを取得し、存在しない場合は新たに作成します。
    /// </summary>
    /// <param name="parent">UIの親オブジェクト</param>
    /// <param name="basePanel">UIオブジェクト</param>
    /// <returns></returns>
    public GameObject GetSingleUI(BasePanel basePanel, Transform parent = null)
    {
        if (dicUIG.ContainsKey(basePanel.UIType))
            return dicUIG[basePanel.UIType];
        if (!parent)
        {
            Debug.LogError("UIの親オブジェクトが存在しません");
            return null;
        }
        GameObject ui = Instantiate(Resources.Load<GameObject>(basePanel.UIType._path), parent);
        ui.name = basePanel.UIType._name;
        dicUIG.Add(basePanel.UIType, ui);
        dicUIO.Add(basePanel.UIType, basePanel);
        //stackUI.Push(basePanel);
        return ui;
    }

    void OnGUI()//それぞれのフレームが一度以上呼び出される可能性があります。
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
    //一部の特別なパネルは特別に処理されます
    /// 

    /// <summary>
    /// ダイアログパネルを作成する
    /// </summary>
    /// <returns></returns>
    public GameObject CreateDialoguePanel()
    {
        // Transform battleUI = canvas.transform.Find("BattleUI");
        GameObject dialoguePanel = Instantiate(Resources.Load<GameObject>("UI/Panel/DialoguePanel"),canvas.Find("Tip"));
        if(dialoguePanel == null)
        {
            Debug.Log("作成に失敗しました");
        }
        return dialoguePanel;
    }

    /// <summary>
    /// BossUIの可視性を設定する
    /// </summary>
    /// <param name="state">表示可能かどうか</param>
    public void SetBossUIVisble(bool state)
    {
        BattleMainPanel battleMainPanel = GetPanel("BattleMainPanel") as BattleMainPanel;
        if(battleMainPanel==null)
        {
            Debug.LogWarning("メインUIパネルが取得できませんでした");
            return;
        }
        else
        {
            battleMainPanel.SetBossInfoAreaVisble(state);
        }
    }


    /// <summary>
    /// プレイヤーのシールドの可視性を設定する
    /// </summary>
    public void SetPlayerShieldVisble(bool state)
    {
        BattleMainPanel battleMainPanel = GetPanel("BattleMainPanel") as BattleMainPanel;
        if(battleMainPanel==null)
        {
            Debug.LogWarning("メインUIパネルが取得できませんでした");
            return;
        }
        else
        {
            battleMainPanel.SetPlayerShieldVisble(state);
        }
    }

///
/// //////////////////UIヘルプ関数部分
/// 
    //ワールドスペースからUIスペースへの変換
    public Vector3 WorldPointToUILocalPoint(Vector3 point)
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(point);

        Vector2 uiPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(),screenPoint,canvas.GetComponent<Canvas>().worldCamera,out uiPosition);
        // Debug.Log(uiPosition);
        return uiPosition;
    }

    /// <summary>
    /// ダメージ数値を生成する
    /// </summary>
    /// <param name="damage">このダメージを受けた</param>
    /// <param name="who">誰の上で発生する</param>
    public void GenerateDamageNum(float damage,Transform who,bool isCritical = false,bool isPlayer = false)
    {
        GameObject nowText = ObjectPool.Instance.GetObject("DamageText",true,true);
        //現在使用しているのはTMPフォントです。
        TMPDamageText tMPDamageText = nowText.GetComponent<TMPDamageText>();
        tMPDamageText.SetDamage(damage,isCritical);
        nowText.transform.localPosition = WorldPointToUILocalPoint(who.position);
        nowText.transform.position += UnityEngine.Random.insideUnitSphere * 2f;
        tMPDamageText.SetStorePos(nowText.transform.position);
        tMPDamageText.PlayerPreset(isPlayer);
    }

    ///<summary>


    ////// 汎用的なヘルスバーを生成し、一般的にはメカニズムに対して使用され、StateBarスクリプトを返します。


    ///</summary>
    /// <returns></returns>
    public StateBar GenerateCommonStatePanel(Transform who)
    {
        GameObject commonStatePanel = Instantiate(Resources.Load<GameObject>("UI/Panel/StatePanel"),battleUIParent);
        StateBar stateBar = new StateBar(commonStatePanel,who.gameObject);
        return stateBar;
    }

    /// <summary>
    /// ターゲットに追従するUIスペース座標を返します。
    /// </summary>
    /// <returns></returns>
    public Vector3 UIFollow(Transform who,float biasY = 0f)
    {
        Vector3 realPos = who.position + new Vector3(0f,biasY,0f);
        return WorldPointToUILocalPoint(realPos);
    }


////////////////////外部インターフェース関数の一部

    ///<summary>


    ////// 現在のキャラクターの残りの体力を返します


    ///</summary>
    /// <returns></returns>
    public float GetPlayerCurrentHP()
    {
        return GameManager.Instance.currentPlayer.characterData.currentHealth;
    }

    ///<summary>


    ////// 現在のボスの残りの体力を返します


    ///</summary>
    /// <returns></returns>
    public float GetBossCurrentHp()
    {
        if (GameManager.Instance.currentBoss != null)
            return GameManager.Instance.currentBoss.bossData.currentHealth;
        return -1;
    }

    ///<summary>


    ////// 現在のプレイヤーの最大体力を返します


    ///</summary>
    /// <returns></returns>
    public float GetPlayerMaxHp()
    {
        return GameManager.Instance.currentPlayer.characterData.maxHealth;
    }

    ///<summary>


    ////// 現在のボスの最大体力を返します


    ///</summary>
    /// <returns></returns>
    public float GetBossMaxHp()
    {
        return GameManager.Instance.currentBoss.bossData.maxHealth;
    }

    ///<summary>


    ////// 現在のプレイヤーの現在のシールド値を返します。-1はシールドがないことを示します。


    ///</summary>
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

    ///<summary>


    ////// 現在のプレイヤーの最大シールド値を返します。-1はシールドが存在しない、またはボスが存在しないことを示します。


    ///</summary>
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
    ///<summary>

    ////// 現在のプレイヤーの現在のシールド値を返します。-1はシールドがない、またはボスが存在しないことを示します。

    ///</summary>
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

    ///<summary>


    ////// 現在のボスの最大シールド値を返します。-1はシールドがない、またはボスが存在しないことを示します。


    ///</summary>
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
    /// コンボ数を獲得しましたが、一時的に使用できません。
    /// </summary>
    /// <returns></returns>
    public float GetcomboTime()
    {
        return GameManager.Instance.CurrentComboCount;
    }

    /// <summary>
    /// プレイヤーの現在の弾数を取得します。-1は、このバフが存在しないか、プレイヤーが存在しないことを示します。
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
    /// プレイヤーの現在の最大弾数を取得します。-1は、このバフが存在しないか、プレイヤーが存在しないことを示します。
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
    /// プレイヤーの現在の蓄積比率を返します、-1はこのBuffが存在しないか、プレイヤーが存在しないことを示します。
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
    /// メインカメラを設定する
    /// </summary>
    public void SetMainCamera(Camera _mainCamera)
    {
        canvas.GetComponent<Canvas>().worldCamera = _mainCamera;
        _mainCamera.nearClipPlane = 2;
    }


    /// <summary>
    /// スキップボタンを設定する
    /// </summary>
    /// <param name="state"></param>
    public void SetSkipButton(bool state)
    {
        canvas.Find("BattleUI").Find("SkipButton").gameObject.SetActive(state);
    }

}
