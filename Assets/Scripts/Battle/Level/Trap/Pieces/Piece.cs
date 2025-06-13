using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    //プレイヤーの攻撃は戦闘システムにカウントされない
    [Tooltip("最高血量")] public float maxHealth = 100.0f;
    [Tooltip("現在の血量 被攻撃固定掉血")] public float curHealth = 100.0f;
    [Tooltip("近戦攻撃時のダメージ値")] public float getSwordDamage = 20.0f;
    [Tooltip("弾丸攻撃時のダメージ値")] public float getShotDamage = 1.0f;
    [Tooltip("魔法攻撃時のダメージ値")] public float getMagicDamage = 15.0f;

    [Tooltip("攻撃力")] public float attack = 10;
    [Tooltip("プレイヤーを打ち倒す力")] public float forcePower = 10;
    [Header("上昇関連パラメータ")]
    [Tooltip("上昇速度")] public float upSpeed = 10.0f;
    [Tooltip("上昇高度")] public float targetY = 3.0f;
    [Tooltip("上昇高度")] public float upWait = 1.5f;
    [Header("水平移動関連パラメータ")]
    [Tooltip("水平移動速度")] public float mvoeSpeed = 10.0f;
    [Header("下降関連パラメータ")]
    [Tooltip("下降速度")] public float downSpeed = 20.0f;
    [Tooltip("下降待機")] public float downWait = 1.5f;
    [Tooltip("下降落点位置")] public float downTargetY = 10.0f;
    [Header("現在の状態関連パラメータ")]
    public bool isMoveing = false;
    public bool isAttacking = false;
    [Header("KingまたはQueenかどうか")] public bool isKingOrQueen = false;
    [Header("Kingかどうか")] public bool isKing;
    [Header("Queenかどうか")] public bool isQueen;
    [Header("白色方かどうか")] public bool isWhite;

    [Header("棋子受擊震動回数")] public float pieceHurtCount;
    [Header("棋子受擊震動單次時間")] public float pieceHurtTime;
    [Header("棋子受擊與傷害值關聯反比例係數")] public float pieceHurtPer;
    [Header("受擊時震動與傷害的比例曲線")] public AnimationCurve hurtEffCurve;
    private IEnumerator moveToTartgetPoint; //現在の移動コルーチンを保存
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
            Debug.Log("正在移動被摧毀了，恢復canNext");
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

    /// <summary>
    /// 駒の移動
    /// </summary>
    /// <param name="targetPosition"> 移動先の目標地点 </param>
    public void Move(Vector3 targetPosition)
    {
        moveStarted = true;
        moveToTartgetPoint = MoveToTartgetPoint(targetPosition);
        StartCoroutine(moveToTartgetPoint);
    }

    /// <summary>
    /// 移動制御コルーチン
    /// </summary>
    /// <param name="target"> 目標地点 </param>
    /// <returns></returns>
    IEnumerator MoveToTartgetPoint(Vector3 target)
    {
        float time = 0; //時間計測用
        //抬起
        if (pieceDownEff != null) Destroy(pieceDownEff);
        isMoveing = true;   //移動開始、プレイヤーは攻撃できず、プレイヤーを攻撃することもできない
        while (transform.position.y < targetY)
        {
            transform.Translate(Vector3.up * Time.deltaTime * upSpeed, Space.World);
            yield return null;
        }
        while (time < upWait) //待機
        {
            time += Time.deltaTime;
            yield return null;
        }
        time = 0;
        //水平移動
        Vector3 curtarget = target;
        curtarget.y = targetY;  //上下の移動を防止
        Vector3 dir = curtarget - transform.position;
        while (dir.magnitude > 0.5f)
        {
            transform.Translate(dir * Time.deltaTime * mvoeSpeed, Space.World);
            dir = curtarget - transform.position;
            yield return null;
        }
        //下降
        isAttacking = true;     //攻撃状態開始
        while (time < downWait) //下降待機
        {
            time += Time.deltaTime;
            yield return null;
        }
        weapon.enabled = true;  //コライダーを有効化
        isMoveing = false;  //プレイヤーが攻撃可能になる
        while (transform.position.y > downTargetY)
        {
            transform.Translate(Vector3.down * Time.deltaTime * downSpeed, Space.World);
            dir = curtarget - transform.position;
            yield return null;
        }
        checkerBoard.CanNext(true);    //次の駒に進める
        weapon.enabled = false; //コライダーを無効化
        isAttacking = false;    //攻撃状態終了
        pieceDownEff = Instantiate(checkerBoard.pieceDownEff, transform.position, Quaternion.identity, transform);

        FMODUnity.RuntimeManager.PlayOneShot("event:/Level/AoMan/qiziGround");
        moveStarted = false;
        yield break;
    }

    /// <summary>
    /// 駒がダメージを受けた時のヒットシェイク効果
    /// </summary>
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

    /// <summary>
    /// 駒はプレイヤーによって破壊される可能性がある
    /// プレイヤーを攻撃することもできる
    /// </summary>
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
    /// weaponスクリプトから呼び出し用
    /// </summary>
    public void WeaponAttackPlayer()
    {
        GameManager.Instance.TrickAttackPlayer(AttackPlayer);
    }
    /// <summary>
    /// プレイヤー攻撃ロジック
    /// </summary>
    /// <param name="curPlayer"> 取得現在のプレイヤー </param>
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
    /// weaponスクリプトから呼び出し用
    /// </summary>
    public void WeaponAttackEnemy(BaseEnemyControl baseEnemyControl)
    {
        GameManager.Instance.TrickAttackEnemy(AttackEnemy, baseEnemyControl);
    }

    /// <summary>
    /// 攻撃小怪邏輯
    /// </summary>
    /// <param name="enemy"> 取得現在小怪 </param>
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

    /// <summary>
    /// 受擊摧毀現在駒
    /// </summary>
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
        if (stateBar != null) stateBar.DestroyThis();   //關閉血條
        stateBar = null;
        gameObject.GetComponent<BoxCollider>().enabled = false; //讓玩家可以通過
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
    /// 因為King和Queen全部摧毀從而摧毀現在駒
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
        if (stateBar != null) stateBar.DestroyThis();   //關閉血條
        stateBar = null;
        gameObject.GetComponent<BoxCollider>().enabled = false; //讓玩家可以通過
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

    /// <summary>
    /// 調用王和后的摧毀效果
    /// </summary>
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

    /// <summary>
    /// 調用摧毀效果
    /// </summary>
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

    /// <summary>
    /// 檢查是否死亡,true代表已經死亡
    /// </summary>
    /// <returns></returns>
    public bool CheckisDead()
    {
        return isDead;
    }
}
