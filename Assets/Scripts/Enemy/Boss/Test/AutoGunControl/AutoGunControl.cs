using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoGunControl : MonoBehaviour
{
    private Transform[] gunBody = new Transform[4]; //浮游炮体，控制炮体旋转
    private Transform gunBodyTF;
    private Transform[] normalModePos = new Transform[4];
    private Transform[] swordSkillModePos = new Transform[4];
    private GameObject gun01ModeGO;
    private Transform[] gun01ModePos = new Transform[4];
    private GameObject gun02ModeGO;
    private Transform[] gun02ModePos = new Transform[4];
    private Transform[] staffModePos = new Transform[4];
    private Transform[] changeStageModePos = new Transform[4];
    private E_BossAttackMode bossAttackMode;
    private bool lockShotDir;   //是否锁定方向
    private int modeChangeOver = 0; //模式切换进度，为0切换完毕
    public int ModeChangeOver { get { return modeChangeOver; } }
    [SerializeReference] public BossControl bossControl;    //Boss脚本引用，用于设置一些动画状态机中状态
    [Header("剑模式转向速度")] public float swordRSpeed = 0.2f;
    [Header("枪模式转向速度")] public float gunRSpeed = 0.05f;

    [Header("枪攻击模式1瞄准时间")] public float gunMode1AimingTime = 1.5f;
    [Header("枪攻击模式1范围提示时间")] public float gunMode1HintTime = 1.0f;
    [Header("枪攻击模式1射击次数")] public int gun1ShotCount = 10;
    [Header("枪攻击模式1射击间隔")] public float gun1ShotInterval = 0.2f;

    [Header("枪攻击模式2瞄准时间")] public float gunMode2AimingTime = 2.0f;
    [Header("枪攻击模式2范围提示时间")] public float gunMode2HintTime = 1.0f;
    [Header("枪攻击模式2射击次数")] public int gun2ShotCount = 20;
    [Header("枪攻击模式2转速")] public float gun2ShotRotateSpeed = 5.0f;
    [Header("枪攻击模式2射击间隔")] public float gun2ShotInterval = 0.2f;

    [Header("杖攻击模式瞄准时间")] public float staffModeAimingTime = 4.0f;
    [Header("杖攻击模式射击次数")] public int staffShotCount = 1;
    [Header("杖攻击模式射击间隔")] public float staffModeShotInterval = 0.75f;
    [Header("杖攻击模式随机范围")] public float staffModeShotRange = 5.0f;

    [Header("杖模式转向速度")] public float staffRSpeed = 0.1f;
    [Header("模式切换移动速度")] public float modeTranstionSpeed = 5f;
    [Header("模式切换旋转速度")] public float modeTranstionRSpeed = 0.2f;

    /// <summary>
    /// 初始化浮游炮
    /// </summary>
    public void AutoGunInit()
    {
        gunBodyTF = transform.Find("Rotate").Find("GunBodys");
        //获取攻击范围 先隐藏
        gun01ModeGO = transform.Find("Rotate").Find("Gun01ModePos").gameObject;
        gun01ModeGO.SetActive(false);
        gun02ModeGO = transform.Find("Rotate").Find("Gun02ModePos").gameObject;
        gun02ModeGO.SetActive(false);
        for (int i = 0; i < gunBody.Length; i++)
        {
            gunBody[i] = gunBodyTF.GetChild(i);
            normalModePos[i] = transform.Find("Rotate").Find("NormalModePos").GetChild(i);
            swordSkillModePos[i] = transform.Find("Rotate").Find("SwordSkillModePos").GetChild(i);
            gun01ModePos[i] = gun01ModeGO.transform.GetChild(i);
            gun02ModePos[i] = gun02ModeGO.transform.GetChild(i);
            staffModePos[i] = transform.Find("Rotate").Find("StaffModePos").GetChild(i);
            changeStageModePos[i] = transform.Find("Rotate").Find("ChangeStageModePos").GetChild(i);
        }
    }
    #region 基础控制
    /// <summary>
    /// 根据模式按照不同速率调整浮游炮朝向
    /// </summary>
    /// <param name="targetPoint"> 目标点 </param>
    /// <param name="_BossAttackMode"> 攻击类型 </param>
    public void ModeLookAt(Vector3 targetPoint, E_BossAttackMode _BossAttackMode)
    {
        if (lockShotDir) return;
        if (_BossAttackMode == E_BossAttackMode.normal)
        {
            LookAt(targetPoint, swordRSpeed);
        }
        else if (_BossAttackMode == E_BossAttackMode.swordSkill)
        {
            LookAt(targetPoint, swordRSpeed);
        }
        else if (_BossAttackMode == E_BossAttackMode.gun01)
        {
            LookAt(targetPoint, gunRSpeed);
        }
        else if (_BossAttackMode == E_BossAttackMode.gun02)
        {
            LookAt(targetPoint, gunRSpeed);
        }
        else if (_BossAttackMode == E_BossAttackMode.staff)
        {
            LookAt(targetPoint, staffRSpeed);
        }
    }

    /// <summary>
    /// 转向
    /// </summary>
    /// <param name="targetPoint">目标点</param>
    /// <param name="rSpeed"> 旋转速度 </param>
    /// <param name="bodyRotate"> 是否需要子浮游炮朝向目标点 </param>
    private void LookAt(Vector3 targetPoint, float rSpeed)
    {
        if (modeChangeOver != 0) return; //说明模式没有切换完成
        targetPoint.y = transform.position.y;
        Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position, Vector3.up);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, rSpeed);
    }

    /// <summary>
    /// 切换武器模式
    /// </summary>
    /// <param name="_weaponType"> 目标武器模式 </param>
    public void ChangeMode(E_BossAttackMode _bossAttackMode)
    {
        bossAttackMode = _bossAttackMode;
        StopAllCoroutines();
        modeChangeOver = 0;
        if (bossAttackMode == E_BossAttackMode.normal)
        {
            for (int i = 0; i < gunBody.Length; i++)
            {
                StartCoroutine(IE_GunTransform(gunBody[i], normalModePos[i]));
                modeChangeOver++;
            }
        }
        else if (bossAttackMode == E_BossAttackMode.swordSkill)
        {
            for (int i = 0; i < gunBody.Length; i++)
            {
                StartCoroutine(IE_GunTransform(gunBody[i], swordSkillModePos[i]));
                modeChangeOver++;
            }
        }
        else if (bossAttackMode == E_BossAttackMode.gun01)
        {
            for (int i = 0; i < gunBody.Length; i++)
            {
                StartCoroutine(IE_GunTransform(gunBody[i], gun01ModePos[i]));
                modeChangeOver++;
            }
        }
        else if (bossAttackMode == E_BossAttackMode.gun02)
        {
            for (int i = 0; i < gunBody.Length; i++)
            {
                StartCoroutine(IE_GunTransform(gunBody[i], gun02ModePos[i]));
                modeChangeOver++;
            }
        }
        else if (bossAttackMode == E_BossAttackMode.staff)
        {
            for (int i = 0; i < gunBody.Length; i++)
            {
                StartCoroutine(IE_GunTransform(gunBody[i], staffModePos[i]));
                modeChangeOver++;
            }
        }
    }

    /// <summary>
    /// 浮游炮进入转阶段模式
    /// </summary>
    public void ChangeMode_ChangeStage()
    {
        StopAllCoroutines();
        gun01ModeGO.SetActive(false);
        gun02ModeGO.SetActive(false);
        lockShotDir = false;    //允许朝向玩家（有种躲避的感觉）
        modeChangeOver = 0;
        for (int i = 0; i < gunBody.Length; i++)
        {
            StartCoroutine(IE_GunTransform(gunBody[i], changeStageModePos[i]));
            modeChangeOver++;
        }
    }

    /// <summary>
    /// 浮游炮进入死亡状态
    /// </summary>
    public void ChangeMode_Dead()
    {
        Debug.Log("浮游炮进入死亡状态");
    }
    #endregion

    #region 攻击相关
    /// <summary>
    /// 枪模式攻击
    /// </summary>
    /// <param name="type"> 攻击模式1或2 </param>
    public void GunAttack(int type)
    {
        if (type == 1)  //攻击模式1
        {
            StartCoroutine(IE_GunAttack01());
        }
        else        //攻击模式2
        {
            StartCoroutine(IE_GunAttack02());
        }
    }

    /// <summary>
    /// 杖模式攻击
    /// </summary>
    public void StaffAttack()
    {
        StartCoroutine(IE_StaffAttack());
    }

    /// <summary>
    /// 枪攻击模式01协程
    /// </summary>
    IEnumerator IE_GunAttack01()
    {
        float time = 0;
        //显示蓄力 追踪玩家
        while (time < gunMode1AimingTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        time = 0;
        bossControl.BossAttack_Gun_End(); //关闭射击吟唱特效
        //锁定方向
        // lockShotDir = true;
        //显示提示范围
        gun01ModeGO.SetActive(true);
        while (time < gunMode1HintTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        time = 0;
        //关闭提示范围 && 发射
        gun01ModeGO.SetActive(false);
        for (int i = 0; i < gun1ShotCount; i++)
        {
            for (int j = 0; j < gunBody.Length; j++)
            {
                EffectManager.Instance.Boss_SetBullet(gunBody[j], bossControl);
                FMODUnity.RuntimeManager.PlayOneShot("event:/BOSS/gunFire");
            }
            while (time < gun1ShotInterval)  //等待发射完成
            {
                time += Time.deltaTime;
                yield return null;
            }
            time = 0;
        }
        while (time < 1.0f)  //等待发射完成
        {
            time += Time.deltaTime;
            yield return null;
        }
        time = 0;
        //发射完毕
        // lockShotDir = false;
        bossControl.SetAnimatorBool("attacking", false);
        bossControl.SetAnimatorBool("move", true);
        bossControl.bossCD.canGunAttack1.flag = false;
        yield break;
    }

    /// <summary>
    /// 枪攻击模式02协程
    /// </summary>
    IEnumerator IE_GunAttack02()
    {
        float time = 0;
        //锁定方向
        lockShotDir = true;
        //显示蓄力
        while (time < gunMode2AimingTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        time = 0;
        bossControl.BossAttack_Gun_End(); //关闭射击吟唱特效
        //显示提示范围
        gun02ModeGO.SetActive(true);
        while (time < gunMode2HintTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        time = 0;
        //关闭提示范围 && 发射
        gun02ModeGO.SetActive(false);
        for (int i = 0; i < gun2ShotCount; i++)
        {
            for (int j = 0; j < gunBody.Length; j++)
            {
                EffectManager.Instance.Boss_SetBullet(gunBody[j], bossControl);
                FMODUnity.RuntimeManager.PlayOneShot("event:/BOSS/gunFire");
            }
            while (time < gun2ShotInterval)  //等待发射完成
            {
                time += Time.deltaTime;
                gunBodyTF.Rotate(Vector3.up * gun2ShotRotateSpeed * Time.deltaTime, Space.World);
                yield return null;
            }
            time = 0;
        }
        while (time < 1.0f)  //等待发射完成
        {
            time += Time.deltaTime;
            yield return null;
        }
        time = 0;
        //发射完毕
        lockShotDir = false;
        bossControl.SetAnimatorBool("attacking", false);
        bossControl.SetAnimatorBool("move", true);
        bossControl.bossCD.canGunAttack2.flag = false;
        yield break;
    }

    /// <summary>
    /// 杖攻击模式协程
    /// </summary>
    IEnumerator IE_StaffAttack()
    {
        PlayerControl curPlayer = GameManager.Instance.currentPlayer; //获取玩家引用
        Vector3 shotPostion;
        float time = 0;
        //锁定方向
        lockShotDir = true;
        //显示蓄力
        for (int j = 0; j < gunBody.Length; j++)
        {
            EffectManager.Instance.Boss_SetMagic_Start(gunBody[j]);
        }
        while (time < staffModeAimingTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        time = 0;
        bossControl.BossAttack_Staff_End(); //关闭法术吟唱特效
        //发射
        for (int i = 0; i < staffShotCount; i++)
        {
            for (int j = 0; j < gunBody.Length; j++)
            {
                shotPostion = curPlayer.transform.position + new Vector3(Random.Range(-staffModeShotRange, staffModeShotRange), 0, Random.Range(-staffModeShotRange, staffModeShotRange));
                EffectManager.Instance.Boss_SetMagic_Shot(shotPostion, bossControl);
                // Debug.Log("发射一个法术");
                while (time < staffModeShotInterval)  //等待发射完成
                {
                    time += Time.deltaTime;
                    yield return null;
                }
                time = 0;
            }
        }
        //等待发射完成
        while (time < 1.0f)
        {
            time += Time.deltaTime;
            yield return null;
        }
        time = 0;
        //发射完毕
        lockShotDir = false;
        bossControl.SetAnimatorBool("attacking", false);
        bossControl.SetAnimatorBool("move", true);
        bossControl.bossCD.canStaffAttack.flag = false;
        yield break;
    }

    #endregion

    /// <summary>
    /// 停止各种状态，让浮游炮进入转阶段状态
    /// </summary>
    public void my_StopAllCoroutines()
    {
        StopAllCoroutines();
        ChangeMode_ChangeStage();
    }

    /// <summary>
    /// 重置各种状态
    /// </summary>
    public void ResetStates()
    {
        ChangeMode(bossAttackMode);
    }

    /// <summary>
    /// 协程控制模式转换时的浮游炮位移效果
    /// </summary>
    /// <param name="gun"> 需要位移的浮游炮 </param>
    /// <param name="target"> 目标位置 </param>
    /// <returns></returns>
    IEnumerator IE_GunTransform(Transform gun, Transform target)
    {
        Vector3 dir = gun.transform.position - target.position;
        float dis = dir.magnitude;
        //先调整角度
        while (Mathf.Abs(Quaternion.Dot(gun.rotation, target.rotation)) < 0.95f)
        {
            // Debug.Log("正在旋转");
            gun.rotation = Quaternion.Lerp(gun.rotation, target.rotation, modeTranstionRSpeed);
            yield return null;
        }
        gun.rotation = target.rotation;
        //在调整位置
        while (dis > 0.1f)
        {
            // Debug.Log("正在位移");
            gun.Translate(dir * modeTranstionSpeed * Time.deltaTime, Space.World);
            dir = target.position - gun.transform.position;
            dis = dir.magnitude;
            yield return null;
        }
        modeChangeOver--;
        yield break;
    }
}
