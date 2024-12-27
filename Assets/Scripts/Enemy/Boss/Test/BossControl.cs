using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

[System.Serializable]
public class BossCD
{
    [Header("普通攻击CD")]
    public CDClass canNormalAttack = new CDClass();
    [Header("技能攻击CD")]
    public CDClass canSkillAttack = new CDClass();
    [Header("枪技能1攻击CD")]
    public CDClass canGunAttack1 = new CDClass();
    [Header("枪技能2攻击CD")]
    public CDClass canGunAttack2 = new CDClass();
    [Header("杖技能攻击CD")]
    public CDClass canStaffAttack = new CDClass();
    [Header("召唤技能攻击CD")]
    public CDClass canSummonAttack = new CDClass();
    [Header("冲刺CD")]
    public CDClass canDodge = new CDClass();
    [Header("闪现CD")]
    public CDClass canFlash = new CDClass();
    [Header("反向闪现CD")]
    public CDClass canBackFlash = new CDClass();
    [Header("回血CD")]
    public CDClass canRestoreHealth = new CDClass();
    [Header("回盾CD")]
    public CDClass canRestoreShield = new CDClass();
    [Header("狂暴CD")]
    public CDClass canRage = new CDClass();

    public void InitCD()
    {
        GameManager.Instance.CDList.Add(canNormalAttack);   //普攻
        canNormalAttack.curTime = 0;
        canNormalAttack.flag = false;

        GameManager.Instance.CDList.Add(canSkillAttack);    //近战连击
        canSkillAttack.curTime = 0;
        canSkillAttack.flag = false;

        GameManager.Instance.CDList.Add(canGunAttack1);     //射击1
        canGunAttack1.curTime = 0;
        canGunAttack1.flag = false;

        GameManager.Instance.CDList.Add(canGunAttack2);     //射击2
        canGunAttack2.curTime = 0;
        canGunAttack2.flag = false;

        GameManager.Instance.CDList.Add(canStaffAttack);    //法杖
        canStaffAttack.curTime = 0;
        canStaffAttack.flag = false;

        GameManager.Instance.CDList.Add(canSummonAttack);   //召唤
        canSummonAttack.curTime = 0;
        canSummonAttack.flag = false;

        GameManager.Instance.CDList.Add(canDodge);          //冲刺
        canDodge.curTime = 0;
        canDodge.flag = false;

        GameManager.Instance.CDList.Add(canFlash);          //闪现
        canFlash.curTime = 0;
        canFlash.flag = false;

        GameManager.Instance.CDList.Add(canBackFlash);      //反向闪现
        canBackFlash.curTime = 0;
        canBackFlash.flag = false;

        GameManager.Instance.CDList.Add(canRestoreHealth);  //回血
        canRestoreHealth.curTime = 0;
        canRestoreHealth.flag = false;

        GameManager.Instance.CDList.Add(canRestoreShield);  //回盾
        canRestoreShield.curTime = 0;
        canRestoreShield.flag = false;

        GameManager.Instance.CDList.Add(canRage);  //狂暴
        canRage.curTime = 0;
        canRage.flag = false;
    }
    public void ClearnCD()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.CDList.Remove(canNormalAttack);   //普攻

        GameManager.Instance.CDList.Remove(canSkillAttack);    //近战连击

        GameManager.Instance.CDList.Remove(canGunAttack1);     //射击1

        GameManager.Instance.CDList.Remove(canGunAttack2);     //射击2

        GameManager.Instance.CDList.Remove(canStaffAttack);    //法杖

        GameManager.Instance.CDList.Remove(canSummonAttack);   //召唤 

        GameManager.Instance.CDList.Remove(canDodge);          //冲刺

        GameManager.Instance.CDList.Remove(canFlash);          //闪现

        GameManager.Instance.CDList.Remove(canBackFlash);      //反向闪现

        GameManager.Instance.CDList.Remove(canRestoreHealth);  //回血

        GameManager.Instance.CDList.Remove(canRestoreShield);  //回盾

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
    [Header("Boss的Buff管理器")] public CharacterBuffManager characterBuffManager = new CharacterBuffManager();
    [Header("Boss属性模板")] public CharacterData_SO tempCharaterData;
    [Header("Boss属性")] public CharacterData bossData;
    [Header("Boss基础移动速度")] public float baseSpeed;
    [Header("Boss受击震动次数")] public float bossHurtCount;
    [Header("Boss受击震动单次时间")] public float bossHurtTime;
    [Header("Boss受击与伤害值关联反比例系数")] public float bossHurtPer;
    [Header("受击时震动与伤害的比例曲线")] public AnimationCurve hurtEffCurve;

