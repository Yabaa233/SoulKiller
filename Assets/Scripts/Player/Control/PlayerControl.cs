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
/// <summary>
/// プレイヤー制御
/// </summary>
public class PlayerControl : MonoBehaviour
{
    [Header("戦闘関連")]
    [Tooltip("現在の武器の種類")] public E_WeaponType weaponType; //武器の種類
    [Tooltip("シューティング感覚に関連する設定")] public ComboNode shotComboNode;
    [Tooltip("魔法攻撃感覚に関連する設定")] public ComboNode magicComboNode;
    public ComboNode currentComboNode;  //現在の連続スキルノード
    public CDClass dodgeCD = new CDClass(); //CDをスプリント
    private bool canShot;   //ガンモードでの長押し連射の射撃時間が基準を満たしているかどうか
    private bool magicBallStart = false;    //力を蓄え始めますか？は日本語です。翻訳する内容がありません。
    public float staffHoldTime;   //目標蓄力時間
    public float curHoldTime;   //現在の累積時間
    public int dodgeCount = 1;  //スプリントの利用可能回数
    [Header("武器のバフレベル")]
    public int swordBuffLevel;  //剣バフレベル
    public int swordBuffTimes;  //剣の連続攻撃の回数
    public int gunBuffLevel;    //ガンバフレベル
    public int staffBuffLevel;  //スタッフBuffレベル
    [Header("浮遊砲制御コンポーネント")]
    public GunControl gunControl;   //浮遊砲制御コンポーネント
    [SerializeField]
    public GameObject comboTrigger;     //コンボの攻撃範囲トリガー
    [Header("アニメーションコントローラー")]
    public Animator animator;
    [Header("モバイルパフォーマンスに関連する")]
    public Vector3 moveRes;    //移動方向の結果
    public float staffStopLerp = 0.02f;
    public bool useMouseScale = false; //マウスを使用して結果に向かい、キーボード入力を使用せずに結果に向かいます。攻撃をロックする際にも使用します。
    private int scaleRes_Move = 1;   //結果として左右に移動する
    private int scaleRes_Mouse = 1;   //キャラクターの攻撃時の左右の回転を制御するために使用します。
    public Vector3 targetPoint;    //マウスの向きの重複作成を防ぐ
    [Header("キャラクター剛体")]
    public Rigidbody rb;   //キャラクター剛体
    [Tooltip("骨格")] public Transform skeletal;  //骨格
    [Tooltip("足の裏が光の輪に向かっています")] public Transform orientationObject; //あなたの足元が光の輪に向かっています
    [Header("（使用しない）キャラクター数値テンプレート")]
    public CharacterData_SO tempCharaterData;   //クローニング用
    [Header("キャラクターの属性に関連する")]
    public CharacterData characterData; //キャラクター属性設定SOファイルのインスタンス
    [Tooltip("移動速度")] public Vector2 speed = new Vector3();
    [Header("キャラクターBuffマネージャー")]
    public CharacterBuffManager characterBuffManager;   //現在のBuff管理リスト
    // public GameObject TestCube; //テストキューブ
    public bool lockHealth = false;
    public bool isDead = false;
    public bool IsDead { get { return isDead; } }
    // [ヘッダー("キャラクターの音声コントローラー")]
    [Header("特殊効果表示関連")]
    [Tooltip("ダッシュ特殊効果の出現位置の補正")] public Vector3 effectDashPos = new Vector3(0.0f, 1.5f, 0.0f);
    [Tooltip("攻撃特効の出現位置補正")] public Vector3 effectAtkPos = new Vector3(0.0f, 1.5f, 0.0f);

    private Vector2 rightStickPos = new Vector2(960, 540);  //右スティックの方向コントローラーの座標
    private Vector2 rightStickMoveSpeed = new Vector2(1200, 800);  //右スティックの感度

    #region 初始化相关
    private void Awake()
    {
        PlayerInit("Player");
    }

