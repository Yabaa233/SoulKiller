using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

[System.Serializable]
public class BossCD
{
    [Header("通常攻撃CD")]
    public CDClass canNormalAttack = new CDClass();
    [Header("スキル攻撃のクールダウン")]
    public CDClass canSkillAttack = new CDClass();
    [Header("銃スキル1攻撃CD")]
    public CDClass canGunAttack1 = new CDClass();
    [Header("銃スキル2攻撃CD")]
    public CDClass canGunAttack2 = new CDClass();
    [Header("杖スキル攻撃のCD")]
    public CDClass canStaffAttack = new CDClass();
    [Header("召喚スキル攻撃のCD")]
    public CDClass canSummonAttack = new CDClass();
    [Header("ステップCD")]
    public CDClass canDodge = new CDClass();
    [Header("フラッシュCD")]
    public CDClass canFlash = new CDClass();
    [Header("リバースフラッシュCD")]
    public CDClass canBackFlash = new CDClass();
    [Header("ヘルスリジェネレーションCD")]
    public CDClass canRestoreHealth = new CDClass();
    [Header("バックシールドCD")]
    public CDClass canRestoreShield = new CDClass();
    [Header("狂暴CD")]
    public CDClass canRage = new CDClass();

    public void InitCD()
    {
        GameManager.Instance.CDList.Add(canNormalAttack);   //通常攻撃
        canNormalAttack.curTime = 0;
        canNormalAttack.flag = false;

        GameManager.Instance.CDList.Add(canSkillAttack);    //近接連続攻撃
        canSkillAttack.curTime = 0;
        canSkillAttack.flag = false;

        GameManager.Instance.CDList.Add(canGunAttack1);     //射撃1
        canGunAttack1.curTime = 0;
        canGunAttack1.flag = false;

        GameManager.Instance.CDList.Add(canGunAttack2);     //シューティング2
        canGunAttack2.curTime = 0;
        canGunAttack2.flag = false;

        GameManager.Instance.CDList.Add(canStaffAttack);    //ステッキ
        canStaffAttack.curTime = 0;
        canStaffAttack.flag = false;

        GameManager.Instance.CDList.Add(canSummonAttack);   //召喚
        canSummonAttack.curTime = 0;
        canSummonAttack.flag = false;

        GameManager.Instance.CDList.Add(canDodge);          //ステップ
        canDodge.curTime = 0;
        canDodge.flag = false;

        GameManager.Instance.CDList.Add(canFlash);          //フラッシュ
        canFlash.curTime = 0;
        canFlash.flag = false;

        GameManager.Instance.CDList.Add(canBackFlash);      //リバースフラッシュ
        canBackFlash.curTime = 0;
        canBackFlash.flag = false;

        GameManager.Instance.CDList.Add(canRestoreHealth);  //回復
        canRestoreHealth.curTime = 0;
        canRestoreHealth.flag = false;

        GameManager.Instance.CDList.Add(canRestoreShield);  //バックシールド
        canRestoreShield.curTime = 0;
        canRestoreShield.flag = false;

        GameManager.Instance.CDList.Add(canRage);  //狂暴
        canRage.curTime = 0;
        canRage.flag = false;
    }
    public void ClearnCD()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.CDList.Remove(canNormalAttack);   //通常攻撃

        GameManager.Instance.CDList.Remove(canSkillAttack);    //近接連続攻撃

        GameManager.Instance.CDList.Remove(canGunAttack1);     //射撃1

        GameManager.Instance.CDList.Remove(canGunAttack2);     //シューティング2

        GameManager.Instance.CDList.Remove(canStaffAttack);    //ステッキ

        GameManager.Instance.CDList.Remove(canSummonAttack);   //召喚

        GameManager.Instance.CDList.Remove(canDodge);          //ステップ

        GameManager.Instance.CDList.Remove(canFlash);          //フラッシュ

        GameManager.Instance.CDList.Remove(canBackFlash);      //リバースフラッシュ

