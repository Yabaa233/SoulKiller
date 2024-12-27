using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunControl : MonoBehaviour
{
    private Transform[] gunBody = new Transform[4]; //浮游炮体，控制炮体旋转
    private Transform staffShotPoint;   //法球发射点
    private Transform[] swordModePos = new Transform[4];
    private Transform[] gunModePos = new Transform[4];
    private Transform[] staffModePos = new Transform[4];
    private E_WeaponType weaponType;
    private int modeChangeOver = 0;
    public int ModeChangeOver { get { return modeChangeOver; } }
    public int gunBuffLevel;    //枪Buff等级
    [Tooltip("最大子弹数")] public float maxAmmunition = 60; //最大子弹数
    //TODO:因为此处是float，所以UI上显示要显示指定类型为int，或者使用能量形式表现
    [Tooltip("当前子弹数")] public float curAmmunition = 60;  //当前子弹数
    [Tooltip("子弹射击CD")] public CDClass gunShotCD = new CDClass(); //射击CD
    [Tooltip("法球尺寸")] public float magicBallSize; //法球尺寸
    [Tooltip("是否正在装弹")] public bool isReloading;  //是否正在装填子弹
    [Tooltip("子弹自动装填速度")] public float autoReloadSpeed; //自动装填速度
    [Tooltip("子弹手动装填速度")] public float manualReloadSpeed;   //手动装填速度
    [Header("剑模式转向速度")] public float swordRSpeed = 0.2f;
    [Header("枪模式转向速度")] public float gunRSpeed = 0.05f;
    [Header("杖模式转向速度")] public float staffRSpeed = 0.1f;
    [Header("模式切换移动速度")] public float modeTranstionSpeed = 1.5f;
    [Header("模式切换旋转速度")] public float modeTranstionRSpeed = 0.1f;

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        GameManager.Instance.CDList.Add(gunShotCD);
        gunShotCD.flag = true;
        staffShotPoint = transform.Find("Rotate").Find("StaffShotPoint");
        for (int i = 0; i < gunBody.Length; i++)
        {
            gunBody[i] = transform.Find("Rotate").Find("GunBodys").GetChild(i);
            swordModePos[i] = transform.Find("Rotate").Find("SwordModePos").GetChild(i);
            gunModePos[i] = transform.Find("Rotate").Find("GunModePos").GetChild(i);
            staffModePos[i] = transform.Find("Rotate").Find("StaffModePos").GetChild(i);
        }
    }

    private void Update()
    {
        if (weaponType != E_WeaponType.gun && curAmmunition < maxAmmunition) //换武器自动装填子弹
        {
            //print("自动换弹中");
            curAmmunition = Mathf.Min(curAmmunition + Time.deltaTime * autoReloadSpeed, maxAmmunition);
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CDList.Remove(gunShotCD);
        }
    }

    /// <summary>
    /// 发射子弹
    /// </summary>
    /// <param name="targetPoint"> 子弹发射目标点 </param>
    public void GunModeShot(Vector3 targetPoint)
    {
        if (modeChangeOver != 0 || isReloading) return; //说明模式没有切换完成或者正在装弹
        if ((int)curAmmunition > 0)
        {
            if (gunShotCD.flag)
            {
                foreach (Transform i in gunBody)
                {
                    targetPoint.y = i.position.y;
                    EffectManager.Instance.SetBullet(i.position, targetPoint, gunBuffLevel);
                    FMODUnity.RuntimeManager.PlayOneShot("event:/Player/Gun/Fire");
                    //FmodManager.Instance.
                }
                curAmmunition--;
                gunShotCD.flag = false;
            }
        }
        else
        {
            StartCoroutine(GunReload());
        }
    }

    /// <summary>
    /// 发射法球
    /// 需要目标点
    /// </summary>
    public void StaffModeShot(Vector3 targetPoint)
    {
        EffectManager.Instance.SetMagicBall(targetPoint, magicBallSize);
        FMODUnity.RuntimeManager.PlayOneShot("event:/Player/Zhang/fireBallBoomLow");
    }

    /// <summary>
    /// 根据模式按照不同速率调整浮游炮朝向
    /// </summary>
    /// <param name="targetPoint"> 目标点 </param>
    /// <param name="_WeaponType"> 武器类型 </param>
    public void ModeLookAt(Vector3 targetPoint, E_WeaponType _WeaponType)
    {
        if (_WeaponType == E_WeaponType.sword)
        {
            LookAt(targetPoint, swordRSpeed, false);
        }
        else if (_WeaponType == E_WeaponType.gun)
        {
            LookAt(targetPoint, gunRSpeed, true);
        }
        else if (_WeaponType == E_WeaponType.staff)
        {
            LookAt(targetPoint, staffRSpeed, false);
        }
    }

    /// <summary>
    /// 转向
    /// </summary>
    /// <param name="targetPoint">目标点</param>
    /// <param name="rSpeed"> 旋转速度 </param>
    /// <param name="bodyRotate"> 是否需要子浮游炮朝向目标点 </param>
    private void LookAt(Vector3 targetPoint, float rSpeed, bool bodyRotate)
    {
        if (modeChangeOver != 0) return; //说明模式没有切换完成
        Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position, Vector3.up);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, rSpeed);
        if (bodyRotate)
        {
            foreach (Transform i in gunBody)
            {
                i.LookAt(targetPoint, Vector3.up);
            }
        }
    }

    /// <summary>
    /// 切换武器模式
    /// </summary>
    /// <param name="_weaponType"> 目标武器模式 </param>
    public void ChangeMode(E_WeaponType _weaponType)
    {
        weaponType = _weaponType;
        StopAllCoroutines();
        isReloading = false;
        modeChangeOver = 0;
        if (weaponType == E_WeaponType.sword)
        {
            for (int i = 0; i < gunBody.Length; i++)
            {
                StartCoroutine(GunTransform(gunBody[i], swordModePos[i]));
                modeChangeOver++;
            }
        }
        else if (weaponType == E_WeaponType.gun)
        {
            for (int i = 0; i < gunBody.Length; i++)
            {
                StartCoroutine(GunTransform(gunBody[i], gunModePos[i]));
                modeChangeOver++;
            }
        }
        else
        {
            for (int i = 0; i < gunBody.Length; i++)
            {
                StartCoroutine(GunTransform(gunBody[i], staffModePos[i]));
                modeChangeOver++;
            }
        }
    }

    /// <summary>
    /// 协程控制模式转换时的浮游炮位移效果
    /// </summary>
    /// <param name="gun"> 需要位移的浮游炮 </param>
    /// <param name="target"> 目标位置 </param>
    /// <returns></returns>
    IEnumerator GunTransform(Transform gun, Transform target)
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

    /// <summary>
    /// 协程手动换弹逻辑
    /// </summary>
    IEnumerator GunReload()
    {
        isReloading = true;
        while (curAmmunition < maxAmmunition)
        {
            // print("手动换弹中");
            curAmmunition = Mathf.Min(Time.deltaTime * manualReloadSpeed + curAmmunition, maxAmmunition);

            yield return null;
        }
        isReloading = false;
        yield break;
    }
}