    /// <summary>
    ///  キャラクターの各機能コンポーネントを初期化する
    /// </summary>
    /// <param name="tagStr"> 初期化したいタグ </param>
    private void PlayerInit(string tagStr)
    {
        //コンポーネントの取得
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        gunControl = transform.Find("GunParent").GetComponent<GunControl>();
        //サブオブジェクトの取得
        skeletal = transform.Find("BOSS");
        orientationObject = transform.Find("OrientationObject");
        //自分の属性設定
        transform.tag = tagStr;
        useMouseScale = false;  //デフォルトではキーボードの向きを使用します
        ResetAnimState();//自分のアニメーション状態をリセットする
        //各子コンポーネントを作成する
        characterData = new CharacterData(Instantiate(tempCharaterData));   //インスタンスを作成し、初期化します。
        characterBuffManager = new CharacterBuffManager();
    }

    private void Start()
    {
        //ゲームマネージャーに登録する
        GameManager.Instance.currentPlayer = this;
        GameManager.Instance.PlayerDie += PlayerDie;
        //CDを登録する
        GameManager.Instance.CDList.Add(dodgeCD);
        dodgeCD.flag = true;
        //Buffシステムの初期化
        characterBuffManager.Init(E_ChararcterType.player);
        PlayerBuffRebuild(BuffDataManager.Instance.playerBuffList);
        characterBuffManager.RefreshData();
        //武器タイプを剣に初期化します
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
    /// アニメーションパラメーターの状態を初期化する
    /// </summary>
    public void ResetAnimState()
    {
        if (animator == null)
        {
#if UNITY_EDITOR
            Debug.LogError("現在のキャラクターアニメーションコントローラーは空です" + this.gameObject);
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
    ///  キャラクターの移動ベクトルを取得する
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
        //自分自身を参照点として使用し、その世界座標から自分の世界座標を引く（ただし、操作が完了すると逆方向になり、その理由はまだ見つかっていない）。
        moveRes = transform.position - transform.TransformPoint(moveRes);
        //積み重ね加速
        moveRes.x *= speed.x;
        moveRes.z *= speed.y;
        // moveRes.y = rb.velocity.y;
        return;
    }

    /// <summary>
    /// モバイルデバイスでの移動入力
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
        //自分自身を参照点として使用し、その世界座標から自分の世界座標を引く（ただし、操作が完了すると逆方向になり、その理由はまだ見つかっていない）。
        moveRes = transform.position - transform.TransformPoint(moveRes);
        //積み重ね加速
        moveRes.x *= speed.x;
        moveRes.z *= speed.y;
        // moveRes.y = rb.velocity.y;
        return;
    }

    /// <summary>
    /// モバイル端末でキャラクターの移動時の回転結果を取得する
    /// </summary>
    public void GetPlayerInput_StickMoveRotate(Vector2 newPos)
    {
        if (newPos.x != 0)
        {
            //移動時の回転を修正する
            scaleRes_Move = newPos.x > 0 ? -1 : 1;
        }
    }

    /// <summary>
    /// モバイルデバイスへの入力方法
    /// </summary>
    /// <param name="callbackContext"></param>
    public void GetPlayerInput_StickRotate(Vector2 newPos)
    {
        Vector2 temp = newPos;
        //操作手順1 増分式変位
        // temp *= Time.deltaTime * rightStickMoveSpeed;
        // temp += rightStickPos;
        // rightStickPos.x = Mathf.Min(1920, temp.x);
        // rightStickPos.x = Mathf.Max(0, temp.x);
        // rightStickPos.y = Mathf.Min(1080, temp.y);
        // rightStickPos.y = Mathf.Max(0, temp.y);

        //操作スキーム2 位置変化式
        temp /= 2;
        temp += Vector2.one / 2;
        temp.x *= 1920;
        temp.y *= 1080;
        rightStickPos = temp;
    }

    /// <summary>
    /// キャラクターのアパーチャー方向を取得し調整する結果
    /// キャラクターがマウスに向かってスケールする際の取得
    /// </summary>
    public void GetPlayerInput_MouseRotate()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
#elif UNITY_ANDROID
        Ray ray = Camera.main.ScreenPointToRay(rightStickPos);
#endif
        //レイの衝突情報
        RaycastHit groundHit;
        if (Physics.Raycast(ray, out groundHit, 2000, LayerMask.GetMask("Ground")))
        {
            targetPoint = groundHit.point;  //キャラクターの向きを取得し、その向きの光環を制御するために使用します。
            scaleRes_Mouse = Vector3.Dot(transform.right, groundHit.point - transform.position) > 0 ? 1 : -1;
        }
        targetPoint.y = orientationObject.transform.position.y;
        // TestCube.transform.position = targetPoint;
        orientationObject.LookAt(targetPoint, Vector3.up);
        gunControl.ModeLookAt(targetPoint, weaponType);
    }