    [Header("Boss攻击模式")] public E_BossAttackMode bossAttackMode = E_BossAttackMode.init;
    [Header("Boss浮游炮")] public AutoGunControl autoGunControl;
    [Header("Boss身体模型")] public Transform bossBody;
    [Header("当前ComboNode")] public ComboNode comboNode;
    [Header("目标Player")] public GameObject targetPlayer;
    [Header("Boss的Buff情况")] public int swordBuffLevel, gunBuffLevel, staffBuffLevel;
    [Header("Boss的材质")] public Material mtr;
    [Header("场地尺寸 用于控制反向闪现的距离")] public float roomSize;

    [Header("各种状态")]
    [Tooltip("是否死亡")] public bool isDead;
    [Tooltip("是否可受击")] public bool canGetHit;
    [Tooltip("是否可以打断")] public bool canInter;
    [Tooltip("是否被打断了")] public bool interTrigger;
    [Tooltip("是否处于狂暴")] public bool isRageing;
    [Tooltip("是否正在受击")] public bool isHurting;
    [Tooltip("是否锁血")] public bool lockHealth = false;
    [Tooltip("当前阶段")] public int stage;

    [Header("设置的内容")]
    [Tooltip("场上是否存在召唤物")] public bool hasFlower;
    [Tooltip("场上的召唤物")] public GameObject bossFlower;
    [Tooltip("攻击特效出现位置补偿")] public Vector3 effectAtkPos = new Vector3(0.0f, 3.0f, 1.0f);
    [Tooltip("回血技能每秒回血量")] public float HealthReplyVolume = 10;
    [Tooltip("回盾技能每秒回盾量")] public float ShieldReplyVolume = 5;
    [Tooltip("狂暴持续时间")] public float rageTime;
    [Tooltip("激光花预制")] public GameObject bossFlowerPrefab;
    private Transform bossEffectParent; //Boss吟唱、转阶段、拖尾特效的挂载节点
    private GameObject bossDashTrailEff;   //boss冲刺拖尾特效
    private GameObject bossSingingEff;      //boss吟唱特效
    private GameObject bossStageChangeEff; //boss转阶段特效
    private GameObject bossAngryStateEff;  //boss狂暴持续特效
    private GameObject bossWeakStateEff;    //boss虚弱特效
    private GameObject bossDeadStateEff;    //boss死亡特效
    private float curRageTime; //已狂暴时间
    private Transform roomCenter;   //场景中心
    private Transform flowerTransform;   //花的出生点
    private BehaviorTree behaviorTree;  //行为树插件
    public BehaviorTree BehaviorTree { get { return behaviorTree; } }   //对外获取用
    private Animator animator;  //动画状态机
    private Rigidbody rb;   //刚体
    private Collider weaponTrigger; //武器碰撞体
    private Transform attackRangeHint; //攻击范围提示
    private Transform orientationObject; //脚底的朝向光圈
    //临时变量，防止重复创建
    private Vector3 dir;    //Boss移动方向
    private Vector3 attackDir;  //Boss此次攻击的方向
    private float NextStageHealth;  //下一阶段血量值
    private int canUseBuffCount = 2;    //可以使用的Buff数量，在指定血量管数时提升

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
        //注册
        GameManager.Instance.currentBoss = this;
        GameManager.Instance.BossDie += BossState_Die;
        //初始化Buff
        characterBuffManager.Init(E_ChararcterType.boss);
        // characterBuffManager.AddBuff(new ShieldBuff(E_ChararcterType.boss, 4), this.gameObject);
        // characterBuffManager.AddBuff(new HpUp(E_ChararcterType.boss, 4), this.gameObject);
        //初始化浮游炮
        autoGunControl.AutoGunInit();
        //初始化武器类型
        ChangeWeaponType(E_BossAttackMode.normal);
        //关闭碰撞体
        weaponTrigger.enabled = false;
        //初始化一些数值
        BossValueInit();
        //注册CD
        bossCD.InitCD();
        //初始化行为树
        BehaviorTreeInit();
        //通知UI显示BossInfo
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
        PanelManager.Instance.SetBossUIVisble(false);//通知UI关闭
    }

    /// <summary>
    /// 初始化一些数值
    /// </summary>
    public void BossValueInit()
    {
        canGetHit = true;   //允许受击
        canInter = false;   //不允许打断
        interTrigger = false;   //不被打断
        isRageing = false;  //不处于狂暴
        hasFlower = false;  //没有花
        NextStageHealth = bossData.maxHealth * 0.8f;
        mtr.SetFloat("_dissolve", 0);
    }

    /// <summary>
    /// 初始化行为树
    /// </summary>
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
    /// 设置动画状态机中的Bool变量
    /// </summary>
    /// <param name="name"> 名称 </param>
    /// <param name="value"> 值 </param>
    public void SetAnimatorBool(string name, bool value)
    {
        animator.SetBool(name, value);
    }

    /// <summary>
    /// 获取动画状态机中的bool变量值
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool GetAnimatorBool(string name)
    {
        return animator.GetBool(name);
    }

    /// <summary>
    /// 设置动画状态机中的Trigger变量
    /// </summary>
    /// <param name="name"> 名称 </param>
    public void SetAnimatorTrigger(string name)
    {
        animator.SetTrigger(name);
    }

    /// <summary>
    /// 重置动画状态机中的Trigger变量
    /// </summary>
    /// <param name="name"> 名称 </param>
    public void ResetAnimatorTrigger(string name)
    {
        animator.ResetTrigger(name);
    }

    /// <summary>
    /// 设置动画状态机中的Int变量
    /// </summary>
    /// <param name="name"> 名称 </param>
    /// <param name="value"> 值 </param>
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
    /// 移动到Player
    /// </summary>
    /// <param name="stoppingDistance"> 停止距离 </param>
    /// <param name="moveSpeed"> 移动速度 </param>
    /// <returns>是否已接近Player</returns>
    public bool MoveToPlayer(float stoppingDistance, float moveSpeed)
    {
        dir = targetPlayer.transform.position - transform.position;
        if (dir.magnitude > stoppingDistance)
        {
            dir.y = rb.velocity.y;
            rb.velocity = dir * moveSpeed * baseSpeed;
            RotateToTarget(Vector3.Dot(transform.right, dir) > 0 ? -1 : 1);
            animator.SetFloat("speed", rb.velocity.magnitude);
            return false;  //没追到一直追，可以被技能中断
        }
        else
        {
            animator.SetFloat("speed", rb.velocity.magnitude);
            return true;  //追到结束
        }
    }

    /// <summary>
    /// 改变朝向
    /// </summary>
    /// <param name="scaleX"> x方向缩放 控制左右朝向 </param>
    public void RotateToTarget(int scaleX)
    {
        Vector3 targetScale = bossBody.localScale;
        targetScale.x = scaleX;
        bossBody.localScale = targetScale;
    }

    /// <summary>
    /// Boss停止移动
    /// </summary>
    /// <param name="power"> 移动衰减倍率 </param>
    public void StopMove(float power)
    {
        rb.velocity /= power;
        animator.SetFloat("speed", rb.velocity.magnitude);
    }

    /// <summary>
    /// 变换攻击类型
    /// 变换浮游炮显示
    /// </summary>
    /// <param name="target"> 浮游炮攻击模式 </param>
    public void ChangeWeaponType(E_BossAttackMode targetMode)
    {
        if (bossAttackMode == targetMode) return;
        bossAttackMode = targetMode;
        autoGunControl.ChangeMode(targetMode);
    }

    /// <summary>
    /// 调整光圈朝向
    /// </summary>
    public void OrientationObjectLookAt()
    {
        Vector3 targetPoint = targetPlayer.transform.position;
        targetPoint.y = orientationObject.transform.position.y;
        orientationObject.transform.LookAt(targetPoint);
    }

    /// <summary>
    /// 向玩家冲刺
    /// </summary>
    public void DodgeToPlayer_Start()
    {
        canGetHit = false;
        bossDashTrailEff.SetActive(true);   //冲刺拖尾开启
        animator.SetTrigger("dodge");
    }

    /// <summary>
    /// 持续向指定方向冲刺
    /// </summary>
    public void DodgeToPlayer(float dodgePower)
    {
        rb.velocity = attackDir.normalized * dodgePower * baseSpeed;
        animator.SetFloat("speed", rb.velocity.magnitude);
    }

    /// <summary>
    /// 冲刺结束
    /// </summary>
    public void DodgeOver()
    {
        bossDashTrailEff.SetActive(false);  //冲刺拖尾关闭
        canGetHit = true;
    }

    /// <summary>
    /// 向玩家闪现
    /// </summary>
    public void FlashToPlayer_Start()
    {
        canGetHit = false;
        animator.SetTrigger("flash");
        StartCoroutine(FlashToPlayer());
    }

    /// <summary>
    /// 闪现协程
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
    /// 向场中闪现
    /// </summary>
    public void FlashBackToPlayer_Start()
    {
        canGetHit = false;
        animator.SetTrigger("backFlash");
        StartCoroutine(FlashToRoomCenter());
    }

    /// <summary>
    /// 向场中闪现协程
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
    /// Boss进入枪攻击状态
    /// </summary>
    /// <param name="type"></param>
    public void BossAttack_Gun(int type)
    {
        autoGunControl.GunAttack(type);
        bossSingingEff.SetActive(true); //吟唱特效
    }

    /// <summary>
    /// 玩家退出枪攻击状态
    /// </summary>
    public void BossAttack_Gun_End()
    {
        bossSingingEff.SetActive(false); //吟唱特效
    }
    /// <summary>
    /// Boss进入杖攻击状态
    /// </summary>
    public void BossAttack_Staff()
    {
        autoGunControl.StaffAttack();
        bossSingingEff.SetActive(true); //吟唱特效
    }

    /// <summary>
    /// 玩家退出杖攻击状态
    /// </summary>
    public void BossAttack_Staff_End()
    {
        bossSingingEff.SetActive(false); //吟唱特效
    }

    /// <summary>
    /// 更新玩家角色坐标
    /// </summary>
    public void UpdatePlayerPosition()
    {
        attackDir = targetPlayer.transform.position - transform.position;
        RotateToTarget(Vector3.Dot(transform.right, attackDir) > 0 ? -1 : 1);
    }

    /// <summary>
    /// 设置武器碰撞体大小、位置
    /// </summary>
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
    /// 开启触发器
    /// 开启特效
    /// </summary>
    public void OpenTrigger()
    {
        SetWeaponTrigger();
        weaponTrigger.enabled = true;
        CreateEffect();
    }

    /// <summary>
    /// 设置Boss攻击提示范围
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
    /// 创建Boss特效
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
    /// 关闭触发器
    /// </summary>
    public void CloseTrigger()
    {
        weaponTrigger.enabled = false;
        attackRangeHint.gameObject.SetActive(false);
        // Debug.Log("关闭");
    }

    /// <summary>
    /// Boss攻击碰撞逻辑
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
    /// Boss受伤时的受击震动效果
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
    /// 显示Boss受击特效
    /// </summary>
    private void ShowBossGetHitEffect()
    {
        GameObject enemyGetHit = ObjectPool.Instance.GetObject("Enemy_Attacked", EffectManager.Instance.transform, true, false);
        enemyGetHit.transform.position = transform.position + Vector3.up * 2;
        enemyGetHit.SetActive(true);
        EffectManager.Instance.LetRecycleEffect("Enemy_Attacked", enemyGetHit, 1.5f);
    }

    /// <summary>
    /// 受伤逻辑 转阶段逻辑
    /// </summary>
    public void Damaged(float damage, bool isCritical = false)
    {
        if (isDead) return;
        ShowBossGetHitEffect();
        PanelManager.Instance.GenerateDamageNum(damage, transform, isCritical);//产生伤害数字 
        if (!isHurting)
        {
            StartCoroutine(IE_BossHurt(damage));
        }
        if (bossData.currentHealth < NextStageHealth)
        {
            stage++;   //换阶段
            NextStageHealth = (4 - stage) * 0.2f * bossData.maxHealth;
        }

        if (canInter)
        {
            interTrigger = true;
        }
        // Debug.Log("NextStageHealth" + NextStageHealth);
    }

    /// <summary>
    /// 准备召唤 播放动画
    /// </summary>
    public void Summon_Start()
    {
        animator.SetTrigger("beforeSkill");
        bossSingingEff.SetActive(true); //吟唱特效
        FMODUnity.RuntimeManager.PlayOneShot("event:/BOSS/beforeSkill");
    }

    /// <summary>
    /// 召唤开始 生成大花
    /// </summary>
    public void Summon_Ready()
    {
        bossFlower = Instantiate(bossFlowerPrefab, flowerTransform.position, bossFlowerPrefab.transform.rotation);
        bossFlower.SetActive(true);
        bossSingingEff.SetActive(false); //吟唱特效
    }

    /// <summary>
    /// 真正的转阶段开始
    /// </summary>
    public void StageChangeReal_Start()
    {
        canInter = false;
        interTrigger = false;
        canGetHit = false;
        animator.SetTrigger("beforeSkill");
        bossStageChangeEff.SetActive(true); //吟唱特效
    }

    /// <summary>
    /// 真正的转阶段结束
    /// </summary>
    public void StageChangeReal_End()
    {
        canGetHit = true;
        bossStageChangeEff.SetActive(false); //吟唱特效
    }

    /// <summary>
    /// 准备狂暴 播放动画
    /// </summary>
    public void Rage_Start()
    {
        animator.SetTrigger("beforeSkill");
        bossSingingEff.SetActive(true); //吟唱特效
    }

    /// <summary>
    /// 狂暴开始 增加暴击率
    /// </summary>
    public void Rage_Ready()
    {
        isRageing = true;
        bossAngryStateEff.SetActive(true);  //开始狂暴
        bossData.currentCritical += 0.6f;
        bossSingingEff.SetActive(false); //吟唱特效
    }

    /// <summary>
    /// 狂暴结束 恢复暴击率
    /// </summary>
    public void Rage_End()
    {
        isRageing = false;
        bossAngryStateEff.SetActive(false);  //开始狂暴
        curRageTime = 0;
        bossData.currentCritical -= 0.6f;
        bossCD.canRage.flag = false;
        // Debug.Log("狂暴结束");
    }

    /// <summary>
    /// 回血准备 可以打断
    /// </summary>
    public void RestoreHealth_Start()
    {
        canInter = true;
        animator.SetTrigger("beforeSkill");
        bossSingingEff.SetActive(true); //吟唱特效
    }

    /// <summary>
    /// 回血开始 不可以打断
    /// </summary>
    public void RestoreHealth_Ready()
    {
        canInter = false;
        bossCD.canRestoreHealth.flag = false;
        bossCD.canRestoreHealth.curTime = 0;
        bossData.currentHealth += Time.deltaTime * HealthReplyVolume;
        bossData.currentHealth = Mathf.Min(bossData.currentHealth, bossData.maxHealth);
        bossSingingEff.SetActive(false); //吟唱特效
    }

    /// <summary>
    /// 回盾准备 可以打断
    /// </summary>
    public void RestoreShield_Start()
    {
        canInter = true;
        animator.SetTrigger("beforeSkill");
        bossSingingEff.SetActive(true); //吟唱特效
    }

    /// <summary>
    /// 回盾开始 不可以打断
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
    /// 回盾结束 取消无敌
    /// </summary>
    public void RestoreShield_End()
    {
        canGetHit = true;
        bossSingingEff.SetActive(false); //吟唱特效
    }

    /// <summary>
    /// 重置可打断状态
    /// </summary>
    public void ResetCanInter()
    {
        canInter = false;
        interTrigger = false;
        animator.SetBool("attacking", false);
        bossSingingEff.SetActive(false); //吟唱特效
    }
    #endregion

    #region 状态相关
    /// <summary>
    /// 真正的转阶段逻辑
    /// </summary>
    public void BossStageChange()
    {
        canUseBuffCount++;
        RoomManager.Instance.BossBuffRebuild(this, canUseBuffCount);
    }

    /// <summary>
    /// 切换阶段开始，停止浮游炮
    /// </summary>
    public void ChangeStage_Start()
    {
        if (!animator.GetBool("stageChangeing"))
        {
            bossWeakStateEff.SetActive(true);
            animator.SetTrigger("stageChange");
            CloseTrigger(); //关闭武器碰撞体
            bossDashTrailEff.SetActive(false);
            bossSingingEff.SetActive(false);
            bossStageChangeEff.SetActive(false);
            bossData.currentDefend = +6; //受伤减轻
            autoGunControl.my_StopAllCoroutines();
            animator.SetBool("attacking", false);
        }
    }

    /// <summary>
    /// 切换阶段结束，恢复浮游炮，更新Buff
    /// </summary>
    public void ChangeStage_End()
    {
        CloseTrigger();
        bossWeakStateEff.SetActive(false);
        bossData.currentDefend = 0; //受伤减轻恢复
        autoGunControl.ResetStates();
    }

    /// <summary>
    /// Boss死亡逻辑
    /// </summary>
    public void BossState_Die()
    {
        animator.SetTrigger("die");
        GameManager.Instance.currentPlayer.lockHealth = true;   //Boss死亡 玩家锁血
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
