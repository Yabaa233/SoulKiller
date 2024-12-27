using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum E_WeaponType
{
    sword,
    gun,
    staff
}

[RequireComponent(typeof(Rigidbody))]
public class PlayerControl : MonoBehaviour
{
    [Header("战斗相关")]
    [Tooltip("当前武器种类")] public E_WeaponType weaponType; //武器种类
    [Tooltip("射击打击感相关配置")] public ComboNode shotComboNode;
    [Tooltip("法术打击感相关配置")] public ComboNode magicComboNode;
    public ComboNode currentComboNode;  //当前连招节点
    public CDClass dodgeCD = new CDClass(); //冲刺CD
    private bool canShot;   //枪模式长按连射射击时间是否达标
    private bool magicBallStart = false;    //是否开始蓄力
    public float staffHoldTime;   //目标蓄力时间
    public float curHoldTime;   //当前蓄力时间
    public int dodgeCount = 1;  //冲刺可用次数
    [Header("武器Buff等级")]
    public int swordBuffLevel;  //剑Buff等级
    public int swordBuffTimes;  //剑可连击次数
    public int gunBuffLevel;    //枪Buff等级
    public int staffBuffLevel;  //杖Buff等级
    [Header("浮游炮控制组件")]
    public GunControl gunControl;   //浮游炮控制组件
    [SerializeField]
    public GameObject comboTrigger;     //连招的攻击范围触发器
    [Header("动画控制器")]
    public Animator animator;
    [Header("移动表现相关")]
    public Vector3 moveRes;    //移动方向结果
    public float staffStopLerp = 0.02f;
    public bool useMouseScale = false; //使用鼠标朝向结果，不使用键盘输入朝向结果，用于锁定攻击时朝向
    private int scaleRes_Move = 1;   //移动左右朝向结果
    private int scaleRes_Mouse = 1;   //用于控制角色攻击时左右旋转
    public Vector3 targetPoint;    //防止重复创建的鼠标朝向
    [Header("角色刚体")]
    public Rigidbody rb;   //角色刚体
    [Tooltip("骨骼")] public Transform skeletal;  //骨骼
    [Tooltip("脚底朝向光圈")] public Transform orientationObject; //脚底的朝向光圈
    [Header("角色数值模板（不会使用的）")]
    public CharacterData_SO tempCharaterData;   //用于克隆
    [Header("角色属性相关")]
    public CharacterData characterData; //角色属性配置SO文件的实例
    [Tooltip("移动速度")] public Vector2 speed = new Vector3();
    [Header("角色Buff管理器")]
    public CharacterBuffManager characterBuffManager;   //当前Buff的管理列表
    // public GameObject TestCube; //测试攻击范围
    public bool lockHealth = false;
    public bool isDead = false;
    public bool IsDead { get { return isDead; } }
    // [Header("角色声音控制器")]
    [Header("特效显示相关")]
    [Tooltip("冲刺特效出现位置补偿")] public Vector3 effectDashPos = new Vector3(0.0f, 1.5f, 0.0f);
    [Tooltip("攻击特效出现位置补偿")] public Vector3 effectAtkPos = new Vector3(0.0f, 1.5f, 0.0f);

    private Vector2 rightStickPos = new Vector2(960, 540);  //右摇杆方向控制器的坐标
    private Vector2 rightStickMoveSpeed = new Vector2(1200, 800);  //右摇杆灵敏度

    #region 初始化相关
    private void Awake()
    {
        PlayerInit("Player");
    }

    /// <summary>
    ///  初始化获取角色上各功能组件
    /// </summary>
    /// <param name="tagStr"> 期望初始化的Tag </param>
    private void PlayerInit(string tagStr)
    {
        //组件获取
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        gunControl = transform.Find("GunParent").GetComponent<GunControl>();
        //子物体获取
        skeletal = transform.Find("BOSS");
        orientationObject = transform.Find("OrientationObject");
        //自身属性设置
        transform.tag = tagStr;
        useMouseScale = false;  //默认使用键盘朝向
        ResetAnimState();//重置自身动画状态
        //创建各子component
        characterData = new CharacterData(Instantiate(tempCharaterData));   //创建实例用来初始化
        characterBuffManager = new CharacterBuffManager();
    }