    /// <summary>
    /// キャラクターの移動時の回転結果を取得する
    /// </summary>
    public void GetPlayerInput_MoveRotate(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.ReadValue<Vector2>().x != 0)
        {
            //移動時の回転を修正する
            scaleRes_Move = callbackContext.ReadValue<Vector2>().x > 0 ? -1 : 1;
        }
    }

    /// <summary>
    ///  キャラクターの攻撃入力を取得する
    /// </summary>
    public void GetPlayerInput_Attack(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (weaponType != E_WeaponType.sword)
            {
                SetWeaponType(E_WeaponType.sword);
            }
            else if (animator.GetBool("canAttack"))  //キー入力は攻撃可能な時だけ取得できます。
            {
                animator.SetBool("attack", true);
                return;
            }
        }
        animator.SetBool("attack", false);
    }

    /// <summary>
    /// キャラクターのシューティング入力を取得する
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
    /// キャラクターの魔法攻撃力を取得する
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
            // Debug.Log("リリースの準備が整いました");
            CM_Effect.Instance.PlayerGetDamaged(Color.black, 10, 0.6f);
            if (staffBuffLevel == 4)
            {
                characterData.currentDefend = 5;
            }
            SetUseMouseScale(true);
            magicBallStart = true;
            EffectManager.Instance.playerMagicRange.gameObject.SetActive(true);
            EffectManager.Instance.playerMagicRange.GetComponent<AreaControl>().IsBuffed = staffBuffLevel >= 2 ? true : false;  //力を蓄える時間を短縮する条件を満たしていますか？
            GameManager.Instance.SetMouse_Shot();
            FmodManager.Instance.PlaySpecialSound("event:/Player/Zhang/fireBallBefore");
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            // Debug.Log("蓄えた力を解放する");
            if (weaponType == E_WeaponType.staff && curHoldTime >= staffHoldTime)
            {
                CM_Effect.Instance.PlayerGetDamaged(Color.white, 8, 0.6f);
                magicBallStart = false; //長押し連射をキャンセル
                // Debug.Log("火球を発射する");
                gunControl.StaffModeShot(targetPoint);
            }
            ExitStaffMode();
            FmodManager.Instance.PauseSpecialSound("event:/Player/Zhang/fireBallBefore");
        }
    }

    /// <summary>
    /// スペルモードを終了します
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
    /// キャラクターの回避入力の取得
    /// </summary>
    public void GetPlayerInput_Dodge(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (dodgeCD.flag)
            {
                dodgeCount = characterBuffManager.GetDogeTimes();
                // Debug.Log("スプリント");
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
    ///<summary>

    ////// 自分の3次元ベクトル差分関数
    /// フレームごとの補間
    /// このプロジェクトではxz軸のみを使用しているため、y軸の差は考慮していません。

    ///</summary>
    /// <param name="cur"> 現在の値 </param>
    /// <param name="tar"> 目標値 </param>
    /// <param name="value"> 微分係数 </param>
    /// <returns></returns>
    public void V3Lerp(ref Vector3 cur, Vector3 tar, float value)
    {
        cur.x = Mathf.Lerp(cur.x, tar.x, value * Time.deltaTime);
        cur.z = Mathf.Lerp(cur.z, tar.z, value * Time.deltaTime);
    }

    /// <summary>
    /// キャラクターの移動制御
    /// キャラクターの速度を変更します
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
    /// キャラクターの動きが徐々に止まります。
    /// </summary>
    public void PlayerStopMove(float StopLerpValue)
    {
        Vector3 lerpVelocity = rb.velocity;
        V3Lerp(ref lerpVelocity, Vector3.zero, StopLerpValue);
        rb.velocity = lerpVelocity;
    }

    /// <summary>
    /// マウスの向きを使用するかどうかを切り替えます
    /// </summary>
    /// <param name="_useMouseScale"> はい または いいえ </param>
    public void SetUseMouseScale(bool _useMouseScale)
    {
        useMouseScale = _useMouseScale;
    }

    ///<summary>


    ////// 移動はキャラクターのLocalScale（左右の向き）を変更します。
    /// 移動やスプリント時の方向調整に使用されます。


    ///</summary>
    public void PlayerBaseRotate_Move()
    {
        if (!useMouseScale)
        {
            skeletal.localScale = new Vector3(scaleRes_Move, 1, 1);   //方向を調整する
        }
    }

    ///<summary>


    ////// 攻撃はキャラクターのLocalScale（左右の向き）を変更します
    /// 攻撃時の方向調整用


    ///</summary>
    public void PlayerBaseRotate_Attack()
    {
        scaleRes_Move = scaleRes_Mouse;
        skeletal.localScale = new Vector3(scaleRes_Mouse, 1, 1);
    }

    /// <summary>
    /// キャラクターの基本的な回避行動
    /// </summary>
    /// <param name="dodgePower"> 回避速度 </param>
    public void PlayerBaseMove_Dodge(float dodgePower)
    {
        PlayerBaseMove_ForceMove(dodgePower);
        //スケルトンの中心位置に特殊効果を表示します
        EffectManager.Instance.SetDashEffect(transform.position + effectDashPos, rb.velocity);
        FMODUnity.RuntimeManager.PlayOneShot("event:/Player/shift");
    }

    /// <summary>
    /// キャラクターの強制移動
    /// </summary>
    /// <param name="movePower"> 強制移動速度 </param>
    /// <param name="useKeyBord"> キーボードを使って方向を制御するかどうか </param>
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

    ///<summary>


    ////// 武器のタイプを設定します。


    ///</summary>
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
        gunControl.ChangeMode(targetType);  //フロート砲モードに切り替えます
    }

    ///<summary>


    ////// 次のノードにコンボを切り替えます


    ///</summary>
    public void ChangeCombo(ComboNode nextCombo)
    {
        currentComboNode = nextCombo;
        // Debug.Log("スイッチする" + currentComboNode);
    }
    #endregion

    #region 战斗相关
    /// <summary>
    /// トリガーを起動する
    /// </summary>
    public void OpenTrigger()
    {
        comboTrigger.SetActive(true);
        // Debug.Log("表現");
    }
    ///<summary>

    ////// 攻撃特効を生成する

    ///</summary>
    public void CreateEffect()
    {
        EffectManager.Instance.SetAttackEffect(currentComboNode.attackEffect, comboTrigger.transform.position + effectAtkPos, comboTrigger.transform.rotation);
        FMODUnity.RuntimeManager.PlayOneShot(currentComboNode.attackSound);
    }
    /// <summary>
    /// トリガーを閉じる
    /// </summary>
    public void CloseTrigger()
    {
        comboTrigger.SetActive(false);
        // Debug.Log("クローズ");
    }
    /// <summary>
    /// キャラクターの探索突撃
    /// 現在のComboNodeに基づいて判断します
    /// </summary>
    public void PlayerAttackMove_Plunge()
    {
        //方向ベクトルを取得します（目標点から中心点へのベクトルを取得）、それを正規化した後、z方向の長さでベクトルを伸ばします。
        Vector3 dir = targetPoint - orientationObject.position;
        dir.y = 0;   //ピッチ角を消去します
        Vector3 dirNormal = dir.normalized; //正規化されたベクトルの結果を保存します。
        dir = dirNormal * currentComboNode.halfPlungeBoxSize.z;
        //中心位置を取得します（向きベクトルに移動した後の位置を中心位置と考えることができます）
        dir = new Vector3(orientationObject.position.x + dir.x, transform.position.y, orientationObject.position.z + dir.z);
        Collider[] colliders = Physics.OverlapBox(dir, currentComboNode.halfPlungeBoxSize, orientationObject.transform.rotation, LayerMask.GetMask("EmyBody"));

        // TestCube.transform.position = dir;
        // TestCube.transform.localScale = currentComboNode.halfPlungeBoxSize * 2;
        // TestCube.transform.rotation = orientationObject.transform.rotation;
        //Tiggerのサイズを変更し、dirはもう使用されていないので、ここで再利用することが可能です。
        dir = dirNormal * (currentComboNode.attackRange.z / 2 + currentComboNode.attackRangeDeviation);
        dir = new Vector3(orientationObject.position.x + dir.x, transform.position.y, orientationObject.position.z + dir.z);
        comboTrigger.transform.position = dir;
        comboTrigger.transform.localScale = currentComboNode.attackRange;
        comboTrigger.transform.rotation = orientationObject.transform.rotation;
        //追跡しなければならない
        if (colliders.Length == 0)
        {
            if (moveRes.magnitude > 3.0f)
            {
                // Debug.Log("敵が見つからない、キーボードの方向で敵を探す");
                // rb.velocity = Vector3.zero;
                dir = moveRes.normalized * currentComboNode.plungePower;
                rb.velocity = new Vector3(dir.x * currentComboNode.forceSpeed.x, rb.velocity.y, dir.z * currentComboNode.forceSpeed.y);
            }
            else
            {
                // Debug.Log("敵が見つからない、マウスの方向で敵を探す");
                // rb.velocity = Vector3.zero;
                dir = (targetPoint - transform.position).normalized * currentComboNode.plungePower;
                rb.velocity = new Vector3(dir.x * currentComboNode.forceSpeed.x, rb.velocity.y, dir.z * currentComboNode.forceSpeed.y);
            }
        }
        else
        {
            dir = colliders[0].transform.position - orientationObject.position;    //変数の再利用
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
            dir = colliders[0].transform.position - orientationObject.position;    //変数の再利用
            dir.y = 0;
            //あまりにも近すぎると追跡しない、攻撃範囲に入ったら追撃しない。
            if (dir.magnitude > currentComboNode.attackRange.z)
            {
                //敵を追いかける速度は、距離が遠くなるほど速度が上がります。
                // Debug.Log("敵を検出し、距離が遠い、索敵");
                // rb.velocity = Vector3.zero;
                rb.velocity = dir * currentComboNode.plungePower;
            }
            else
            {
                // Debug.Log("敵を検出し、距離も近い、索敵しない");
            }
        }
    }

    /// <summary>
    /// ヘビーアタックロジック
    /// </summary>
    private void PlayerAttack_Heavy(InputActionPhase phase)
    {
        if (phase == InputActionPhase.Started)
        {
            // Debug.Log("ヘビーアタック");
            animator.SetTrigger("heavyAttack");
        }
        else
        {
            // Debug.Log("ヘビーアタックをクリア");
            animator.ResetTrigger("heavyAttack");
        }
    }

    ///<summary>


    ////// 銃撃のロジック


    ///</summary>
    private void PlayerAttack_Gun()
    {
        // if (canShot && animator.GetBool("canAttack"))
        if (canShot)
        {
            gunControl.GunModeShot(targetPoint);
        }
    }

    /// <summary>
    /// スティック攻撃ロジック
    /// </summary>
    private void PlayerAttack_Staff()
    {
        //攻撃不能な状態になった時、蓄積時間をリセットします。
        //スプリントを中断するために適用されます
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
            //マウスの方向へ向かう
            PlayerBaseRotate_Attack();
            //パワーを蓄えている間は移動することができません。
            PlayerStopMove(staffStopLerp);
        }
    }

    /// <summary>
    /// プレイヤーのコンボ数に応じて、プレイヤーの攻撃倍率を上げる
    /// </summary>
    /// <param name="maxMagnification"> 最大倍率 </param>
    /// <param name="stepMagnification"> 連続ヒットごとの増加倍率 </param>
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
    /// プレイヤーのコンボ数に応じて、プレイヤーの攻撃倍率を上げる
    /// </summary>
    /// <param name="maxMagnification"> 最大倍率 </param>
    /// <param name="stepMagnification"> 連続ヒットごとの増加倍率 </param>
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

    ///<summary>


    ////// 攻撃判定


    ///</summary>
    /// <param name="other"> 打撃を受けた物体 </param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "EmyBody")
        {
            // Debug.Log("攻撃がヒットします");
            SetCurrentComboAttack(2f, 0.1f);
            GameManager.Instance.PlayerAttack(other.transform.parent.GetComponent<BaseEnemyControl>(), transform.position);
            GameManager.Instance.Player_StartHitEffect();   //打撃感プロセスを開始します
        }
        if (other.tag == "BossBody")
        {
            SetCurrentComboAttack(2, 0.1f);
            GameManager.Instance.PlayerAttack(other.GetComponent<BossControl>());
            GameManager.Instance.Player_StartHitEffect();   //打撃感プロセスを開始します
        }
    }

    /// <summary>
    /// アニメーションを一時停止、チョッピング効果
    /// </summary>
    /// <param name="pauseSpeed"> アニメーション再生速度を一時停止 </param>
    public void PauseAnimation(float pauseSpeed)
    {
        animator.speed = pauseSpeed;
    }

    /// <summary>
    /// アニメーションの復元 ラグ効果
    /// </summary>
    public void ContinueAnimation()
    {
        animator.speed = 1.0f;
    }
    #endregion

    #region 测试相关
    ///<summary>

    ////// 入力をブロックする

    ///</summary>
    public void DisableInput()
    {
        GetComponent<PlayerInput>().enabled = false;
    }

    ///<summary>


    ////// 入力を復元する


    ///</summary>
    public void EnableInput()
    {
        GetComponent<PlayerInput>().enabled = true;
        GetComponent<PlayerInput>().actions.actionMaps[0].Enable();
        GetComponent<PlayerInput>().actions.actionMaps[0].FindAction("Move").Enable();
    }

    /// <summary>
    /// Buff関連データを再読み込みします
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
    /// プレイヤーのバフを再読み込みします
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
    /// キャラクターの死
    /// </summary>
    public void PlayerDie()
    {
        isDead = true;
        DisableInput(); //入力をブロックする
        ContinueAnimation();
        animator.SetTrigger("die");
    }

    /// <summary>
    /// キャラクターが攻撃を受けました
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
    /// Qキーを押した後の操作テスト
    /// </summary>
    public void GetInputKey_Q(InputAction.CallbackContext context)
    {
        PlayerAttack_Heavy(context.phase);
        if (context.phase == InputActionPhase.Performed)
        {
        }
    }

    /// <summary>
    /// Eキーを押した後の操作テスト
    /// </summary>
    public void GetInputKey_E(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            // CM_Effect.Instance.CM_TransitionDim(8, 1.2f);
        }
    }

    /// <summary>
    /// Rキーを押した後の操作テスト
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
