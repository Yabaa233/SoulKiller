using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

// public class BossBehaviourTemp : Conditional
public class BossBehaviourTemp : Action
{
    // public SharedInt randomNum;
    // Owner.SetVariable();
    // Owner.GetVariable();
    // public override TaskStatus OnUpdate()
    // {
    //     randomNum.Value = Random.Range(0, 10);
    //     return TaskStatus.Success;
    // }
}
#region 运动控制相关
/// <summary>
/// 正常向玩家移动
/// </summary>
[TaskCategory("BossBaseMove")]
public class BossMoveToPlayer : Action
{
    private BossControl bossControl;
    public float stoppingDistance;
    public float moveSpeed;
    public override void OnStart()
    {
        bossControl = gameObject.GetComponent<BossControl>();
        bossControl.SetAnimatorBool("move", true);
    }
    public override TaskStatus OnUpdate()
    {
        bossControl.MoveToPlayer(stoppingDistance, moveSpeed);
        return TaskStatus.Success;
    }
}

/// <summary>
/// 进入冲刺状态
/// </summary>
[TaskCategory("BossBaseMove")]
public class BossDodgeToPlayer : Action
{
    private BossControl bossControl;
    public override void OnStart()
    {
        bossControl = gameObject.GetComponent<BossControl>();
        bossControl.DodgeToPlayer_Start();
        bossControl.SetAnimatorBool("dodgeing", true);
    }
    public override TaskStatus OnUpdate()
    {
        if (bossControl.GetAnimatorBool("dodgeing"))
        {
            return TaskStatus.Running;
        }
        else
        {
            bossControl.DodgeOver();
            return TaskStatus.Success;
        }
    }
}

/// <summary>
/// 进入闪现状态
/// </summary>
[TaskCategory("BossBaseMove")]
public class BossFlashToPlayer : Action
{
    private BossControl bossControl;
    public override void OnStart()
    {
        bossControl = gameObject.GetComponent<BossControl>();
        bossControl.FlashToPlayer_Start();
        bossControl.SetAnimatorBool("flashing", true);
    }
    public override TaskStatus OnUpdate()
    {
        if (bossControl.GetAnimatorBool("flashing"))
        {
            return TaskStatus.Running;
        }
        else
        {
            return TaskStatus.Success;
        }
    }
}

/// <summary>
/// 进入反向闪现状态
/// </summary>
[TaskCategory("BossBaseMove")]
public class BossFlashBackPlayer : Action
{
    private BossControl bossControl;
    public override void OnStart()
    {
        bossControl = gameObject.GetComponent<BossControl>();
        bossControl.FlashBackToPlayer_Start();
        bossControl.SetAnimatorBool("backFlashing", true);
    }
    public override TaskStatus OnUpdate()
    {
        if (bossControl.GetAnimatorBool("backFlashing"))
        {
            return TaskStatus.Running;
        }
        else
        {
            return TaskStatus.Success;
        }
    }
}

/// <summary>
/// 获取玩家
/// </summary>
[TaskCategory("BossCheck")]
public class GetPlayer : Conditional
{
    public SharedBool hasPlayer;
    public override TaskStatus OnUpdate()
    {
        hasPlayer.Value = Owner.GetVariable("player").GetValue() == null ? false : true;
        if (hasPlayer.Value)
        {
            // Debug.Log("有玩家");
            return TaskStatus.Success;
        }
        else
        {
            // Debug.Log("没有玩家");
            return TaskStatus.Failure;
        }
    }
}

/// <summary>
/// 朝向摄像机
/// </summary>
[TaskCategory("BossBase")]
public class LookCamera : Action
{
    public SharedTransform bodyTransform;
    public override TaskStatus OnUpdate()
    {
        FaceToCamera();
        return base.OnUpdate();
    }
    public void FaceToCamera()
    {
        var rotation = Quaternion.LookRotation(Camera.main.transform.TransformVector(Vector3.forward),
            Camera.main.transform.TransformVector(Vector3.up));
        rotation = new Quaternion(0f, rotation.y, 0, rotation.w);
        var rotationx = Quaternion.Euler(45f, 0f, 0f);
        rotation *= rotationx;
        bodyTransform.Value.rotation = rotation;
    }
}