    private void Start()
    {
        //向GameManager注册
        GameManager.Instance.currentPlayer = this;
        GameManager.Instance.PlayerDie += PlayerDie;
        //注册CD
        GameManager.Instance.CDList.Add(dodgeCD);
        dodgeCD.flag = true;
        //初始化Buff系统
        characterBuffManager.Init(E_ChararcterType.player);
        PlayerBuffRebuild(BuffDataManager.Instance.playerBuffList);
        characterBuffManager.RefreshData();
        //初始化武器类型为剑
        gunControl.Init();
        SetWeaponType(E_WeaponType.sword);
    }

    private void Update()
    {
        characterBuffManager.OnUpdate(Time.deltaTime);
        // Debug.Log("HoldRight");
        if (weaponType == E_WeaponType.gun)
        {
            PlayerAttack_Gun();
        }
        else if (weaponType == E_WeaponType.staff)
        {
            PlayerAttack_Staff();
        }
        if (rb.velocity.y > 0.5f)
        {
            Vector3 resVelocity = rb.velocity;
            resVelocity.y *= -1.5f;
            rb.velocity = resVelocity;
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerDie -= PlayerDie;
            GameManager.Instance.CDList.Remove(dodgeCD);
        }
    }

    /// <summary>
    /// 初始化动画参数状态
    /// </summary>
    public void ResetAnimState()
    {
        if (animator == null)
        {
#if UNITY_EDITOR
            Debug.LogError("当前角色动画控制器为空" + this.gameObject);
#endif
            return;
        }
        else
        {
            animator.ResetTrigger("die");
            animator.ResetTrigger("hurt");
            animator.ResetTrigger("dodge");

            animator.SetBool("canMove", true);
            animator.SetBool("canAttack", true);
            animator.SetBool("canDodge", true);
            animator.SetBool("isDie", false);

            animator.SetBool("attack", false);

            animator.SetFloat("speed", 0);
        }
    }
    #endregion

    #region 输入相关
    /// <summary>
    ///  获取角色的移动向量
    /// </summary>
    public void GetPlayerInput_Move(InputAction.CallbackContext callbackContext)
    {
        if (magicBallStart) return;
        Vector2 movement = callbackContext.ReadValue<Vector2>();
        moveRes.x = movement.x;
        moveRes.y = rb.velocity.y;
        moveRes.z = movement.y;
        if (moveRes.x == 0 && moveRes.z == 0)
        {
            return;
        }
        //用以自己为参考的目标点的世界坐标减去自己的世界坐标（但是操作完是反方向，原因暂未找出）
        moveRes = transform.position - transform.TransformPoint(moveRes);
        //叠加速度
        moveRes.x *= speed.x;
        moveRes.z *= speed.y;
        // moveRes.y = rb.velocity.y;
        return;
    }

    /// <summary>
    /// 移动端移动输入
    /// </summary>
    /// <param name="callbackContext"></param>
    public void GetPlayerInput_StickMove(Vector2 newPos)
    {
        if (magicBallStart) return;
        Vector2 movement = newPos;
        moveRes.x = movement.x;
        moveRes.y = rb.velocity.y;
        moveRes.z = movement.y;
        if (moveRes.x == 0 && moveRes.z == 0)
        {
            return;
        }
        //用以自己为参考的目标点的世界坐标减去自己的世界坐标（但是操作完是反方向，原因暂未找出）
        moveRes = transform.position - transform.TransformPoint(moveRes);
        //叠加速度
        moveRes.x *= speed.x;
        moveRes.z *= speed.y;
        // moveRes.y = rb.velocity.y;
        return;
    }

    /// <summary>
    /// 移动端获取角色移动时的旋转结果
    /// </summary>
    public void GetPlayerInput_StickMoveRotate(Vector2 newPos)
    {
        if (newPos.x != 0)
        {
            //修改移动时的旋转
            scaleRes_Move = newPos.x > 0 ? -1 : 1;
        }
    }

    /// <summary>
    /// 移动端朝向输入
    /// </summary>
    /// <param name="callbackContext"></param>
    public void GetPlayerInput_StickRotate(Vector2 newPos)
    {
        Vector2 temp = newPos;
        //操作方案1 增量式位移
        // temp *= Time.deltaTime * rightStickMoveSpeed;
        // temp += rightStickPos;
        // rightStickPos.x = Mathf.Min(1920, temp.x);
        // rightStickPos.x = Mathf.Max(0, temp.x);
        // rightStickPos.y = Mathf.Min(1080, temp.y);
        // rightStickPos.y = Mathf.Max(0, temp.y);

        //操作方案2 位置式位移
        temp /= 2;
        temp += Vector2.one / 2;
        temp.x *= 1920;
        temp.y *= 1080;
        rightStickPos = temp;
    }

