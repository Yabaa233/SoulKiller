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
/// プレイヤーは正常に移動します
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
/// スプリント状態に入る
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
/// フラッシュ状態に入る
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
/// リバースフラッシュモードに入る
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
/// プレイヤーを獲得する
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
            // Debug.Log("プレイヤーがいます");
            return TaskStatus.Success;
        }
        else
        {
            // Debug.Log("プレイヤーがいません");
            return TaskStatus.Failure;
        }
    }
}

/// <summary>
/// カメラに向かって
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
/// すべての攻撃が共通の攻撃モード切替パフォーマンスノード
/// </summary>
[TaskCategory("BossBase")]
public class BossChangeWeaponOver : Action
{
    private BossControl bossControl;
    [Header("ボス攻撃モード")]
    public E_BossAttackMode bossAttackMode;
    [Header("停止速度")] public float stopPower = 3;
    public override void OnStart()
    {
        bossControl = gameObject.GetComponent<BossControl>();
        bossControl.ChangeWeaponType(bossAttackMode);
        bossControl.SetAnimatorBool("move", false);
        // Debug.Log("攻撃モードを切り替える");
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
            // Debug.Log("モードの切り替えが完了しました");
            return TaskStatus.Success;
        }
    }
}

#endregion

#region 攻击相关

///<summary>


////// 近接攻撃距離の判断


///</summary>
[TaskCategory("BossCheck")]
public class BossAttack_CanHit : Conditional
{
    public SharedGameObject player;
    [Header("現在の攻撃範囲")]
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

///<summary>


////// 通常の攻撃が始まります


///</summary>
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
        // Debug.Log("剣の通常攻撃");
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
///<summary>

////// 剣攻撃開始

///</summary>
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
        // Debug.Log("剣術攻撃");
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
///<summary>

////// 銃撃攻撃

///</summary>
[TaskCategory("BossBaseAttack/Gun")]
public class BossAttack_Gun : Action
{
    private BossControl bossControl;
    public int attackType;  //アタックタイプ
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
        // Debug.Log("ガンスキルアタック");
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
/// スタッフアタック
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
        // Debug.Log("スタッフスキルアタック");
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
/// チャージ召喚
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

///<summary>


////// 召喚開始


///</summary>
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
        // Debug.Log("召喚スキルが発動されました");
    }
}
#endregion

#region 回复技能相关

///<summary>


////// 回復の準備


///</summary>
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

///<summary>


////// 回復が始まります


///</summary>
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
        // Debug.Log("回復スキル");
    }
    public override TaskStatus OnUpdate()
    {
        bossControl.RestoreHealth_Ready();
        return TaskStatus.Running;
    }
}

/// <summary>
/// シールドチャージ
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
/// シールドの開始
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
        // Debug.Log("シールドスキルのリターン");
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

///<summary>


////// 激しく力を蓄える


///</summary>
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

///<summary>


////// 暴走が始まる


///</summary>
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
        // Debug.Log("狂暴スキルが発動されました");
    }
}
#endregion

#endregion

#region 状态相关

/// <summary>
/// ボスがフェーズを移行したかどうかを判断します
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
            // Debug.Log("フェーズ移行の開始");
            return TaskStatus.Success;  //移行期に入り、弱体化しています。
        }
        else
        {
            return TaskStatus.Failure;
        }
    }
}

/// <summary>
/// ボスのフェーズ移行が終了したかどうかを判断する
/// </summary>
[TaskCategory("BossBaseState")]
public class BossState_StageChange_End : Action
{
    private BossControl bossControl;
    public SharedInt curStage;
    [Header("ステージ2に進むために減らす必要があるヘルスバーの数")] public int stage2HealthBarCount = 1;
    [Header("ステージ3に進むために減らす必要があるヒットポイントの数")] public int stage3HealthBarCount = 3;
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
        // Debug.Log("フェーズ移行が完了しました");
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
/// ボスは本当にフェーズシフトを開始したのですか？
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
/// ボスは本当のフェーズチェンジを終了しました。
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
        //ボスのHPバーとシールドバーをリフレッシュします
        BattleMainPanel battleMainPanel = PanelManager.Instance.GetPanel("BattleMainPanel") as BattleMainPanel;
        battleMainPanel.SetBossInfoAreaVisble(true);
    }
}

/// <summary>
/// ボスが中断可能かどうかを判断してください。
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
/// ボスとプレイヤーの距離が遠いか近いかを判断する
/// 異なるシリーズのスキルを解放する
/// </summary>
[TaskCategory("BossCheck")]
public class BossCheckDistence : Conditional
{
    public SharedGameObject player;
    public SharedTransform boss;
    private Transform tf_Player;
    private Transform tf_Boss;
    [Header("遠近距離の境界")]
    public float limit = 10.0f;
    public bool isFar = true;
    public override void OnStart()
    {
        tf_Player = player.Value.transform;
        tf_Boss = boss.Value;
    }
    public override TaskStatus OnUpdate()
    {
        //limitより小さいということは近距離にいるということなので、近接攻撃を使用します。
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
/// ボスが必要なバフを持っているかどうかを判断します
/// </summary>
[TaskCategory("BossCheck")]
public class BossCheckBuff : Conditional
{
    BossControl bossControl;
    [Header("その後のスキルは、バフが必要で発動できますか？")] public bool needBuff;
    [Header("必要な場合、どのBuffが必要ですか？")] public E_BuffKind buff;
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
/// ボスの現在の段階が要求を満たしているかどうかを判断します
/// </summary>
[TaskCategory("BossCheck")]
public class BossCheckStage : Conditional
{
    private SharedInt curStage; //現在の段階
    [Header("進むことができるステージ数 注：ステージ数は0 1 2 3 4の5段階で、成功の判断条件は現在のステージ数がtargetStage以上であること。")]
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
/// ボスが攻撃を受ける
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
        bossControl.ResetCanInter();    //中断可能状態をリセットします
        // Debug.Log("ボスのチャージスキルを中断しました！");
    }
}

/// <summary>
/// ボスの死亡ロジック
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

///<summary>


////// 単一ボスのスキルCDを設定する


///</summary>
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
/// ボスの各スキルのCDをリセットする
/// </summary>
[TaskCategory("BossCD")]
public class BossOneCDReset : Action
{
    [Header("CDをリセットする必要があるスキル")]
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
/// CDが基底クラスを取得します
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
/// ボスがプレイヤーにダッシュCD
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
/// ボスがプレイヤーにフラッシュCDを向ける
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
/// ボスがプレイヤーに背を向けてフラッシュCDを使う
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
/// ボスの通常攻撃のクールダウン
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
/// ボスの剣技攻撃のクールダウン
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
/// ボスの銃攻撃1CD
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
/// ボスの銃攻撃2CD
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
/// ボスの杖攻撃CD
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
/// ボスの召喚攻撃のクールダウン
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
/// ボスのHP回復CD
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
/// ボスのシールドのCD
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
/// ボスの暴走CD
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

/// <summary>