/// <summary>
/// 所有攻击通用的切换攻击模式演出节点
/// </summary>
[TaskCategory("BossBase")]
public class BossChangeWeaponOver : Action
{
    private BossControl bossControl;
    [Header("Boss攻击模式")]
    public E_BossAttackMode bossAttackMode;
    [Header("停止速度")] public float stopPower = 3;
    public override void OnStart()
    {
        bossControl = gameObject.GetComponent<BossControl>();
        bossControl.ChangeWeaponType(bossAttackMode);
        bossControl.SetAnimatorBool("move", false);
        // Debug.Log("切换攻击模式");
    }

    public override TaskStatus OnUpdate()
    {
        if (bossControl.autoGunControl.ModeChangeOver != 0)
        {
            bossControl.StopMove(stopPower);
            return TaskStatus.Running;
        }
        else
        {
            // Debug.Log("模式切换完成");
            return TaskStatus.Success;
        }
    }
}

#endregion

#region 攻击相关

/// <summary>
/// 近战攻击距离判断
/// </summary>
[TaskCategory("BossCheck")]
public class BossAttack_CanHit : Conditional
{
    public SharedGameObject player;
    [Header("当前攻击攻击范围")]
    public float attackRange;
    private BossControl bossControl;
    public override void OnStart()
    {
        bossControl = gameObject.GetComponent<BossControl>();
    }
    public override TaskStatus OnUpdate()
    {
        if ((bossControl.transform.position - player.Value.transform.position).magnitude < attackRange)
        {
            return TaskStatus.Success;
        }
        else
        {
            return TaskStatus.Failure;
        }
    }
}

#region 普通攻击相关

/// <summary>
/// 普通攻击开始
/// </summary>
[TaskCategory("BossBaseAttack/Normal")]
public class BossAttack_Normal_Start : Action
{
    private BossControl bossControl;
    public override void OnStart()
    {
        if (bossControl == null)
        {
            bossControl = gameObject.GetComponent<BossControl>();
        }
        bossControl.SetAnimatorBool("move", false);
        bossControl.SetAnimatorInt("normalType", Random.Range((int)1, (int)4));
        bossControl.SetAnimatorBool("attacking", true);
        bossControl.SetAnimatorTrigger("normalAttack");
        // Debug.Log("剑普通攻击");
    }
    public override TaskStatus OnUpdate()
    {
        if (bossControl.GetAnimatorBool("attacking"))
        {
            return TaskStatus.Running;
        }
        else
        {
            return TaskStatus.Success;
        }
    }
}
#endregion

#region 剑攻击相关
/// <summary>
/// 剑攻击开始
/// </summary>
[TaskCategory("BossBaseAttack/Sword")]
public class BossAttack_Sword_Start : Action
{
    private BossControl bossControl;
    public override void OnStart()
    {
        if (bossControl == null)
        {
            bossControl = gameObject.GetComponent<BossControl>();
        }
        bossControl.SetAnimatorBool("move", false);
        bossControl.SetAnimatorBool("attacking", true);
        bossControl.SetAnimatorTrigger("skill");
        // Debug.Log("剑技能攻击");
    }
    public override TaskStatus OnUpdate()
    {
        if (bossControl.GetAnimatorBool("attacking"))
        {
            return TaskStatus.Running;
        }
        else
        {
            return TaskStatus.Success;
        }
    }
}

#endregion