        GameManager.Instance.CDList.Remove(canRestoreHealth);  //回復

        GameManager.Instance.CDList.Remove(canRestoreShield);  //バックシールド

        GameManager.Instance.CDList.Remove(canRage);  //狂暴
    }
}

public enum E_BossAttackMode
{
    normal,
    swordSkill,
    gun01,
    gun02,
    staff,
    init
}

public class BossControl : MonoBehaviour
{
    [Header("BossCD")] public BossCD bossCD;
    [Header("ボスのバフマネージャー")] public CharacterBuffManager characterBuffManager = new CharacterBuffManager();
    [Header("ボス属性テンプレート")] public CharacterData_SO tempCharaterData;
    [Header("ボス属性")] public CharacterData bossData;
    [Header("ボスの基本移動速度")] public float baseSpeed;
    [Header("ボスが被弾した回数")] public float bossHurtCount;
    [Header("ボスが一回の衝撃で揺れる時間")] public float bossHurtTime;
    [Header("ボスが受けるダメージと反比例する係数")] public float bossHurtPer;
    [Header("被打撃時の振動とダメージの比率曲線")] public AnimationCurve hurtEffCurve;

    [Header("ボス攻撃モード")] public E_BossAttackMode bossAttackMode = E_BossAttackMode.init;
    [Header("ボス浮遊砲")] public AutoGunControl autoGunControl;
    [Header("ボスの体型モデル")] public Transform bossBody;
    [Header("現在のComboNode")] public ComboNode comboNode;
    [Header("目標プレイヤー")] public GameObject targetPlayer;
    [Header("ボスのバフ状況")] public int swordBuffLevel, gunBuffLevel, staffBuffLevel;
    [Header("ボスの素材")] public Material mtr;
    [Header("フィールドサイズ 逆転フラッシュの距離を制御するために使用されます")] public float roomSize;

    [Header("様々な状態")]
    [Tooltip("死亡しましたか？")] public bool isDead;
    [Tooltip("打撃を受けられますか？")] public bool canGetHit;
    [Tooltip("邪魔してもいいですか？")] public bool canInter;
    [Tooltip("中断されましたか？")] public bool interTrigger;
    [Tooltip("狂暴な状態にありますか？")] public bool isRageing;
    [Tooltip("打たれていますか？")] public bool isHurting;
    [Tooltip("血液をロックしましたか？")] public bool lockHealth = false;
    [Tooltip("現在の段階")] public int stage;

    [Header("設定の内容")]
    [Tooltip("フィールド上に召喚物が存在していますか？")] public bool hasFlower;
    [Tooltip("フィールド上の召喚物")] public GameObject bossFlower;
    [Tooltip("攻撃特効の出現位置補正")] public Vector3 effectAtkPos = new Vector3(0.0f, 3.0f, 1.0f);
    [Tooltip("HP回復スキルの毎秒のHP回復量")] public float HealthReplyVolume = 10;
    [Tooltip("シールド回復スキルは、毎秒シールドを回復します。")] public float ShieldReplyVolume = 5;
    [Tooltip("狂暴継続時間")] public float rageTime;
    [Tooltip("レーザーフラワーのプレファブ")] public GameObject bossFlowerPrefab;
    private Transform bossEffectParent; //ボスの詠唱、フェーズ変更、トレイルエフェクトのマウントノード
    private GameObject bossDashTrailEff;   //ボスのダッシュ残像エフェクト
    private GameObject bossSingingEff;      //ボスの詠唱特効
    private GameObject bossStageChangeEff; //ボスのフェーズ変更特殊効果
    private GameObject bossAngryStateEff;  //ボスの狂暴状態が持続する特殊効果
    private GameObject bossWeakStateEff;    //ボスの弱体化エフェクト
    private GameObject bossDeadStateEff;    //ボスの死亡エフェクト
    private float curRageTime; //既に暴走時間が経過しています
    private Transform roomCenter;   //シーンセンター
    private Transform flowerTransform;   //花の誕生地
    private BehaviorTree behaviorTree;  //行動ツリープラグイン
    public BehaviorTree BehaviorTree { get { return behaviorTree; } }   //外部から取得するために使用
    private Animator animator;  //アニメーションステートマシン
    private Rigidbody rb;   //剛体
    private Collider weaponTrigger; //武器の衝突体
    private Transform attackRangeHint; //攻撃範囲のヒント
    private Transform orientationObject; //足底が光の輪に向かっています
    //一時変数、重複作成を防ぐ
    private Vector3 dir;    //ボスの移動方向
    private Vector3 attackDir;  //ボスの今回の攻撃方向
    private float NextStageHealth;  //次の段階のヒットポイント値
    private int canUseBuffCount = 2;    //使用可能なバフの数は、指定されたHP量で増加します。