    /// <summary>
    /// 获取并调整角色的光圈朝向结果
    /// 获取角色朝向鼠标时的缩放
    /// </summary>
    public void GetPlayerInput_MouseRotate()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
#elif UNITY_ANDROID
        Ray ray = Camera.main.ScreenPointToRay(rightStickPos);
#endif
        //射线碰撞信息
        RaycastHit groundHit;
        if (Physics.Raycast(ray, out groundHit, 2000, LayerMask.GetMask("Ground")))
        {
            targetPoint = groundHit.point;  //获取角色朝向，用于控制朝向光圈
            scaleRes_Mouse = Vector3.Dot(transform.right, groundHit.point - transform.position) > 0 ? 1 : -1;
        }
        targetPoint.y = orientationObject.transform.position.y;
        // TestCube.transform.position = targetPoint;
        orientationObject.LookAt(targetPoint, Vector3.up);
        gunControl.ModeLookAt(targetPoint, weaponType);
    }

    /// <summary>
    /// 获取角色移动时的旋转结果
    /// </summary>
    public void GetPlayerInput_MoveRotate(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.ReadValue<Vector2>().x != 0)
        {
            //修改移动时的旋转
            scaleRes_Move = callbackContext.ReadValue<Vector2>().x > 0 ? -1 : 1;
        }
    }

    /// <summary>
    ///  获取角色的攻击输入
    /// </summary>
    public void GetPlayerInput_Attack(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (weaponType != E_WeaponType.sword)
            {
                SetWeaponType(E_WeaponType.sword);
            }
            else if (animator.GetBool("canAttack"))  //只有可攻击时才能获取按键输入
            {
                animator.SetBool("attack", true);
                return;
            }
        }
        animator.SetBool("attack", false);
    }

    /// <summary>
    /// 获取角色的射击输入
    /// </summary>
    public void GetPlayerInput_ShotAttack(InputAction.CallbackContext context)
    {
        if (gunBuffLevel == 0) return;
        if (context.phase == InputActionPhase.Performed)
        {
            if (weaponType != E_WeaponType.gun)
            {
                SetWeaponType(E_WeaponType.gun);
            }
            canShot = true;
            GameManager.Instance.SetMouse_Shot();
            return;
        }
        GameManager.Instance.SetMouse_Pointer();
        canShot = false;
    }

    /// <summary>
    /// 获取角色的法术攻击输入
    /// </summary>
    public void GetPlayerInput_StaffAttack(InputAction.CallbackContext context)
    {
        if (staffBuffLevel == 0) return;
        if (context.phase == InputActionPhase.Started)
        {
            if (weaponType != E_WeaponType.staff)
            {
                SetWeaponType(E_WeaponType.staff);
                magicBallStart = false;
                curHoldTime = 0;
            }
            moveRes = Vector3.zero;
            // Debug.Log("准备释放");
            CM_Effect.Instance.PlayerGetDamaged(Color.black, 10, 0.6f);
            if (staffBuffLevel == 4)
            {
                characterData.currentDefend = 5;
            }
            SetUseMouseScale(true);
            magicBallStart = true;
            EffectManager.Instance.playerMagicRange.gameObject.SetActive(true);
            EffectManager.Instance.playerMagicRange.GetComponent<AreaControl>().IsBuffed = staffBuffLevel >= 2 ? true : false;  //是否满足降低蓄力时间的条件
            GameManager.Instance.SetMouse_Shot();
            FmodManager.Instance.PlaySpecialSound("event:/Player/Zhang/fireBallBefore");
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            // Debug.Log("松开蓄力");
            if (weaponType == E_WeaponType.staff && curHoldTime >= staffHoldTime)
            {
                CM_Effect.Instance.PlayerGetDamaged(Color.white, 8, 0.6f);
                magicBallStart = false; //取消长按连发
                // Debug.Log("发射法球");
                gunControl.StaffModeShot(targetPoint);
            }
            ExitStaffMode();
            FmodManager.Instance.PauseSpecialSound("event:/Player/Zhang/fireBallBefore");
        }
    }

    /// <summary>
    /// 退出法术模式
    /// </summary>
    public void ExitStaffMode()
    {
        SetUseMouseScale(false);
        characterData.currentDefend = 0;
        magicBallStart = false;
        curHoldTime = 0;
        EffectManager.Instance.playerMagicRange.gameObject.SetActive(false);
        GameManager.Instance.SetMouse_Pointer();
    }

    /// <summary>
    /// 角色闪避输入获取
    /// </summary>
    public void GetPlayerInput_Dodge(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (dodgeCD.flag)
            {
                dodgeCount = characterBuffManager.GetDogeTimes();
                // Debug.Log("冲刺");
                if ((animator.GetBool("canDodge")))
                {
                    animator.SetTrigger("dodge");
                    dodgeCount--;
                    dodgeCD.flag = false;
                    dodgeCD.curTime = 0;
                }
            }
            else if (dodgeCount > 0 && animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Dodge"))
            {
                animator.SetTrigger("dodge");
                dodgeCount--;
                dodgeCD.flag = false;
                dodgeCD.curTime = 0;
            }
        }
    }
    #endregion

    #region 控制相关
    /// <summary>
    /// 自己的三维向量差值函数
    /// 按帧插值
    /// 本项目只使用xz轴，所以不对y轴差值
    /// </summary>
    /// <param name="cur"> 当前值 </param>
    /// <param name="tar"> 目标值 </param>
    /// <param name="value"> 差值系数 </param>
    /// <returns></returns>
    public void V3Lerp(ref Vector3 cur, Vector3 tar, float value)
    {
        cur.x = Mathf.Lerp(cur.x, tar.x, value * Time.deltaTime);
        cur.z = Mathf.Lerp(cur.z, tar.z, value * Time.deltaTime);
    }

    /// <summary>
    /// 角色Move运动控制
    /// 改变角色velocity
    /// </summary>
    public void PlayerBaseMove(float velocityLerpValue)
    {
        if (magicBallStart) return;
        Vector3 lerpVelocity = rb.velocity;
        V3Lerp(ref lerpVelocity, moveRes, velocityLerpValue);
        animator.SetFloat("speed", lerpVelocity.magnitude);
        rb.velocity = lerpVelocity;
    }

    /// <summary>
    /// 角色运动逐渐停止运动
    /// </summary>
    public void PlayerStopMove(float StopLerpValue)
    {
        Vector3 lerpVelocity = rb.velocity;
        V3Lerp(ref lerpVelocity, Vector3.zero, StopLerpValue);
        rb.velocity = lerpVelocity;
    }

    /// <summary>
    /// 切换是否使用鼠标朝向
    /// </summary>
    /// <param name="_useMouseScale"> 是或否 </param>
    public void SetUseMouseScale(bool _useMouseScale)
    {
        useMouseScale = _useMouseScale;
    }

    /// <summary>
    /// 移动改变角色LocalScale（左右朝向）
    /// 用于移动和冲刺时的面向调整
    /// </summary>
    public void PlayerBaseRotate_Move()
    {
        if (!useMouseScale)
        {
            skeletal.localScale = new Vector3(scaleRes_Move, 1, 1);   //调整朝向
        }
    }

    /// <summary>
    /// 攻击改变角色LocalScale（左右朝向）
    /// 用于攻击时的面向调整
    /// </summary>
    public void PlayerBaseRotate_Attack()
    {
        scaleRes_Move = scaleRes_Mouse;
        skeletal.localScale = new Vector3(scaleRes_Mouse, 1, 1);
    }

    /// <summary>
    /// 角色基础闪避运动
    /// </summary>
    /// <param name="dodgePower"> 闪避速度 </param>
    public void PlayerBaseMove_Dodge(float dodgePower)
    {
        PlayerBaseMove_ForceMove(dodgePower);
        //在骨架中心位置显示特效
        EffectManager.Instance.SetDashEffect(transform.position + effectDashPos, rb.velocity);
        FMODUnity.RuntimeManager.PlayOneShot("event:/Player/shift");
    }

    /// <summary>
    /// 角色强制移动
    /// </summary>
    /// <param name="movePower"> 强制移动速度 </param>
    /// <param name="useKeyBord"> 是否使用键盘控制方向 </param>
    public void PlayerBaseMove_ForceMove(float movePower)
    {
        Vector3 plusVelocity;
        rb.velocity = Vector3.zero;
        if (moveRes.magnitude > 3.0f)
        {
            PlayerBaseRotate_Move();
            plusVelocity = moveRes.normalized * movePower;
            rb.velocity = new Vector3(plusVelocity.x, rb.velocity.y, plusVelocity.z);
        }
        else
        {
            SetUseMouseScale(true);
            PlayerBaseRotate_Attack();
            SetUseMouseScale(false);
            plusVelocity = (targetPoint - transform.position).normalized * movePower;
            rb.velocity = new Vector3(plusVelocity.x, rb.velocity.y, plusVelocity.z);
        }
    }

    public void PlayerForceMove(Vector3 dir)
    {
        rb.velocity = new Vector3(dir.x, rb.velocity.y, dir.z);
    }

    /// <summary>
    /// 设置武器类型
    /// </summary>
    public void SetWeaponType(E_WeaponType targetType)
    {
        if (gunControl.ModeChangeOver != 0) return;
        if (targetType == E_WeaponType.sword)
        {
            ExitStaffMode();
            weaponType = E_WeaponType.sword;
            canShot = false;
        }
        else if (targetType == E_WeaponType.gun)
        {
            ExitStaffMode();
            weaponType = E_WeaponType.gun;
        }
        else
        {
            weaponType = E_WeaponType.staff;
            canShot = false;
        }
        gunControl.ChangeMode(targetType);  //切换浮游炮模式
    }

    /// <summary>
    /// 切换连招到下一个node
    /// </summary>
    public void ChangeCombo(ComboNode nextCombo)
    {
        currentComboNode = nextCombo;
        // Debug.Log("切换到" + currentComboNode);
    }
    #endregion

    #region 战斗相关
    /// <summary>
    /// 开启触发器
    /// </summary>
    public void OpenTrigger()
    {
        comboTrigger.SetActive(true);
        // Debug.Log("显示");
    }
    /// <summary>
    /// 生成攻击特效
    /// </summary>
    public void CreateEffect()
    {
        EffectManager.Instance.SetAttackEffect(currentComboNode.attackEffect, comboTrigger.transform.position + effectAtkPos, comboTrigger.transform.rotation);
        FMODUnity.RuntimeManager.PlayOneShot(currentComboNode.attackSound);
    }
    /// <summary>
    /// 关闭触发器
    /// </summary>
    public void CloseTrigger()
    {
        comboTrigger.SetActive(false);
        // Debug.Log("关闭");
    }
    /// <summary>
    /// 角色索敌突进
    /// 基于当前ComboNode判断
    /// </summary>
    public void PlayerAttackMove_Plunge()
    {
        //获取朝向向量（目标点-中心点获取向量）归一化后乘以z方向长度拉伸向量
        Vector3 dir = targetPoint - orientationObject.position;
        dir.y = 0;   //消除俯仰角
        Vector3 dirNormal = dir.normalized; //保存朝向向量的归一化结果
        dir = dirNormal * currentComboNode.halfPlungeBoxSize.z;
        //获取中心位置（可想象成通过将中心位置向朝向向量移动后的位置就是中心位置）
        dir = new Vector3(orientationObject.position.x + dir.x, transform.position.y, orientationObject.position.z + dir.z);
        Collider[] colliders = Physics.OverlapBox(dir, currentComboNode.halfPlungeBoxSize, orientationObject.transform.rotation, LayerMask.GetMask("EmyBody"));

        // TestCube.transform.position = dir;
        // TestCube.transform.localScale = currentComboNode.halfPlungeBoxSize * 2;
        // TestCube.transform.rotation = orientationObject.transform.rotation;
        //修改Tigger的尺寸，dir没用了可以在这里复用
        dir = dirNormal * (currentComboNode.attackRange.z / 2 + currentComboNode.attackRangeDeviation);
        dir = new Vector3(orientationObject.position.x + dir.x, transform.position.y, orientationObject.position.z + dir.z);
        comboTrigger.transform.position = dir;
        comboTrigger.transform.localScale = currentComboNode.attackRange;
        comboTrigger.transform.rotation = orientationObject.transform.rotation;
        //如果没有就不追踪
        if (colliders.Length == 0)
        {
            if (moveRes.magnitude > 3.0f)
            {
                // Debug.Log("没有搜索到敌人，键盘方向索敌");
                // rb.velocity = Vector3.zero;
                dir = moveRes.normalized * currentComboNode.plungePower;
                rb.velocity = new Vector3(dir.x * currentComboNode.forceSpeed.x, rb.velocity.y, dir.z * currentComboNode.forceSpeed.y);
            }
            else
            {
                // Debug.Log("没有搜索到敌人，鼠标方向索敌");
                // rb.velocity = Vector3.zero;
                dir = (targetPoint - transform.position).normalized * currentComboNode.plungePower;
                rb.velocity = new Vector3(dir.x * currentComboNode.forceSpeed.x, rb.velocity.y, dir.z * currentComboNode.forceSpeed.y);
            }
        }
        else
        {
            dir = colliders[0].transform.position - orientationObject.position;    //重复利用变量
            dir.y = 0;
            float minDistence = dir.magnitude;
            foreach (Collider c in colliders)
            {
                dir = c.transform.position - orientationObject.position;
                dir.y = 0;
                if (dir.magnitude < minDistence)
                {
                    minDistence = dir.magnitude;
                    colliders[0] = c;
                }
            }
            dir = colliders[0].transform.position - orientationObject.position;    //重复利用变量
            dir.y = 0;
            //如果太近就不追踪，进入攻击距离不在追击
            if (dir.magnitude > currentComboNode.attackRange.z)
            {
                //追击敌人速度，越远速率越高
                // Debug.Log("搜索到敌人，并且距离较远，索敌");
                // rb.velocity = Vector3.zero;
                rb.velocity = dir * currentComboNode.plungePower;
            }
            else
            {
                // Debug.Log("搜索到敌人，并且距离不远，不索敌");
            }
        }
    }

    /// <summary>
    /// 重攻击逻辑
    /// </summary>
    private void PlayerAttack_Heavy(InputActionPhase phase)
    {
        if (phase == InputActionPhase.Started)
        {
            // Debug.Log("重攻击");
            animator.SetTrigger("heavyAttack");
        }
        else
        {
            // Debug.Log("清除重攻击");
            animator.ResetTrigger("heavyAttack");
        }
    }

    /// <summary>
    /// 枪攻击逻辑
    /// </summary>
    private void PlayerAttack_Gun()
    {
        // if (canShot && animator.GetBool("canAttack"))
        if (canShot)
        {
            gunControl.GunModeShot(targetPoint);
        }
    }

    /// <summary>
    /// 杖攻击逻辑
    /// </summary>
    private void PlayerAttack_Staff()
    {
        //进入不可攻击状态时重置蓄力时间
        //适用于冲刺打断
        // if (!animator.GetBool("canAttack") && staffBuffLevel < 4)
        // {
        //     ExitStaffMode();
        // }
        if (magicBallStart)
        {

            curHoldTime += Time.deltaTime;
            if (curHoldTime > staffHoldTime)
            {
                curHoldTime = staffHoldTime;
            }
            EffectManager.Instance.playerMagicRange.position = targetPoint;
            //朝向鼠标方向
            PlayerBaseRotate_Attack();
            //蓄力中无法移动
            PlayerStopMove(staffStopLerp);
        }
    }

    /// <summary>
    /// 根据玩家连击数提升玩家攻击倍率
    /// </summary>
    /// <param name="maxMagnification"> 最大倍率 </param>
    /// <param name="stepMagnification"> 每连击增加的倍率 </param>
    public void SetCurrentComboAttack(float maxMagnification, float stepMagnification)
    {
        if (swordBuffLevel == 4)
        {
            characterData.currentComboAttack = currentComboNode.baseDamage * (Mathf.Min(maxMagnification, 1 + GameManager.Instance.CurrentComboCount * stepMagnification));
        }
        else
        {
            characterData.currentComboAttack = currentComboNode.baseDamage;
        }
    }

    /// <summary>
    /// 根据玩家连击数提升玩家攻击倍率
    /// </summary>
    /// <param name="maxMagnification"> 最大倍率 </param>
    /// <param name="stepMagnification"> 每连击增加的倍率 </param>
    public void SetSpecifyComboAttack(E_WeaponType weaponType, float maxMagnification, float stepMagnification)
    {
        if (weaponType == E_WeaponType.gun)
        {
            if (swordBuffLevel == 4)
            {
                characterData.currentComboAttack = shotComboNode.baseDamage * (Mathf.Min(maxMagnification, 1 + GameManager.Instance.CurrentComboCount * stepMagnification));
            }
            else
            {
                characterData.currentComboAttack = shotComboNode.baseDamage;
            }
        }
        else if (weaponType == E_WeaponType.staff)
        {
            if (swordBuffLevel == 4)
            {
                characterData.currentComboAttack = magicComboNode.baseDamage * (Mathf.Min(maxMagnification, 1 + GameManager.Instance.CurrentComboCount * stepMagnification));
            }
            else
            {
                characterData.currentComboAttack = magicComboNode.baseDamage;
            }
        }
    }

    /// <summary>
    /// 攻击判定
    /// </summary>
    /// <param name="other"> 被打击物体 </param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "EmyBody")
        {
            // Debug.Log("攻击命中");
            SetCurrentComboAttack(2f, 0.1f);
            GameManager.Instance.PlayerAttack(other.transform.parent.GetComponent<BaseEnemyControl>(), transform.position);
            GameManager.Instance.Player_StartHitEffect();   //开始打击感流程
        }
        if (other.tag == "BossBody")
        {
            SetCurrentComboAttack(2, 0.1f);
            GameManager.Instance.PlayerAttack(other.GetComponent<BossControl>());
            GameManager.Instance.Player_StartHitEffect();   //开始打击感流程
        }
    }

    /// <summary>
    /// 暂停动画 卡顿效果
    /// </summary>
    /// <param name="pauseSpeed"> 期望动画播放速度 </param>
    public void PauseAnimation(float pauseSpeed)
    {
        animator.speed = pauseSpeed;
    }

    /// <summary>
    /// 恢复动画 卡顿效果
    /// </summary>
    public void ContinueAnimation()
    {
        animator.speed = 1.0f;
    }
    #endregion

    #region 测试相关
    /// <summary>
    /// 屏蔽输入
    /// </summary>
    public void DisableInput()
    {
        GetComponent<PlayerInput>().enabled = false;
    }

    /// <summary>
    /// 恢复输入
    /// </summary>
    public void EnableInput()
    {
        GetComponent<PlayerInput>().enabled = true;
        GetComponent<PlayerInput>().actions.actionMaps[0].Enable();
        GetComponent<PlayerInput>().actions.actionMaps[0].FindAction("Move").Enable();
    }

    /// <summary>
    /// 重新加载Buff相关数据
    /// </summary>
    private void RebuildBuffLevel()
    {
        swordBuffLevel = characterBuffManager.PlayerSwordLevel();
        swordBuffTimes = characterBuffManager.PlayerSwordTimes();
        animator.SetInteger("swordLevel", swordBuffLevel);
        animator.SetInteger("swordTimes", swordBuffTimes);

        gunBuffLevel = characterBuffManager.PlayerGunBuffLevel();
        gunControl.gunBuffLevel = gunBuffLevel;

        staffBuffLevel = characterBuffManager.PlayerStaffBuffLevel();
        dodgeCount = characterBuffManager.GetDogeTimes();
    }

    /// <summary>
    /// 重新加载玩家Buff
    /// </summary>
    public void PlayerBuffRebuild(List<I_BuffBase> newBuffList)
    {
        if (BuffDataManager.Instance != null)
        {
            characterBuffManager.BuffReBuild(newBuffList, this.gameObject);
            RebuildBuffLevel();
        }
    }


    /// <summary>
    /// 角色死亡
    /// </summary>
    public void PlayerDie()
    {
        isDead = true;
        DisableInput(); //屏蔽输入
        ContinueAnimation();
        animator.SetTrigger("die");
    }

    /// <summary>
    /// 角色收到攻击
    /// </summary>
    public void GetDamage()
    {
        if (!characterBuffManager.HasShield())
        {
            ContinueAnimation();
            animator.SetTrigger("hurt");
        }
    }

    /// <summary>
    /// 按下Q键后的操作测试
    /// </summary>
    public void GetInputKey_Q(InputAction.CallbackContext context)
    {
        PlayerAttack_Heavy(context.phase);
        if (context.phase == InputActionPhase.Performed)
        {
        }
    }

    /// <summary>
    /// 按下E键后的操作测试
    /// </summary>
    public void GetInputKey_E(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            // CM_Effect.Instance.CM_TransitionDim(8, 1.2f);
        }
    }

    /// <summary>
    /// 按下R键后的操作测试
    /// </summary>
    public void GetInputKey_R(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            // SetWeaponType(E_WeaponType.staff);
        }
    }
    #endregion
}