#region 枪攻击相关
/// <summary>
/// 枪攻击
/// </summary>
[TaskCategory("BossBaseAttack/Gun")]
public class BossAttack_Gun : Action
{
    private BossControl bossControl;
    public int attackType;  //攻击类型
    public override void OnStart()
    {
        if (bossControl == null)
        {
            bossControl = gameObject.GetComponent<BossControl>();
        }
        bossControl.SetAnimatorBool("move", false);
        bossControl.SetAnimatorBool("attacking", true);
        bossControl.SetAnimatorTrigger("beforeSkill");
        bossControl.BossAttack_Gun(attackType);
        // Debug.Log("枪技能攻击");
    }
    public override TaskStatus OnUpdate()
    {
        if (bossControl.GetAnimatorBool("attacking"))
        {
            return TaskStatus.Running;
        }
        else
        {
            return TaskStatus.Success;
        }
    }
}
#endregion

#region 法杖攻击相关
/// <summary>
/// 法杖攻击
/// </summary>
[TaskCategory("BossBaseAttack/Staff")]
public class BossAttack_Staff : Action
{
    private BossControl bossControl;
    public override void OnStart()
    {
        if (bossControl == null)
        {
            bossControl = gameObject.GetComponent<BossControl>();
        }
        bossControl.SetAnimatorBool("move", false);
        bossControl.SetAnimatorBool("attacking", true);
        bossControl.SetAnimatorTrigger("beforeSkill");
        bossControl.BossAttack_Staff();
        // Debug.Log("杖技能攻击");
    }
    public override TaskStatus OnUpdate()
    {
        if (bossControl.GetAnimatorBool("attacking"))
        {
            return TaskStatus.Running;
        }
        else
        {
            return TaskStatus.Success;
        }
    }
}

/// <summary>
/// 召唤蓄力
/// </summary>
[TaskCategory("BossBaseAttack/Sunmmon")]
public class BossAttack_Summon_Start : Action
{
    private BossControl bossControl;
    public override void OnStart()
    {
        if (bossControl == null)
        {
            bossControl = gameObject.GetComponent<BossControl>();
        }
        bossControl.SetAnimatorBool("move", false);
        bossControl.Summon_Start();
        bossControl.StopMove(3);
    }
}

/// <summary>
/// 召唤开始
/// </summary>
[TaskCategory("BossBaseAttack/Sunmmon")]
public class BossAttack_Summon_Ready : Action
{
    private BossControl bossControl;
    public override void OnStart()
    {
        if (bossControl == null)
        {
            bossControl = gameObject.GetComponent<BossControl>();
        }
        bossControl.Summon_Ready();
        bossControl.StopMove(3);
        // Debug.Log("召唤技能释放");
    }
}
#endregion

#region 回复技能相关

/// <summary>
/// 回血准备
/// </summary>
[TaskCategory("BossSkill/Health")]
public class BossSkill_Health_Start : Action
{
    private BossControl bossControl;
    public override void OnStart()
    {
        if (bossControl == null)
        {
            bossControl = gameObject.GetComponent<BossControl>();
        }
        bossControl.SetAnimatorBool("move", false);
        bossControl.RestoreHealth_Start();
        bossControl.StopMove(3);
    }
}

/// <summary>
/// 回血开始
/// </summary>
[TaskCategory("BossSkill/Health")]
public class BossSkill_Health_Ready : Action
{
    private BossControl bossControl;
    public override void OnStart()
    {
        if (bossControl == null)
        {
            bossControl = gameObject.GetComponent<BossControl>();
        }
        bossControl.RestoreHealth_Ready();
        bossControl.StopMove(3);
        // Debug.Log("回血技能");
    }
    public override TaskStatus OnUpdate()
    {
        bossControl.RestoreHealth_Ready();
        return TaskStatus.Running;
    }
}

/// <summary>
/// 回盾蓄力
/// </summary>
[TaskCategory("BossSkill/Shield")]
public class BossSkill_Shield_Start : Action
{
    private BossControl bossControl;
    public override void OnStart()
    {
        if (bossControl == null)
        {
            bossControl = gameObject.GetComponent<BossControl>();
        }
        bossControl.SetAnimatorBool("move", false);
        bossControl.RestoreShield_Start();
        bossControl.StopMove(3);
    }
}