    private void Awake()
    {
        behaviorTree = GetComponent<BehaviorTree>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        weaponTrigger = transform.Find("Weapon").GetComponent<Collider>();
        attackRangeHint = transform.Find("AttackRangeHint");
        orientationObject = transform.Find("OrientationObject");
        autoGunControl = transform.Find("GunParent").GetComponent<AutoGunControl>();
        autoGunControl.bossControl = this;
        bossBody = transform.Find("PlayerCharacter");
        mtr.SetColor("_Color0", Color.white);
        roomCenter = transform.parent.parent.Find("RoomCenter");
        flowerTransform = transform.parent.parent.Find("FlowerTransform");
        bossData = new CharacterData(Instantiate(tempCharaterData));

        bossEffectParent = transform.Find("BossEffect");
        bossSingingEff = bossEffectParent.Find("BossSinging").gameObject;
        bossDashTrailEff = bossEffectParent.Find("BossDashTrail").gameObject;
        bossStageChangeEff = bossEffectParent.Find("BossStateChange").gameObject;
        bossAngryStateEff = bossEffectParent.Find("BossAngryState").gameObject;
        bossWeakStateEff = bossEffectParent.Find("BossWeakState").gameObject;
        bossDeadStateEff = bossEffectParent.Find("BossDeadState").gameObject;
    }

    void Start()
    {
        //登録
        GameManager.Instance.currentBoss = this;
        GameManager.Instance.BossDie += BossState_Die;
        //Buffを初期化する
        characterBuffManager.Init(E_ChararcterType.boss);
        // characterBuffManager.AddBuff(new ShieldBuff(E_ChararcterType.boss, 4), this.gameObject);
        // characterBuffManager.AddBuff(new HpUp(E_ChararcterType.boss, 4), this.gameObject);
        //フロート砲の初期化
        autoGunControl.AutoGunInit();
        //武器タイプの初期化
        ChangeWeaponType(E_BossAttackMode.normal);
        //コライダーを閉じる
        weaponTrigger.enabled = false;
        //いくつかの数値を初期化します
        BossValueInit();
        //CDを登録する
        bossCD.InitCD();
        //行動ツリーの初期化
        BehaviorTreeInit();
        //BossInfoのUI表示を通知します
        PanelManager.Instance.SetBossUIVisble(true);
    }

