using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoGunControl : MonoBehaviour
{
    private Transform[] gunBody = new Transform[4]; //浮遊砲身、砲身の回転を制御します
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
    private bool lockShotDir;   //方向をロックしますか？
    private int modeChangeOver = 0; //モード切替の進行状況、0は切替が完了したことを示します。
    public int ModeChangeOver { get { return modeChangeOver; } }
    [SerializeReference] public BossControl bossControl;    //ボススクリプト参照は、アニメーションステートマシンの状態設定に使用されます。
    [Header("剣モードの旋回速度")] public float swordRSpeed = 0.2f;
    [Header("ガンモードの旋回速度")] public float gunRSpeed = 0.05f;

    [Header("ガンアタックモード1の照準時間")] public float gunMode1AimingTime = 1.5f;
    [Header("ガンアタックモード1の範囲提示時間")] public float gunMode1HintTime = 1.0f;
    [Header("銃攻撃モード1の発射回数")] public int gun1ShotCount = 10;
    [Header("ガンアタックモード1射撃間隔")] public float gun1ShotInterval = 0.2f;

    [Header("ガンアタックモード2の照準時間")] public float gunMode2AimingTime = 2.0f;
    [Header("ガンアタックモード2の範囲表示時間")] public float gunMode2HintTime = 1.0f;
    [Header("ガンアタックモード2の射撃回数")] public int gun2ShotCount = 20;
    [Header("ガンアタックモード2回転速度")] public float gun2ShotRotateSpeed = 5.0f;
    [Header("ガンアタックモード2の射撃間隔")] public float gun2ShotInterval = 0.2f;

    [Header("スティック攻撃モードの照準時間")] public float staffModeAimingTime = 4.0f;
    [Header("スティック攻撃モードの射撃回数")] public int staffShotCount = 1;
    [Header("スティック攻撃モードの射撃間隔")] public float staffModeShotInterval = 0.75f;
    [Header("スタッフの攻撃モードはランダムな範囲です。")] public float staffModeShotRange = 5.0f;

    [Header("スティックモードの旋回速度")] public float staffRSpeed = 0.1f;
    [Header("パターン切替の移動速度")] public float modeTranstionSpeed = 5f;
    [Header("パターン切替の回転速度")] public float modeTranstionRSpeed = 0.2f;

    /// <summary>
    /// フロート砲の初期化
    /// </summary>
    public void AutoGunInit()
    {
        gunBodyTF = transform.Find("Rotate").Find("GunBodys");
        //攻撃範囲を取得し、まずは隠します。
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
    /// パターンに従って、浮遊砲の向きを異なる速度で調整します。
    /// </summary>
    /// <param name="targetPoint"> ターゲットポイント </param>
    /// <param name="_BossAttackMode"> 攻撃タイプ </param>
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

    ///<summary>


    ////// 方向転換


    ///</summary>
    /// <param name="targetPoint">目標点</param>
    /// <param name="rSpeed"> 回転速度 </param>
    /// <param name="bodyRotate"> ターゲットポイントに向けて子浮遊砲が必要かどうか </param>
    private void LookAt(Vector3 targetPoint, float rSpeed)
    {
        if (modeChangeOver != 0) return; //モードの切り替えがまだ完了していません
        targetPoint.y = transform.position.y;
        Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position, Vector3.up);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, rSpeed);
    }

    ///<summary>


    ////// 武器モードを切り替える


    ///</summary>
    /// <param name="_weaponType"> ターゲットの武器モード </param>
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

    ///<summary>


    ////// 浮遊砲は転換フェーズモードに入ります


    ///</summary>
    public void ChangeMode_ChangeStage()
    {
        StopAllCoroutines();
        gun01ModeGO.SetActive(false);
        gun02ModeGO.SetActive(false);
        lockShotDir = false;    //プレイヤーに対して許可する（避ける感じがある）
        modeChangeOver = 0;
        for (int i = 0; i < gunBody.Length; i++)
        {
            StartCoroutine(IE_GunTransform(gunBody[i], changeStageModePos[i]));
            modeChangeOver++;
        }
    }

    ///<summary>


    ////// フローティング砲が死亡状態になります。


    ///</summary>
    public void ChangeMode_Dead()
    {
        Debug.Log("フローティング砲が死亡状態に入ります。");
    }
    #endregion

    #region 攻击相关
    /// <summary>
    /// ガンモード攻撃
    /// </summary>
    /// <param name="type"> 攻撃モード1または2 </param>
    public void GunAttack(int type)
    {
        if (type == 1)  //攻撃モード1
        {
            StartCoroutine(IE_GunAttack01());
        }
        else        //攻撃モード2
        {
            StartCoroutine(IE_GunAttack02());
        }
    }

    /// <summary>
    /// スティックモード攻撃
    /// </summary>
    public void StaffAttack()
    {
        StartCoroutine(IE_StaffAttack());
    }

    /// <summary>
    /// ガンアタックモード01コルーチン
    /// </summary>
    IEnumerator IE_GunAttack01()
    {
        float time = 0;
        //プレイヤーを追跡する力を蓄えるを表示します。
        while (time < gunMode1AimingTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        time = 0;
        bossControl.BossAttack_Gun_End(); //ショットチャントの特殊効果をオフにします
        //方向を固定する
        // lockShotDir = true;
        //表示範囲のヒント
        gun01ModeGO.SetActive(true);
        while (time < gunMode1HintTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        time = 0;
        //ヒントの範囲を閉じる && 発射
        gun01ModeGO.SetActive(false);
        for (int i = 0; i < gun1ShotCount; i++)
        {
            for (int j = 0; j < gunBody.Length; j++)
            {
                EffectManager.Instance.Boss_SetBullet(gunBody[j], bossControl);
                FMODUnity.RuntimeManager.PlayOneShot("event:/BOSS/gunFire");
            }
            while (time < gun1ShotInterval)  //発射完了を待つ
            {
                time += Time.deltaTime;
                yield return null;
            }
            time = 0;
        }
        while (time < 1.0f)  //発射完了を待つ
        {
            time += Time.deltaTime;
            yield return null;
        }
        time = 0;
        //発射完了
        // lockShotDir = false;
        bossControl.SetAnimatorBool("attacking", false);
        bossControl.SetAnimatorBool("move", true);
        bossControl.bossCD.canGunAttack1.flag = false;
        yield break;
    }

    /// <summary>
    /// ガンアタックモード02コルーチン
    /// </summary>
    IEnumerator IE_GunAttack02()
    {
        float time = 0;
        //方向を固定する
        lockShotDir = true;
        //充電表示
        while (time < gunMode2AimingTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        time = 0;
        bossControl.BossAttack_Gun_End(); //ショットチャントの特殊効果をオフにします
        //表示範囲のヒント
        gun02ModeGO.SetActive(true);
        while (time < gunMode2HintTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        time = 0;
        //ヒントの範囲を閉じる && 発射
        gun02ModeGO.SetActive(false);
        for (int i = 0; i < gun2ShotCount; i++)
        {
            for (int j = 0; j < gunBody.Length; j++)
            {
                EffectManager.Instance.Boss_SetBullet(gunBody[j], bossControl);
                FMODUnity.RuntimeManager.PlayOneShot("event:/BOSS/gunFire");
            }
            while (time < gun2ShotInterval)  //発射完了を待つ
            {
                time += Time.deltaTime;
                gunBodyTF.Rotate(Vector3.up * gun2ShotRotateSpeed * Time.deltaTime, Space.World);
                yield return null;
            }
            time = 0;
        }
        while (time < 1.0f)  //発射完了を待つ
        {
            time += Time.deltaTime;
            yield return null;
        }
        time = 0;
        //発射完了
        lockShotDir = false;
        bossControl.SetAnimatorBool("attacking", false);
        bossControl.SetAnimatorBool("move", true);
        bossControl.bossCD.canGunAttack2.flag = false;
        yield break;
    }

    /// <summary>
    /// スティックアタックモードのコルーチン
    /// </summary>
    IEnumerator IE_StaffAttack()
    {
        PlayerControl curPlayer = GameManager.Instance.currentPlayer; //プレイヤーの参照を取得する
        Vector3 shotPostion;
        float time = 0;
        //方向を固定する
        lockShotDir = true;
        //充電表示
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
        bossControl.BossAttack_Staff_End(); //スペルキャスティングエフェクトをオフにする
        //発射
        for (int i = 0; i < staffShotCount; i++)
        {
            for (int j = 0; j < gunBody.Length; j++)
            {
                shotPostion = curPlayer.transform.position + new Vector3(Random.Range(-staffModeShotRange, staffModeShotRange), 0, Random.Range(-staffModeShotRange, staffModeShotRange));
                EffectManager.Instance.Boss_SetMagic_Shot(shotPostion, bossControl);
                // Debug.Log("スペルを発射する");
                while (time < staffModeShotInterval)  //発射完了を待つ
                {
                    time += Time.deltaTime;
                    yield return null;
                }
                time = 0;
            }
        }
        //発射完了を待つ
        while (time < 1.0f)
        {
            time += Time.deltaTime;
            yield return null;
        }
        time = 0;
        //発射完了
        lockShotDir = false;
        bossControl.SetAnimatorBool("attacking", false);
        bossControl.SetAnimatorBool("move", true);
        bossControl.bossCD.canStaffAttack.flag = false;
        yield break;
    }

    #endregion

    /// <summary>
    /// すべての状態を停止し、浮遊砲を転移段階の状態に移行してください。
    /// </summary>
    public void my_StopAllCoroutines()
    {
        StopAllCoroutines();
        ChangeMode_ChangeStage();
    }

    ///<summary>


    ////// 様々な状態をリセットする


    ///</summary>
    public void ResetStates()
    {
        ChangeMode(bossAttackMode);
    }

    /// <summary>
    /// コルーチン制御モードの切替時における浮遊砲の位相効果
    /// </summary>
    /// <param name="gun"> 移動が必要な浮遊砲 </param>
    /// <param name="target"> 目標位置 </param>
    /// <returns></returns>
    IEnumerator IE_GunTransform(Transform gun, Transform target)
    {
        Vector3 dir = gun.transform.position - target.position;
        float dis = dir.magnitude;
        //まず、角度を調整してください。
        while (Mathf.Abs(Quaternion.Dot(gun.rotation, target.rotation)) < 0.95f)
        {
            // Debug.Log("回転中");
            gun.rotation = Quaternion.Lerp(gun.rotation, target.rotation, modeTranstionRSpeed);
            yield return null;
        }
        gun.rotation = target.rotation;
        //位置調整中です
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
}