/// <summary>
/// 回盾开始
/// </summary>
[TaskCategory("BossSkill/Shield")]
public class BossSkill_Shield_Ready : Action
{
    private BossControl bossControl;
    public override void OnStart()
    {
        if (bossControl == null)
        {
            bossControl = gameObject.GetComponent<BossControl>();
        }
        bossControl.RestoreShield_Ready();
        bossControl.StopMove(3);
        // Debug.Log("回盾技能");
    }
    public override TaskStatus OnUpdate()
    {
        bossControl.RestoreShield_Ready();
        return TaskStatus.Running;
    }
}

[TaskCategory("BossSkill/Shield")]
public class BossSkill_Shield_End : Action
{
    private BossControl bossControl;
    public override void OnStart()
    {
        if (bossControl == null)
        {
            bossControl = gameObject.GetComponent<BossControl>();
        }
        bossControl.RestoreShield_End();
    }
}

/// <summary>
/// 狂暴蓄力
/// </summary>
[TaskCategory("BossSkill/Rage")]
public class BossSkill_Rage_Start : Action
{
    private BossControl bossControl;
    public override void OnStart()
    {
        if (bossControl == null)
        {
            bossControl = gameObject.GetComponent<BossControl>();
        }
        bossControl.SetAnimatorBool("move", false);
        bossControl.Rage_Start();
        bossControl.StopMove(3);
    }
}

/// <summary>
/// 狂暴开始
/// </summary>
[TaskCategory("BossSkill/Rage")]
public class BossSkill_Rage_Ready : Action
{
    private BossControl bossControl;
    public override void OnStart()
    {
        if (bossControl == null)
        {
            bossControl = gameObject.GetComponent<BossControl>();
        }
        bossControl.Rage_Ready();
        bossControl.StopMove(3);
        // Debug.Log("狂暴技能释放");
    }
}
#endregion

#endregion

#region 状态相关

/// <summary>
/// 判断Boss是否转阶段
/// </summary>
[TaskCategory("BossBaseState")]
public class BossState_StageChange : Conditional
{
    public SharedInt curStage;
    private BossControl bossControl;
    public override void OnStart()
    {
        if (bossControl == null)
        {
            bossControl = gameObject.GetComponent<BossControl>();
        }
    }
    public override TaskStatus OnUpdate()
    {
        if (curStage.Value != bossControl.stage)
        {
            bossControl.SetAnimatorBool("move", false);
            bossControl.ChangeStage_Start();
            // Debug.Log("转阶段开始");
            return TaskStatus.Success;  //进入转阶段虚弱
        }
        else
        {
            return TaskStatus.Failure;
        }
    }
}

/// <summary>
/// 判断Boss转阶段结束
/// </summary>
[TaskCategory("BossBaseState")]
public class BossState_StageChange_End : Action
{
    private BossControl bossControl;
    public SharedInt curStage;
    [Header("进入阶段2需要扣除的血条数")] public int stage2HealthBarCount = 1;
    [Header("进入阶段3需要扣除的血条数")] public int stage3HealthBarCount = 3;
    public override void OnStart()
    {
        if (bossControl == null)
        {
            bossControl = gameObject.GetComponent<BossControl>();
        }
        bossControl.ChangeStage_End();
        curStage.Value = bossControl.stage;
        if (curStage.Value == stage2HealthBarCount || curStage.Value == stage3HealthBarCount)
        {
            bossControl.BossStageChange();
        }
        // Debug.Log("转阶段结束");
    }
    public override TaskStatus OnUpdate()
    {
        if (curStage.Value == stage2HealthBarCount || curStage.Value == stage3HealthBarCount)
        {
            return TaskStatus.Success;
        }
        else
        {
            return TaskStatus.Failure;
        }
    }
}

/// <summary>
/// Boss是否真正开始转阶段
/// </summary>
[TaskCategory("BossBaseState")]
public class BossState_StageChangeReal : Action
{
    private BossControl bossControl;
    public override void OnStart()
    {
        if (bossControl == null)
        {
            bossControl = gameObject.GetComponent<BossControl>();
        }
        bossControl.StageChangeReal_Start();
    }
}