    private void Update()
    {
        if (!isDead)
        {
            OrientationObjectLookAt();
            autoGunControl.ModeLookAt(targetPlayer.transform.position, bossAttackMode);
            if (isRageing)
            {
                curRageTime += Time.deltaTime;
                if (curRageTime > rageTime)
                {
                    Rage_End();
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.BossDie -= BossState_Die;
        }
        bossCD.ClearnCD();
        bossCD = null;
        if (bossFlower != null)
        {
            Destroy(bossFlower);
        }
        PanelManager.Instance.SetBossUIVisble(false);//UIを閉じる通知
    }

    /// <summary>
    /// いくつかの数値を初期化します
    /// </summary>
    public void BossValueInit()
    {
        canGetHit = true;   //打撃を受けることを許可する
        canInter = false;   //邪魔しないでください
        interTrigger = false;   //邪魔されないで
        isRageing = false;  //暴走していない
        hasFlower = false;  //花がない
        NextStageHealth = bossData.maxHealth * 0.8f;
        mtr.SetFloat("_dissolve", 0);
    }

    ///<summary>


    ////// アクションツリーの初期化


    ///</summary>
    public void BehaviorTreeInit()
    {
        targetPlayer = GameManager.Instance.currentPlayer.gameObject;
        behaviorTree.SetVariableValue("bodyTransform", bossBody);
        behaviorTree.SetVariableValue("player", targetPlayer);
        behaviorTree.SetVariableValue("thisBoss", gameObject);
        behaviorTree.EnableBehavior();
    }

    #region Animator相关控制
    /// <summary>
    /// アニメーションステートマシンのBool変数を設定する
    /// </summary>
    /// <param name="name"> 名前 </param>
    /// <param name="value"> 値 </param>
    public void SetAnimatorBool(string name, bool value)
    {
        animator.SetBool(name, value);
    }

    /// <summary>
    /// アニメーションステートマシン内のbool変数の値を取得する
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool GetAnimatorBool(string name)
    {
        return animator.GetBool(name);
    }

    /// <summary>
    /// アニメーションステートマシンのTrigger変数を設定する
    /// </summary>
    /// <param name="name"> 名前 </param>
    public void SetAnimatorTrigger(string name)
    {
        animator.SetTrigger(name);
    }

    /// <summary>
    /// トリガー変数をアニメーションステートマシンでリセットします
    /// </summary>
    /// <param name="name"> 名前 </param>
    public void ResetAnimatorTrigger(string name)
    {
        animator.ResetTrigger(name);
    }

    /// <summary>
    /// アニメーションステートマシンのInt変数を設定する
    /// </summary>
    /// <param name="name"> 名前 </param>
    /// <param name="value"> 値 </param>
    public void SetAnimatorInt(string name, int value)
    {
        animator.SetInteger(name, value);
    }

    public AnimatorStateInfo GetAnimatorStateInfo()
    {
        return animator.GetCurrentAnimatorStateInfo(0);
    }
    #endregion

    #region 控制相关
    /// <summary>
    /// Playerに移動します
    /// </summary>
    /// <param name="stoppingDistance"> 停止距離 </param>
    /// <param name="moveSpeed"> 移動速度 </param>
    /// <returns>プレイヤーに近づいたかどうか</returns>
    public bool MoveToPlayer(float stoppingDistance, float moveSpeed)
    {
        dir = targetPlayer.transform.position - transform.position;
        if (dir.magnitude > stoppingDistance)
        {
            dir.y = rb.velocity.y;
            rb.velocity = dir * moveSpeed * baseSpeed;
            RotateToTarget(Vector3.Dot(transform.right, dir) > 0 ? -1 : 1);
            animator.SetFloat("speed", rb.velocity.magnitude);
            return false;  //追い続けても追いつけない、スキルによって中断されることができます。
        }
        else
        {
            animator.SetFloat("speed", rb.velocity.magnitude);
            return true;  //終わりまで追いかける
        }
    }

    ///<summary>


    ////// 方向を変える


    ///</summary>
    /// <param name="scaleX"> X方向のスケーリング 左右の向きを制御 </param>
    public void RotateToTarget(int scaleX)
    {
        Vector3 targetScale = bossBody.localScale;
        targetScale.x = scaleX;
        bossBody.localScale = targetScale;
    }

    /// <summary>
    /// ボスが動きを停止しました
    /// </summary>
    /// <param name="power"> 移動減衰倍率 </param>
    public void StopMove(float power)
    {
        rb.velocity /= power;
        animator.SetFloat("speed", rb.velocity.magnitude);
    }

    ///<summary>


    ////// 攻撃タイプを変更する
    /// 変換浮遊砲表示


    ///</summary>
    /// <param name="target"> 浮遊砲攻撃モード </param>
    public void ChangeWeaponType(E_BossAttackMode targetMode)
    {
        if (bossAttackMode == targetMode) return;
        bossAttackMode = targetMode;
        autoGunControl.ChangeMode(targetMode);
    }

    ///<summary>


    ////// アパーチャの向きを調整する


    ///</summary>
    public void OrientationObjectLookAt()
    {
        Vector3 targetPoint = targetPlayer.transform.position;
        targetPoint.y = orientationObject.transform.position.y;
        orientationObject.transform.LookAt(targetPoint);
    }

    /// <summary>
    /// プレイヤーに突進する
    /// </summary>
    public void DodgeToPlayer_Start()
    {
        canGetHit = false;
        bossDashTrailEff.SetActive(true);   //スプリントフィニッシュが始まりました
        animator.SetTrigger("dodge");
    }

    ///<summary>


    ////// 指定された方向に向かって持続的にスプリントします


    ///</summary>
    public void DodgeToPlayer(float dodgePower)
    {
        rb.velocity = attackDir.normalized * dodgePower * baseSpeed;
        animator.SetFloat("speed", rb.velocity.magnitude);
    }

    /// <summary>
    /// スプリント終了
    /// </summary>
    public void DodgeOver()
    {
        bossDashTrailEff.SetActive(false);  //スプリントの終了を追い込む
        canGetHit = true;
    }

    /// <summary>
    /// プレイヤーにフラッシュする
    /// </summary>
    public void FlashToPlayer_Start()
    {
        canGetHit = false;
        animator.SetTrigger("flash");
        StartCoroutine(FlashToPlayer());
    }

    /// <summary>
    /// コルーチンのフラッシュ
    /// </summary>
    IEnumerator FlashToPlayer()
    {
        float time = 0;
        bossDashTrailEff.SetActive(true);
        while (time < 0.5f)
        {
            time += Time.deltaTime;
            mtr.SetFloat("_dissolve", time * 2);
            yield return null;
        }
        time = 0.5f;
        transform.position = targetPlayer.transform.position;
        while (time >= 0)
        {
            time -= Time.deltaTime;
            mtr.SetFloat("_dissolve", time * 2);
            yield return null;
        }
        canGetHit = true;
        bossDashTrailEff.SetActive(false);
        yield break;
    }

    /// <summary>
    /// フィールドにフラッシュ
    /// </summary>
    public void FlashBackToPlayer_Start()
    {
        canGetHit = false;
        animator.SetTrigger("backFlash");
        StartCoroutine(FlashToRoomCenter());
    }

    /// <summary>
    /// コルーチンをフィールドにフラッシュします
    /// </summary>
    IEnumerator FlashToRoomCenter()
    {
        float time = 0;
        Vector3 flashDir = new Vector3();
        Vector3 targetPosition;
        bossDashTrailEff.SetActive(true);
        while (time < 0.5f)
        {
            time += Time.deltaTime;
            mtr.SetFloat("_dissolve", time * 2);
            yield return null;
        }
        time = 0.5f;
        flashDir = roomCenter.position - transform.position;
        flashDir.y = 0;
        flashDir.Normalize();
        targetPosition = roomCenter.position + flashDir.normalized * roomSize;
        targetPosition.y = transform.position.y;
        transform.position = targetPosition;
        while (time >= 0)
        {
            time -= Time.deltaTime;
            mtr.SetFloat("_dissolve", time * 2);
            yield return null;
        }
        bossDashTrailEff.SetActive(false);
        canGetHit = true;
        yield break;
    }
    #endregion

    #region 战斗相关
    /// <summary>
    /// ボスが銃攻撃状態に入ります
    /// </summary>
    /// <param name="type"></param>
    public void BossAttack_Gun(int type)
    {
        autoGunControl.GunAttack(type);
        bossSingingEff.SetActive(true); //詠唱特効
    }

    /// <summary>
    /// プレイヤーはガンアタック状態を終了します
    /// </summary>
    public void BossAttack_Gun_End()
    {
        bossSingingEff.SetActive(false); //詠唱特効
    }
    /// <summary>
    /// ボスが杖攻撃状態に入ります
    /// </summary>
    public void BossAttack_Staff()
    {
        autoGunControl.StaffAttack();
        bossSingingEff.SetActive(true); //詠唱特効
    }

    /// <summary>
    /// プレイヤーは杖攻撃状態を終了します
    /// </summary>
    public void BossAttack_Staff_End()
    {
        bossSingingEff.SetActive(false); //詠唱特効
    }

    /// <summary>
    /// プレイヤーキャラクターの座標を更新する
    /// </summary>
    public void UpdatePlayerPosition()
    {
        attackDir = targetPlayer.transform.position - transform.position;
        RotateToTarget(Vector3.Dot(transform.right, attackDir) > 0 ? -1 : 1);
    }

    ///<summary>


    ////// 武器の衝突体のサイズと位置を設定する


    ///</summary>
    public void SetWeaponTrigger()
    {
        Vector3 temp = attackDir;
        temp.y = 0;
        temp = temp.normalized * (comboNode.attackRange.z + comboNode.attackRangeDeviation);
        weaponTrigger.transform.rotation = Quaternion.LookRotation(temp);
        temp = new Vector3(orientationObject.position.x + temp.x, transform.position.y, orientationObject.position.z + temp.z);
        weaponTrigger.transform.position = temp;
        weaponTrigger.transform.localScale = comboNode.attackRange;
    }

    /// <summary>
    /// トリガーを起動する
    /// 特殊効果を開始する
    /// </summary>
    public void OpenTrigger()
    {
        SetWeaponTrigger();
        weaponTrigger.enabled = true;
        CreateEffect();
    }

    /// <summary>
    /// ボスの攻撃範囲のヒントを設定する
    /// </summary>
    public void SetAttackRangeHint()
    {
        Vector3 temp = attackDir;
        temp.y = 0;
        temp = temp.normalized * (comboNode.attackRange.z + comboNode.attackRangeDeviation);
        attackRangeHint.rotation = Quaternion.LookRotation(temp);
        temp = new Vector3(orientationObject.position.x + temp.x, this.transform.position.y, orientationObject.position.z + temp.z);
        attackRangeHint.position = temp;
        // for (int i = 0; i < attackRangeHint.transform.childCount; i++)
        // {
        //     attackRangeHint.GetChild(i).localScale = comboNode.attackRange;
        // }
        attackRangeHint.gameObject.SetActive(true);
    }

    /// <summary>
    /// ボスエフェクトを作成する
    /// </summary>
    public void CreateEffect()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/BOSS/sword2");
        if (comboNode.attackEffect != null)
        {
            EffectManager.Instance.SetBossAttackEffect(comboNode.attackEffect, attackRangeHint.position + effectAtkPos, attackRangeHint.rotation);
        }
    }

    /// <summary>
    /// トリガーを閉じる
    /// </summary>
    public void CloseTrigger()
    {
        weaponTrigger.enabled = false;
        attackRangeHint.gameObject.SetActive(false);
        // Debug.Log("クローズ");
    }

    /// <summary>
    /// ボスの攻撃衝突ロジック
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            bossData.currentComboAttack = comboNode.baseDamage;
            GameManager.Instance.BossAttack();
        }
    }

    /// <summary>
    /// ボスが傷ついた時のヒットショックエフェクト
    /// </summary>
    /// <param name="damage"></param>
    /// <returns></returns>
    IEnumerator IE_BossHurt(float damage)
    {
        isHurting = true;
        float time = 0;
        Vector3 deviation = bossBody.position - targetPlayer.transform.position;
        deviation.y = 0;
        deviation.Normalize();
        deviation *= hurtEffCurve.Evaluate(damage / bossHurtPer);
        for (int i = 0; i < bossHurtCount; i++)
        {
            mtr.SetColor("_Color0", Color.gray);
            bossBody.position += deviation;
            while (time < bossHurtTime)
            {
                time += Time.deltaTime;
                yield return null;
            }
            time = 0;
            bossBody.position -= deviation;
            mtr.SetColor("_Color0", Color.white);
            while (time < bossHurtTime)
            {
                time += Time.deltaTime;
                yield return null;
            }
            time = 0;
        }
        isHurting = false;
        yield break;
    }

    /// <summary>
    /// ボスがダメージを受ける特殊効果を表示する
    /// </summary>
    private void ShowBossGetHitEffect()
    {
        GameObject enemyGetHit = ObjectPool.Instance.GetObject("Enemy_Attacked", EffectManager.Instance.transform, true, false);
        enemyGetHit.transform.position = transform.position + Vector3.up * 2;
        enemyGetHit.SetActive(true);
        EffectManager.Instance.LetRecycleEffect("Enemy_Attacked", enemyGetHit, 1.5f);
    }

    ///<summary>


    ////// ダメージロジック 変換フェーズロジック


    ///</summary>
    public void Damaged(float damage, bool isCritical = false)
    {
        if (isDead) return;
        ShowBossGetHitEffect();
        PanelManager.Instance.GenerateDamageNum(damage, transform, isCritical);//ダメージ数値を生成する
        if (!isHurting)
        {
            StartCoroutine(IE_BossHurt(damage));
        }
        if (bossData.currentHealth < NextStageHealth)
        {
            stage++;   //フェーズチェンジ
            NextStageHealth = (4 - stage) * 0.2f * bossData.maxHealth;
        }

        if (canInter)
        {
            interTrigger = true;
        }
        // Debug.Log("NextStageHealth" + NextStageHealth);
    }

    /// <summary>
    /// アニメーションの再生を呼び出す準備をしています
    /// </summary>
    public void Summon_Start()
    {
        animator.SetTrigger("beforeSkill");
        bossSingingEff.SetActive(true); //詠唱特効
        FMODUnity.RuntimeManager.PlayOneShot("event:/BOSS/beforeSkill");
    }

    ///<summary>


    ////// 召喚開始 大きな花を生成


    ///</summary>
    public void Summon_Ready()
    {
        bossFlower = Instantiate(bossFlowerPrefab, flowerTransform.position, bossFlowerPrefab.transform.rotation);
        bossFlower.SetActive(true);
        bossSingingEff.SetActive(false); //詠唱特効
    }

    ///<summary>


    ////// 本当のフェーズシフトが始まります


    ///</summary>
    public void StageChangeReal_Start()
    {
        canInter = false;
        interTrigger = false;
        canGetHit = false;
        animator.SetTrigger("beforeSkill");
        bossStageChangeEff.SetActive(true); //詠唱特効
    }

    ///<summary>


    ////// 本当のフェーズチェンジが終了しました


    ///</summary>
    public void StageChangeReal_End()
    {
        canGetHit = true;
        bossStageChangeEff.SetActive(false); //詠唱特効
    }

    ///<summary>


    ////// 準備が整ったら、アニメーションを再生します


    ///</summary>
    public void Rage_Start()
    {
        animator.SetTrigger("beforeSkill");
        bossSingingEff.SetActive(true); //詠唱特効
    }

    ///<summary>


    ////// 狂暴開始、クリティカル率上昇


    ///</summary>
    public void Rage_Ready()
    {
        isRageing = true;
        bossAngryStateEff.SetActive(true);  //狂暴を開始する
        bossData.currentCritical += 0.6f;
        bossSingingEff.SetActive(false); //詠唱特効
    }

    ///<summary>


    ////// 狂暴終了　クリティカル率回復


    ///</summary>
    public void Rage_End()
    {
        isRageing = false;
        bossAngryStateEff.SetActive(false);  //狂暴を開始する
        curRageTime = 0;
        bossData.currentCritical -= 0.6f;
        bossCD.canRage.flag = false;
        // Debug.Log("大暴走終了");
    }

    ///<summary>


    ////// 回復の準備が可能で、中断も可能です。


    ///</summary>
    public void RestoreHealth_Start()
    {
        canInter = true;
        animator.SetTrigger("beforeSkill");
        bossSingingEff.SetActive(true); //詠唱特効
    }

    ///<summary>


    ////// 回復が始まりました。これは中断できません。


    ///</summary>
    public void RestoreHealth_Ready()
    {
        canInter = false;
        bossCD.canRestoreHealth.flag = false;
        bossCD.canRestoreHealth.curTime = 0;
        bossData.currentHealth += Time.deltaTime * HealthReplyVolume;
        bossData.currentHealth = Mathf.Min(bossData.currentHealth, bossData.maxHealth);
        bossSingingEff.SetActive(false); //詠唱特効
    }

    /// <summary>
    /// シールドリターンの準備、中断可能です
    /// </summary>
    public void RestoreShield_Start()
    {
        canInter = true;
        animator.SetTrigger("beforeSkill");
        bossSingingEff.SetActive(true); //詠唱特効
    }

    /// <summary>
    /// シールドの開始、中断不可
    /// </summary>
    public void RestoreShield_Ready()
    {
        canInter = false;
        canGetHit = false;
        bossCD.canRestoreShield.flag = false;
        bossCD.canRestoreShield.curTime = 0;
        characterBuffManager.RaiseShieldHP(Time.deltaTime * ShieldReplyVolume);
    }

    /// <summary>
    /// シールド終了、無敵キャンセル
    /// </summary>
    public void RestoreShield_End()
    {
        canGetHit = true;
        bossSingingEff.SetActive(false); //詠唱特効
    }

    ///<summary>


    ////// 中断可能状態をリセットします


    ///</summary>
    public void ResetCanInter()
    {
        canInter = false;
        interTrigger = false;
        animator.SetBool("attacking", false);
        bossSingingEff.SetActive(false); //詠唱特効
    }
    #endregion

    #region 状态相关
    ///<summary>

    ////// 真のフェーズトランジションロジック

    ///</summary>
    public void BossStageChange()
    {
        canUseBuffCount++;
        RoomManager.Instance.BossBuffRebuild(this, canUseBuffCount);
    }

    /// <summary>
    /// フェーズ切り替え開始、浮遊砲を停止します。
    /// </summary>
    public void ChangeStage_Start()
    {
        if (!animator.GetBool("stageChangeing"))
        {
            bossWeakStateEff.SetActive(true);
            animator.SetTrigger("stageChange");
            CloseTrigger(); //武器のコリジョンボディを閉じる
            bossDashTrailEff.SetActive(false);
            bossSingingEff.SetActive(false);
            bossStageChangeEff.SetActive(false);
            bossData.currentDefend = +6; //怪我が軽減する
            autoGunControl.my_StopAllCoroutines();
            animator.SetBool("attacking", false);
        }
    }

    /// <summary>
    /// フェーズ切り替え終了、浮遊砲を回復し、Buffを更新します。
    /// </summary>
    public void ChangeStage_End()
    {
        CloseTrigger();
        bossWeakStateEff.SetActive(false);
        bossData.currentDefend = 0; //怪我の軽減と回復
        autoGunControl.ResetStates();
    }

    /// <summary>
    /// ボスの死亡ロジック
    /// </summary>
    public void BossState_Die()
    {
        animator.SetTrigger("die");
        GameManager.Instance.currentPlayer.lockHealth = true;   //ボスが死亡、プレイヤーのHPが固定
        CloseTrigger();
        bossDashTrailEff.SetActive(false);
        bossSingingEff.SetActive(false);
        bossStageChangeEff.SetActive(false);
        bossAngryStateEff.SetActive(false);
        bossWeakStateEff.SetActive(false);
        bossDeadStateEff.SetActive(true);
        autoGunControl.ChangeMode_Dead();
        isDead = true;
        behaviorTree.DisableBehavior();
        if (bossFlower != null)
        {
            bossFlower.SetActive(false);
            Destroy(bossFlower);
        }
        FmodManager.Instance.BossState_Die();
    }
    #endregion
}
