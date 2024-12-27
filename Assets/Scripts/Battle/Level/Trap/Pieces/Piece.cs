using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    //被玩家攻击不计入战斗系统
    [Tooltip("最高血量")] public float maxHealth = 100.0f;
    [Tooltip("当前血量 被攻击固定掉血")] public float curHealth = 100.0f;
    [Tooltip("受到近战攻击时伤害值")] public float getSwordDamage = 20.0f;
    [Tooltip("受到子弹攻击时伤害值")] public float getShotDamage = 1.0f;
    [Tooltip("受到魔法攻击时伤害值")] public float getMagicDamage = 15.0f;

    [Tooltip("攻击力")] public float attack = 10;
    [Tooltip("击退玩家的力度")] public float forcePower = 10;
    [Header("上升相关参数")]
    [Tooltip("上升速度")] public float upSpeed = 10.0f;
    [Tooltip("上升高度")] public float targetY = 3.0f;
    [Tooltip("上升高度")] public float upWait = 1.5f;
    [Header("平移相关参数")]
    [Tooltip("平移速度")] public float mvoeSpeed = 10.0f;
    [Header("下落相关参数")]
    [Tooltip("下落速度")] public float downSpeed = 20.0f;
    [Tooltip("下落等待")] public float downWait = 1.5f;
    [Tooltip("下落落点位置")] public float downTargetY = 10.0f;
    [Header("当前状态相关参数")]
    public bool isMoveing = false;
    public bool isAttacking = false;
    [Header("是否是King或者Queen")] public bool isKingOrQueen = false;
    [Header("是King")] public bool isKing;
    [Header("是Queen")] public bool isQueen;
    [Header("是白色方")] public bool isWhite;

    [Header("棋子受击震动次数")] public float pieceHurtCount;
    [Header("棋子受击震动单次时间")] public float pieceHurtTime;
    [Header("棋子受击与伤害值关联反比例系数")] public float pieceHurtPer;
    [Header("受击时震动与伤害的比例曲线")] public AnimationCurve hurtEffCurve;
    private IEnumerator moveToTartgetPoint; //存储当前运动协程
    private bool moveStarted;
    private Collider weapon;    //攻击范围
    private CheckerBoard checkerBoard;  //棋盘脚本
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
        curHealth = maxHealth;  //初始化血量
        if (isKingOrQueen)
        {
            checkerBoard.kingOrQueenCount++;
        }
        checkerBoard.pieceCount++;
        stateBar = PanelManager.Instance.GenerateCommonStatePanel(this.transform);
        stateBar.SetPositionBias(new Vector3(0, 2, 0)); //设置血条偏移
        curPlayerTF = GameManager.Instance.currentPlayer.transform;
    }
    private void OnDisable()
    {
        if (moveStarted)
        {
            Debug.Log("正在移动被摧毁了，恢复canNext");
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
    /// 棋子移动
    /// </summary>
    /// <param name="targetPosition"> 移动到的目标点 </param>
    public void Move(Vector3 targetPosition)
    {
        moveStarted = true;
        moveToTartgetPoint = MoveToTartgetPoint(targetPosition);
        StartCoroutine(moveToTartgetPoint);
    }

    /// <summary>
    /// 移动控制协程
    /// </summary>
    /// <param name="target"> 目标点 </param>
    /// <returns></returns>
    IEnumerator MoveToTartgetPoint(Vector3 target)
    {
        float time = 0; //计时用
        //抬起
        if (pieceDownEff != null) Destroy(pieceDownEff);
        isMoveing = true;   //开始移动，玩家无法攻击到，也无法攻击玩家
        while (transform.position.y < targetY)
        {
            transform.Translate(Vector3.up * Time.deltaTime * upSpeed, Space.World);
            yield return null;
        }
        while (time < upWait) //等待
        {
            time += Time.deltaTime;
            yield return null;
        }
        time = 0;
        //平移
        Vector3 curtarget = target;
        curtarget.y = targetY;  //防止上下位移
        Vector3 dir = curtarget - transform.position;
        while (dir.magnitude > 0.5f)
        {
            transform.Translate(dir * Time.deltaTime * mvoeSpeed, Space.World);
            dir = curtarget - transform.position;
            yield return null;
        }
        //下落
        isAttacking = true;     //攻击状态开始
        while (time < downWait) //下落等待
        {
            time += Time.deltaTime;
            yield return null;
        }
        weapon.enabled = true;  //开启碰撞体
        isMoveing = false;  //玩家可以攻击到了
        while (transform.position.y > downTargetY)
        {
            transform.Translate(Vector3.down * Time.deltaTime * downSpeed, Space.World);
            dir = curtarget - transform.position;
            yield return null;
        }
        checkerBoard.CanNext(true);    //可以下一步棋子
        weapon.enabled = false; //关闭碰撞体
        isAttacking = false;    //攻击状态结束
        pieceDownEff = Instantiate(checkerBoard.pieceDownEff, transform.position, Quaternion.identity, transform);

        FMODUnity.RuntimeManager.PlayOneShot("event:/Level/AoMan/qiziGround");
        moveStarted = false;
        yield break;
    }

    /// <summary>
    /// 棋子受伤时的受击震动效果
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
    /// 棋子可能被玩家摧毁
    /// 也可以攻击玩家
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
    /// 供weapon脚本调用
    /// </summary>
    public void WeaponAttackPlayer()
    {
        GameManager.Instance.TrickAttackPlayer(AttackPlayer);
    }
    /// <summary>
    /// 攻击玩家逻辑
    /// </summary>
    /// <param name="curPlayer"> 获取当前玩家 </param>
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
    /// 供weapon脚本调用
    /// </summary>
    public void WeaponAttackEnemy(BaseEnemyControl baseEnemyControl)
    {
        GameManager.Instance.TrickAttackEnemy(AttackEnemy, baseEnemyControl);
    }

    /// <summary>
    /// 攻击小怪逻辑
    /// </summary>
    /// <param name="enemy"> 获取当前小怪 </param>
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
    /// 受击摧毁当前棋子
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
        if (stateBar != null) stateBar.DestroyThis();   //关闭血条
        stateBar = null;
        gameObject.GetComponent<BoxCollider>().enabled = false; //让玩家可以通过
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
    /// 因为King和Queen全部摧毁从而摧毁当前棋子
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
        if (stateBar != null) stateBar.DestroyThis();   //关闭血条
        stateBar = null;
        gameObject.GetComponent<BoxCollider>().enabled = false; //让玩家可以通过
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
    /// 调用王和后的摧毁效果
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
    /// 调用摧毁效果
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
    /// 检查是否死亡,true代表已经死亡
    /// </summary>
    /// <returns></returns>
    public bool CheckisDead()
    {
        return isDead;
    }
}