/// <summary>
/// Boss结束真正的转阶段
/// </summary>
[TaskCategory("BossBaseState")]
public class BossState_StageChangeRealEnd : Action
{
    private BossControl bossControl;
    public override void OnStart()
    {
        if (bossControl == null)
        {
            bossControl = gameObject.GetComponent<BossControl>();
        }
        bossControl.StageChangeReal_End();
        //刷新boss血条盾条
        BattleMainPanel battleMainPanel = PanelManager.Instance.GetPanel("BattleMainPanel") as BattleMainPanel;
        battleMainPanel.SetBossInfoAreaVisble(true);
    }
}

/// <summary>
/// 判断Boss是否可打断
/// </summary>
[TaskCategory("BossBaseState")]
public class BossBaseState_CanHit : Conditional
{
    private BossControl bossControl;
    public override void OnStart()
    {
        if (bossControl == null)
        {
            bossControl = gameObject.GetComponent<BossControl>();
        }
    }
    public override TaskStatus OnUpdate()
    {
        if (bossControl.canInter && bossControl.interTrigger)
        {
            return TaskStatus.Success;
        }
        else
        {
            return TaskStatus.Failure;
        }
    }
}

/// <summary>
/// 判断Boss与玩家的距离远或近
/// 释放不同系列技能
/// </summary>
[TaskCategory("BossCheck")]
public class BossCheckDistence : Conditional
{
    public SharedGameObject player;
    public SharedTransform boss;
    private Transform tf_Player;
    private Transform tf_Boss;
    [Header("远近距离分界")]
    public float limit = 10.0f;
    public bool isFar = true;
    public override void OnStart()
    {
        tf_Player = player.Value.transform;
        tf_Boss = boss.Value;
    }
    public override TaskStatus OnUpdate()
    {
        //小于limit说明在近距离，使用近战攻击
        if ((tf_Player.position - tf_Boss.position).magnitude < limit)
        {
            return isFar ? TaskStatus.Failure : TaskStatus.Success;
        }
        else
        {
            return isFar ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}

/// <summary>
/// 判断Boss是否持有所需Buff
/// </summary>
[TaskCategory("BossCheck")]
public class BossCheckBuff : Conditional
{
    BossControl bossControl;
    [Header("之后的技能是否需要Buff才能释放")] public bool needBuff;
    [Header("如果需要 那么需要哪个Buff")] public E_BuffKind buff;
    public override void OnStart()
    {
        if (bossControl == null)
        {
            bossControl = gameObject.GetComponent<BossControl>();
        }
    }
    public override TaskStatus OnUpdate()
    {
        if (needBuff)
        {
            return bossControl.characterBuffManager.FindBuff(buff) ? TaskStatus.Success : TaskStatus.Failure;
        }
        else
        {
            return TaskStatus.Success;
        }
    }
}

/// <summary>
/// 判断Boss当前阶段是否满足需求
/// </summary>
[TaskCategory("BossCheck")]
public class BossCheckStage : Conditional
{
    private SharedInt curStage; //当前阶段
    [Header("能够进入后续阶段的阶段数 注意：阶段数为0 1 2 3 4阶段，判定成功的条件是当前阶段数大于等于targetStage。")]
    public int targetStage;
    public override void OnStart()
    {
        curStage = (SharedInt)Owner.GetVariable("curStage");
    }
    public override TaskStatus OnUpdate()
    {
        return curStage.Value >= targetStage ? TaskStatus.Success : TaskStatus.Failure;
    }
}

/// <summary>
/// Boss受击
/// </summary>
[TaskCategory("BossBaseState")]
public class BossBaseState_GetHit : Action
{
    private BossControl bossControl;
    public override void OnStart()
    {
        if (bossControl == null)
        {
            bossControl = gameObject.GetComponent<BossControl>();
        }
        bossControl.SetAnimatorTrigger("hurt");
        bossControl.ResetCanInter();    //重置可打断状态
        // Debug.Log("打断Boss读条技能！");
    }
}

/// <summary>
/// Boss死亡逻辑
/// </summary>
[TaskCategory("BossBaseState")]
public class BossBaseState_Die : Conditional
{
    private BossControl bossControl;
    public override void OnStart()
    {
        if (bossControl == null)
        {
            bossControl = gameObject.GetComponent<BossControl>();
        }
    }
    public override TaskStatus OnUpdate()
    {
        return bossControl.isDead ? TaskStatus.Success : TaskStatus.Failure;
    }
}
#endregion

#region CD获取节点
public enum BossCDType
{
    canNormalAttack,
    canSkillAttack,
    canGunAttack1,
    canGunAttack2,
    canStaffAttack,
    canSummonAttack,
    canDodge,
    canFlash,
    canBackFlash,
    canRestoreHealth,
    canRestoreShield,
    canRage
}

/// <summary>
/// 设置单个Boss技能CD
/// </summary>
[TaskCategory("BossCD")]
public class BossCDSet : Action
{
    public BossCDType bossCDType;
    public float targetMaxCDTime;
    protected BossControl bossControl;
    private bool isSeted = false;
    public override void OnStart()
    {
        if (bossControl == null)
        {
            bossControl = GetComponent<BossControl>();
        }
        if (!isSeted)
        {
            isSeted = true;
            switch (bossCDType)
            {
                case BossCDType.canNormalAttack: bossControl.bossCD.canNormalAttack.maxCDTime = targetMaxCDTime; break;
                case BossCDType.canSkillAttack: bossControl.bossCD.canSkillAttack.maxCDTime = targetMaxCDTime; break;
                case BossCDType.canGunAttack1: bossControl.bossCD.canGunAttack1.maxCDTime = targetMaxCDTime; break;
                case BossCDType.canGunAttack2: bossControl.bossCD.canGunAttack2.maxCDTime = targetMaxCDTime; break;
                case BossCDType.canStaffAttack: bossControl.bossCD.canStaffAttack.maxCDTime = targetMaxCDTime; break;
                case BossCDType.canSummonAttack: bossControl.bossCD.canSummonAttack.maxCDTime = targetMaxCDTime; break;
                case BossCDType.canDodge: bossControl.bossCD.canDodge.maxCDTime = targetMaxCDTime; break;
                case BossCDType.canFlash: bossControl.bossCD.canFlash.maxCDTime = targetMaxCDTime; break;
                case BossCDType.canBackFlash: bossControl.bossCD.canBackFlash.maxCDTime = targetMaxCDTime; break;
                case BossCDType.canRestoreHealth: bossControl.bossCD.canRestoreHealth.maxCDTime = targetMaxCDTime; break;
                case BossCDType.canRestoreShield: bossControl.bossCD.canRestoreShield.maxCDTime = targetMaxCDTime; break;
                case BossCDType.canRage: bossControl.bossCD.canRage.maxCDTime = targetMaxCDTime; break;
                default: break;
            }
        }
    }
}

/// <summary>
/// 重置Boss单个技能的CD
/// </summary>
[TaskCategory("BossCD")]
public class BossOneCDReset : Action
{
    [Header("需要重置CD的技能")]
    public BossCDType bossCDType;
    protected BossControl bossControl;
    public override void OnStart()
    {
        if (bossControl == null)
        {
            bossControl = GetComponent<BossControl>();
        }
        switch (bossCDType)
        {
            case BossCDType.canNormalAttack: ResetCD(bossControl.bossCD.canNormalAttack); break;
            case BossCDType.canSkillAttack: ResetCD(bossControl.bossCD.canSkillAttack); break;
            case BossCDType.canGunAttack1: ResetCD(bossControl.bossCD.canGunAttack1); break;
            case BossCDType.canGunAttack2: ResetCD(bossControl.bossCD.canGunAttack2); break;
            case BossCDType.canStaffAttack: ResetCD(bossControl.bossCD.canStaffAttack); break;
            case BossCDType.canSummonAttack: ResetCD(bossControl.bossCD.canSummonAttack); break;
            case BossCDType.canDodge: ResetCD(bossControl.bossCD.canDodge); break;
            case BossCDType.canFlash: ResetCD(bossControl.bossCD.canFlash); break;
            case BossCDType.canBackFlash: ResetCD(bossControl.bossCD.canBackFlash); break;
            case BossCDType.canRestoreHealth: ResetCD(bossControl.bossCD.canRestoreHealth); break;
            case BossCDType.canRestoreShield: ResetCD(bossControl.bossCD.canRestoreShield); break;
            case BossCDType.canRage: ResetCD(bossControl.bossCD.canRage); break;
            default: break;
        }
    }
    private void ResetCD(CDClass cd)
    {
        cd.flag = true;
        cd.curTime = 0;
    }
}

[TaskCategory("BossCD")]
public class BossAllCDSet : Action
{
    public float canNormalAttack;
    public float canSkillAttack;
    public float canGunAttack1;
    public float canGunAttack2;
    public float canStaffAttack;
    public float canSummonAttack;
    public float canDodge;
    public float canFlash;
    public float canBackFlash;
    public float canRestoreHealth;
    public float canRestoreShield;
    public float canRage;

    public float waitEmptyTime;

    protected BossControl bossControl;
    private bool isSeted = false;
    public override void OnStart()
    {
        if (bossControl == null)
        {
            bossControl = GetComponent<BossControl>();
        }
        bossControl.bossCD.canNormalAttack.maxCDTime = canNormalAttack;
        bossControl.bossCD.canSkillAttack.maxCDTime = canSkillAttack;
        bossControl.bossCD.canGunAttack1.maxCDTime = canGunAttack1;
        bossControl.bossCD.canGunAttack2.maxCDTime = canGunAttack2;
        bossControl.bossCD.canStaffAttack.maxCDTime = canStaffAttack;
        bossControl.bossCD.canSummonAttack.maxCDTime = canSummonAttack;
        bossControl.bossCD.canDodge.maxCDTime = canDodge;
        bossControl.bossCD.canFlash.maxCDTime = canFlash;
        bossControl.bossCD.canBackFlash.maxCDTime = canBackFlash;
        bossControl.bossCD.canRestoreHealth.maxCDTime = canRestoreHealth;
        bossControl.bossCD.canRestoreShield.maxCDTime = canRestoreShield;
        bossControl.bossCD.canRage.maxCDTime = canRage;

        //GlobalVariables.Instance.SetVariableValue("WaitTime",waitEmptyTime);
        Owner.SetVariableValue("WaitEmptyTime", waitEmptyTime);

    }
}

/// <summary>
/// CD获取基类
/// </summary>
public class BossCDConditional : Conditional
{
    protected BossControl bossControl;
    public override void OnStart()
    {
        if (bossControl == null)
        {
            bossControl = gameObject.GetComponent<BossControl>();
        }
    }
}

/// <summary>
/// BOSS向玩家冲刺CD
/// </summary>
[TaskCategory("BossCD")]
public class BossCanDodgeToPlayer : BossCDConditional
{
    public override TaskStatus OnUpdate()
    {
        return bossControl.bossCD.canDodge.flag && !bossControl.GetAnimatorBool("attacking") ? TaskStatus.Success : TaskStatus.Failure;
    }
}

/// <summary>
/// BOSS向玩家闪现CD
/// </summary>
[TaskCategory("BossCD")]
public class BossCanFlashToPlayer : BossCDConditional
{
    public override TaskStatus OnUpdate()
    {
        return bossControl.bossCD.canFlash.flag && !bossControl.GetAnimatorBool("attacking") ? TaskStatus.Success : TaskStatus.Failure;
    }
}

/// <summary>
/// BOSS背向玩家闪现CD
/// </summary>
[TaskCategory("BossCD")]
public class BossCanFlashBackPlayer : BossCDConditional
{
    public override TaskStatus OnUpdate()
    {
        return bossControl.bossCD.canBackFlash.flag && !bossControl.GetAnimatorBool("attacking") ? TaskStatus.Success : TaskStatus.Failure;
    }
}

/// <summary>
/// BOSS普通攻击CD
/// </summary>
[TaskCategory("BossCD")]
public class BossAttackCD_Normal : BossCDConditional
{
    public override TaskStatus OnUpdate()
    {
        return bossControl.bossCD.canNormalAttack.flag && !bossControl.GetAnimatorBool("attacking") ? TaskStatus.Success : TaskStatus.Failure;
    }
}

/// <summary>
/// BOSS剑技能攻击CD
/// </summary>
[TaskCategory("BossCD")]
public class BossAttackCD_SwordSkill : BossCDConditional
{
    public override TaskStatus OnUpdate()
    {
        return bossControl.bossCD.canSkillAttack.flag && !bossControl.GetAnimatorBool("attacking") ? TaskStatus.Success : TaskStatus.Failure;
    }
}

/// <summary>
/// BOSS枪攻击1CD
/// </summary>
[TaskCategory("BossCD")]
public class BossAttackCD_Shot01 : BossCDConditional
{
    public override TaskStatus OnUpdate()
    {
        return bossControl.bossCD.canGunAttack1.flag && !bossControl.GetAnimatorBool("attacking") ? TaskStatus.Success : TaskStatus.Failure;
    }
}

/// <summary>
/// BOSS枪攻击2CD
/// </summary>
[TaskCategory("BossCD")]
public class BossAttackCD_Shot02 : BossCDConditional
{
    public override TaskStatus OnUpdate()
    {
        return bossControl.bossCD.canGunAttack2.flag && !bossControl.GetAnimatorBool("attacking") ? TaskStatus.Success : TaskStatus.Failure;
    }
}


/// <summary>
/// BOSS杖攻击CD
/// </summary>
[TaskCategory("BossCD")]
public class BossAttackCD_Staff : BossCDConditional
{
    public override TaskStatus OnUpdate()
    {
        return bossControl.bossCD.canStaffAttack.flag && !bossControl.GetAnimatorBool("attacking") ? TaskStatus.Success : TaskStatus.Failure;
    }
}

/// <summary>
/// Boss召唤攻击CD
/// </summary>
[TaskCategory("BossCD")]
public class BossAttackCD_Summon : BossCDConditional
{
    public override TaskStatus OnUpdate()
    {
        return bossControl.bossCD.canSummonAttack.flag && !bossControl.hasFlower && !bossControl.GetAnimatorBool("attacking") ? TaskStatus.Success : TaskStatus.Failure;
    }
}

/// <summary>
/// Boss回血CD
/// </summary>
[TaskCategory("BossCD/Skill")]
public class BossBuffCD_Health : BossCDConditional
{
    public override TaskStatus OnUpdate()
    {
        return bossControl.bossCD.canRestoreHealth.flag && !bossControl.GetAnimatorBool("attacking") ? TaskStatus.Success : TaskStatus.Failure;
    }
}

/// <summary>
/// Boss回盾CD
/// </summary>
[TaskCategory("BossCD/Skill")]
public class BossBuffCD_Shield : BossCDConditional
{
    public override TaskStatus OnUpdate()
    {
        return bossControl.bossCD.canRestoreShield.flag && !bossControl.GetAnimatorBool("attacking") ? TaskStatus.Success : TaskStatus.Failure;
    }
}

/// <summary>
/// Boss狂暴CD
/// </summary>
[TaskCategory("BossCD/Skill")]
public class BossBuffCD_Rage : BossCDConditional
{
    public override TaskStatus OnUpdate()
    {
        return bossControl.bossCD.canRage.flag && !bossControl.GetAnimatorBool("attacking") && !bossControl.isRageing ? TaskStatus.Success : TaskStatus.Failure;
    }
}
#endregion

