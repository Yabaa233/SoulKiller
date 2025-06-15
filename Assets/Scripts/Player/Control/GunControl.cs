using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunControl : MonoBehaviour
{
    private Transform[] gunBody = new Transform[4]; //浮遊砲身、砲身の回転を制御します
    private Transform staffShotPoint;   //法球発射点
    private Transform[] swordModePos = new Transform[4];
    private Transform[] gunModePos = new Transform[4];
    private Transform[] staffModePos = new Transform[4];
    private E_WeaponType weaponType;
    private int modeChangeOver = 0;
    public int ModeChangeOver { get { return modeChangeOver; } }
    public int gunBuffLevel;    //ガンバフレベル
    [Tooltip("最大弾数")] public float maxAmmunition = 60; //最大弾数
    //TODO：ここはfloatなので、UI表示はint型を指定するか、エネルギー形式で表示する必要があります。
    [Tooltip("現在の弾薬数")] public float curAmmunition = 60;  //現在の弾薬数
    [Tooltip("弾丸シューティングCD")] public CDClass gunShotCD = new CDClass(); //ショットCD
    [Tooltip("フランスのボールのサイズ")] public float magicBallSize; //フランスのボールのサイズ
    [Tooltip("弾を装填していますか？")] public bool isReloading;  //弾を装填していますか？
    [Tooltip("弾丸の自動装填速度")] public float autoReloadSpeed; //自動装填速度
    [Tooltip("弾丸の手動装填速度")] public float manualReloadSpeed;   //手動装填速度
    [Header("剣モードの旋回速度")] public float swordRSpeed = 0.2f;
    [Header("ガンモードの旋回速度")] public float gunRSpeed = 0.05f;
    [Header("スティックモードの旋回速度")] public float staffRSpeed = 0.1f;
    [Header("パターン切替移動速度")] public float modeTranstionSpeed = 1.5f;
    [Header("パターン切替回転速度")] public float modeTranstionRSpeed = 0.1f;

    ///<summary>


    ////// 初期化


    ///</summary>
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
        if (weaponType != E_WeaponType.gun && curAmmunition < maxAmmunition) //武器を交換すると自動的に弾が装填されます
        {
            //print("自動的に弾丸を交換中です");
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

    ///<summary>


    ////// 弾を発射する


    ///</summary>
    /// <param name="targetPoint"> 弾の発射目標点 </param>
    public void GunModeShot(Vector3 targetPoint)
    {
        if (modeChangeOver != 0 || isReloading) return; //モードの切り替えが完了していないか、または弾薬を装填中です。
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

    ///<summary>


    ////// 発射法球
    /// 目標点が必要です


    ///</summary>
    public void StaffModeShot(Vector3 targetPoint)
    {
        EffectManager.Instance.SetMagicBall(targetPoint, magicBallSize);
        FMODUnity.RuntimeManager.PlayOneShot("event:/Player/Zhang/fireBallBoomLow");
    }

    /// <summary>
    /// パターンに従って、浮遊砲の向きを異なる速度で調整します。
    /// </summary>
    /// <param name="targetPoint"> ターゲットポイント </param>
    /// <param name="_WeaponType"> 武器タイプ </param>
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

    ///<summary>


    ////// 方向変更


    ///</summary>
    /// <param name="targetPoint">目標点</param>
    /// <param name="rSpeed"> 回転速度 </param>
    /// <param name="bodyRotate"> ターゲットポイントに向かって子浮遊砲が必要かどうか </param>
    private void LookAt(Vector3 targetPoint, float rSpeed, bool bodyRotate)
    {
        if (modeChangeOver != 0) return; //モードの切り替えが完了していません
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

    ///<summary>


    ////// 武器モードを切り替える


    ///</summary>
    /// <param name="_weaponType"> 目標の武器モード </param>
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
    /// コルーチン制御モード切替時の浮遊砲の位移効果
    /// </summary>
    /// <param name="gun"> 移動が必要な浮遊砲 </param>
    /// <param name="target"> 目標位置 </param>
    /// <returns></returns>
    IEnumerator GunTransform(Transform gun, Transform target)
    {
        Vector3 dir = gun.transform.position - target.position;
        float dis = dir.magnitude;
        //まず角度を調整してください。
        while (Mathf.Abs(Quaternion.Dot(gun.rotation, target.rotation)) < 0.95f)
        {
            // Debug.Log("回転中");
            gun.rotation = Quaternion.Lerp(gun.rotation, target.rotation, modeTranstionRSpeed);
            yield return null;
        }
        gun.rotation = target.rotation;
        //位置を調整しています
        while (dis > 0.1f)
        {
            // Debug.Log("移動中");
            gun.Translate(dir * modeTranstionSpeed * Time.deltaTime, Space.World);
            dir = target.position - gun.transform.position;
            dis = dir.magnitude;
            yield return null;
        }
        modeChangeOver--;
        yield break;
    }

    /// <summary>
    /// コルーチンでの手動弾薬交換ロジック
    /// </summary>
    IEnumerator GunReload()
    {
        isReloading = true;
        while (curAmmunition < maxAmmunition)
        {
            // print("手動で弾丸を交換中");
            curAmmunition = Mathf.Min(Time.deltaTime * manualReloadSpeed + curAmmunition, maxAmmunition);

            yield return null;
        }
        isReloading = false;
        yield break;
    }
}
