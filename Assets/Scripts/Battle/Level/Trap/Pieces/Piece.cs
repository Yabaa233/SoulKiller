using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    //プレイヤーの攻撃は戦闘システムにカウントされません。
    [Tooltip("Highest Health Points")] public float maxHealth = 100.0f;
    [Tooltip("Current HP, fixed damage when attacked.")] public float curHealth = 100.0f;
    [Tooltip("Damage value during close combat attack")] public float getSwordDamage = 20.0f;
    [Tooltip("Damage value during bullet attack")] public float getShotDamage = 1.0f;
    [Tooltip("Damage value during magic attack")] public float getMagicDamage = 15.0f;

    [Tooltip("Attack Power")] public float attack = 10;
    [Tooltip("The power to defeat the player")] public float forcePower = 10;
    [Header("Rising Related Parameters")]
    [Tooltip("Rate of ascent")] public float upSpeed = 10.0f;
    [Tooltip("Ascending altitude")] public float targetY = 3.0f;
    [Tooltip("Ascending altitude")] public float upWait = 1.5f;
    [Header("Horizontal Movement Related Parameters")]
    [Tooltip("Horizontal moving speed")] public float mvoeSpeed = 10.0f;
    [Header("Decline-related Parameters")]
    [Tooltip("Descent speed")] public float downSpeed = 20.0f;
    [Tooltip("ダウン待機")] public float downWait = 1.5f;
    [Tooltip("Landing point location")] public float downTargetY = 10.0f;
    [Header("Current state-related parameters")]
    public bool isMoveing = false;
    public bool isAttacking = false;
    [Header("KingまたはQueenかどうか")] public bool isKingOrQueen = false;
    [Header("Kingかどうか")] public bool isKing;
    [Header("Queenかどうか")] public bool isQueen;
    [Header("Whether it's white or not.")] public bool isWhite;

    [Header("Number of times the chess piece is hit and vibrates")] public float pieceHurtCount;
    [Header("Duration of a single vibration when the chess piece is hit")] public float pieceHurtTime;
    [Header("The coefficient of inverse proportionality between the damage value and the hit taken by the chess piece.")] public float pieceHurtPer;
    [Header("Vibration and damage ratio curve when hit")] public AnimationCurve hurtEffCurve;
    private IEnumerator moveToTartgetPoint; //現在の移動コルーチンを保存する
    private bool moveStarted;
    private Collider weapon;    //攻撃範囲
    private CheckerBoard checkerBoard;  //チェッカーボードスクリプト
    private StateBar stateBar;
    private Material[] pieceMaterial;
    private bool isDead = false;
    private GameObject pieceDownEff;
    private bool isHurting;
    private Transform curPlayerTF;
    private Transform pieceBody;
    private void Awake()
    {
        weapon = transform.Find("Weapon").GetComponent<Collider>();
        checkerBoard = transform.parent.parent.GetComponent<CheckerBoard>();
        pieceBody = transform.GetChild(0);
    }
    public void InitMove(Vector3 targetPosition)
    {
        transform.position = targetPosition;
    }
    private void Start()
    {
        curHealth = maxHealth;  //HP初期化
        if (isKingOrQueen)
        {
            checkerBoard.kingOrQueenCount++;
        }
        checkerBoard.pieceCount++;
        stateBar = PanelManager.Instance.GenerateCommonStatePanel(this.transform);
        stateBar.SetPositionBias(new Vector3(0, 2, 0)); //HPバーのオフセット設定
        curPlayerTF = GameManager.Instance.currentPlayer.transform;
    }
    private void OnDisable()
    {
        if (moveStarted)
        {
            Debug.Log("The movement has been destroyed, restoring canNext.");
            checkerBoard.CanNext(true);
        }
        if (stateBar != null) stateBar.DestroyThis();
        stateBar = null;
    }
    private void OnDestroy()
    {
        if (stateBar != null) stateBar.DestroyThis();
        stateBar = null;
    }

    private void Update()
    {
        if (stateBar != null) stateBar.UpdateState(curHealth, maxHealth);
    }

    ///<summary>


    ////// 駒の移動


    ///</summary>
    /// <param name="targetPosition"> 移動先の目標地点 </param>
    public void Move(Vector3 targetPosition)
    {
        moveStarted = true;
        moveToTartgetPoint = MoveToTartgetPoint(targetPosition);
        StartCoroutine(moveToTartgetPoint);
    }

    /// <summary>
    /// モバイル制御コルーチン
    /// </summary>
    /// <param name="target"> 目的地 </param>
    /// <returns></returns>
    IEnumerator MoveToTartgetPoint(Vector3 target)
    {
        float time = 0; //時間計測用
        //持ち上げる
        if (pieceDownEff != null) Destroy(pieceDownEff);
        isMoveing = true;   //移動が開始されると、プレイヤーは攻撃することができず、またプレイヤーを攻撃することもできません。
        while (transform.position.y < targetY)
        {
            transform.Translate(Vector3.up * Time.deltaTime * upSpeed, Space.World);
            yield return null;
        }
        while (time < upWait) //スタンバイ
        {
            time += Time.deltaTime;
            yield return null;
        }
        time = 0;
        //水平移動
        Vector3 curtarget = target;
        curtarget.y = targetY;  //上下の動きを防止する
        Vector3 dir = curtarget - transform.position;
        while (dir.magnitude > 0.5f)
        {
            transform.Translate(dir * Time.deltaTime * mvoeSpeed, Space.World);
            dir = curtarget - transform.position;
            yield return null;
        }
        //下降
        isAttacking = true;     //攻撃状態開始
        while (time < downWait) //ダウン待機
        {
            time += Time.deltaTime;
            yield return null;
        }
        weapon.enabled = true;  //コライダーを有効にする
        isMoveing = false;  //プレイヤーが攻撃可能になる
        while (transform.position.y > downTargetY)
        {
            transform.Translate(Vector3.down * Time.deltaTime * downSpeed, Space.World);
            dir = curtarget - transform.position;
            yield return null;
        }
        checkerBoard.CanNext(true);    //次のピースを進める
        weapon.enabled = false; //コライダーを無効にする
        isAttacking = false;    //攻撃状態が終了しました
        pieceDownEff = Instantiate(checkerBoard.pieceDownEff, transform.position, Quaternion.identity, transform);

        FMODUnity.RuntimeManager.PlayOneShot("event:/Level/AoMan/qiziGround");
        moveStarted = false;
        yield break;
    }

    ///<summary>


    ////// 駒がダメージを受けた時のヒットシェイク効果


    ///</summary>
    /// <param name="damage"></param>
    /// <returns></returns>
    IEnumerator IE_PieceHurt(float damage)
    {
        isHurting = true;
        float time = 0;
        Vector3 deviation = pieceBody.position - curPlayerTF.position;
        deviation.y = 0;
        deviation.Normalize();
        deviation *= hurtEffCurve.Evaluate(damage / pieceHurtPer);
        for (int i = 0; i < pieceHurtCount; i++)
        {
            pieceBody.position += deviation;
            while (time < pieceHurtTime)
            {
                time += Time.deltaTime;
                yield return null;
            }
            time = 0;
            pieceBody.position -= deviation;
            while (time < pieceHurtTime)
            {
                time += Time.deltaTime;
                yield return null;
            }
            time = 0;
        }
        isHurting = false;
        yield break;
    }

    ///<summary>


    ////// 駒はプレイヤーによって破壊される可能性があります。また、プレイヤーが攻撃することも可能です。


    ///</summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (isMoveing) return;
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerWeapon"))
        {
            if (other.tag == "PlayerWeapon")
            {
                curHealth -= getSwordDamage;
                if (!isHurting) StartCoroutine(IE_PieceHurt(getSwordDamage));
            }
            else if (other.tag == "PlayerBullet")
            {
                curHealth -= getShotDamage;
                if (!isHurting) StartCoroutine(IE_PieceHurt(getShotDamage));
            }
            else if (other.tag == "PlayerMagic")
            {
                curHealth -= getMagicDamage;
                if (!isHurting) StartCoroutine(IE_PieceHurt(getMagicDamage));
            }
            else
            {
                return;
            }
            FMODUnity.RuntimeManager.PlayOneShot("event:/Level/AoMan/qiziHit");
            if (curHealth <= 0)
            {
                BreakThisPiece();
            }
        }
    }

    /// <summary>
    /// weaponスクリプトからの呼び出し用
    /// </summary>
    public void WeaponAttackPlayer()
    {
        GameManager.Instance.TrickAttackPlayer(AttackPlayer);
    }
    /// <summary>
    /// プレイヤーの攻撃ロジック
    /// </summary>
    /// <param name="curPlayer"> 現在のプレイヤーを取得 </param>
    private void AttackPlayer(PlayerControl curPlayer)
    {
        Vector3 dir = curPlayer.transform.position - transform.position;
        dir.y = 0;
        curPlayer.rb.AddForce(dir.normalized * forcePower, ForceMode.Impulse);
        if (!curPlayer.characterBuffManager.CalcuSheild(null, attack))
        {
            curPlayer.characterData.currentHealth -= attack;
        }
    }

    /// <summary>
    /// weaponスクリプトからの呼び出し用
    /// </summary>
    public void WeaponAttackEnemy(BaseEnemyControl baseEnemyControl)
    {
        GameManager.Instance.TrickAttackEnemy(AttackEnemy, baseEnemyControl);
    }

    ///<summary>


    ////// 小さなモンスターの攻撃ロジック


    ///</summary>
    /// <param name="enemy"> 現在のモンスターを取得 </param>
    private float AttackEnemy(BaseEnemyControl enemy)
    {
        Vector3 dir = enemy.transform.position - transform.position;
        dir.y = 0;
        enemy.rb.AddForce(dir.normalized * forcePower, ForceMode.Impulse);
        Debug.Log(dir.normalized * forcePower);
        if (!enemy.characterBuffManager.CalcuSheild(null, attack))
        {
            enemy.enemyData.currentHealth -= attack;
            enemy.Damaged(attack);
        }
        return attack;
    }

    ///<summary>


    ////// 現在の駒は攻撃で破壊されました。


    ///</summary>
    private void BreakThisPiece()
    {
        if (moveToTartgetPoint != null)
        {
            StopCoroutine(moveToTartgetPoint);
        }
        if (isDead)
        {
            return;
        }
        isDead = true;
        if (stateBar != null) stateBar.DestroyThis();   //ヘルスバーを閉じる
        stateBar = null;
        gameObject.GetComponent<BoxCollider>().enabled = false; //プレイヤーが通過できるようにする
        if (isKingOrQueen)
        {
            BreakKingOrQueen();
        }
        else
        {
            BreakEffect();
        }
        FMODUnity.RuntimeManager.PlayOneShot("event:/Level/AoMan/qiziBoom");
        checkerBoard.pieceCount--;
        if (isKingOrQueen)
        {
            checkerBoard.CheckKingAndQueen();
        }
    }

    /// <summary>
    /// KingとQueenが全て破壊され、結果として現在の駒も破壊されました。
    /// </summary>
    public void BreakAllPiece_One()
    {
        if (moveToTartgetPoint != null)
        {
            StopCoroutine(moveToTartgetPoint);
        }
        if (isDead)
        {
            return;
        }
        isDead = true;
        if (stateBar != null) stateBar.DestroyThis();   //ヘルスバーを閉じる
        stateBar = null;
        gameObject.GetComponent<BoxCollider>().enabled = false; //プレイヤーが通過できるようにする
        if (isKingOrQueen)
        {
            BreakKingOrQueen();
        }
        else
        {
            BreakEffect();
        }
        // gameObject.SetActive(false);
        checkerBoard.pieceCount--;
    }

    ///<summary>


    ////// 王と女王の破壊効果を発動する


    ///</summary>
    private void BreakKingOrQueen()
    {
        if (isKing)
        {
            if (isWhite)
            {
                Instantiate(checkerBoard.kingBreakEff_W, transform.position, Quaternion.identity);
            }
            else
            {
                Instantiate(checkerBoard.kingBreakEff_B, transform.position, Quaternion.identity);
            }
        }
        else if (isQueen)
        {
            if (isWhite)
            {
                Instantiate(checkerBoard.queenBreakEff_W, transform.position, Quaternion.identity);
            }
            else
            {
                Instantiate(checkerBoard.queenBreakEff_B, transform.position, Quaternion.identity);
            }
        }
        gameObject.SetActive(false);
    }

    ///<summary>


    ////// 破壊効果を発動する


    ///</summary>
    public void BreakEffect()
    {
        pieceMaterial = transform.GetChild(0).gameObject.GetComponent<Renderer>().materials;
        foreach (var material in pieceMaterial)
        {
            material.SetVector("_AbsorbPoint", new Vector4(transform.position.x, transform.position.y - 0.5f, transform.position.z, 0f));
        }
        StartCoroutine(BreakPiece(2f, 60));
    }

    IEnumerator BreakPiece(float duringTime, float endFloat)
    {
        float time = 0f;
        while (time < duringTime)
        {
            time += Time.deltaTime;
            float percent = time / duringTime;
            float curFloat = endFloat * percent;
            // Debug.Log(curFloat);
            foreach (var material in pieceMaterial)
            {
                material.SetFloat("_AbsorbRadius", curFloat);
            }
            yield return null;
        }
        gameObject.SetActive(false);
        yield break;
    }

    ///<summary>


    ////// 死亡しているかどうかを確認します、trueはすでに死亡していることを示します。


    ///</summary>
    /// <returns></returns>
    public bool CheckisDead()
    {
        return isDead;
    }
}
