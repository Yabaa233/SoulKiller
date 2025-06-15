using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFlower : MonoBehaviour
{
    [Header("パーティクルエフェクト")]
    public GameObject effect;
    [Header("Interval Time")]
    public float intervalTime;
    [Header("Objective")]
    public Transform target;
    [Header("Maximum Health Points")]
    public float maxHealth = 500.0f;
    public float curHealth = 500.0f;
    [Tooltip("Damage value when attacked in close combat")] public float getSwordDamage = 20.0f;
    [Tooltip("Damage value when hit by a bullet")] public float getShotDamage = 1.0f;
    [Tooltip("Damage value when attacked by magic")] public float getMagicDamage = 15.0f;
    private float endTimer;
    private Animation anim;
    private bool isWidth, isEnd;
    private BossControl boss;
    private StateBar stateBar;
    private bool isDead = false;
    private void Start()
    {
        endTimer = Time.time;
        isWidth = false;
        isEnd = false;
        stateBar = PanelManager.Instance.GenerateCommonStatePanel(this.transform);
        stateBar.SetPositionBias(new Vector3(0, 2, 0));
    }

    private void OnEnable()
    {
        //現在のプレイヤーを取得する
        target = GameManager.Instance.currentPlayer.transform;
        //ボスに登録を開始します
        boss = GameManager.Instance.currentBoss;
        boss.hasFlower = true;
        curHealth = maxHealth;
    }
    private void OnDestroy()
    {
        //敗北時にボスにログアウトします
        if (stateBar != null)
        {
            stateBar.DestroyThis();
            stateBar = null;
        }
        if (boss != null)
        {
            boss.hasFlower = false;
            boss.bossCD.canSummonAttack.flag = false;
        }
    }

    ///<summary>


    ////// 現在の花を破壊する


    ///</summary>
    private void BreakThisFlower()
    {
        Destroy(this.gameObject);
    }

    ///<summary>


    ////// 花はプレイヤーによって破壊される可能性があります。
    /// プレイヤーを攻撃することも可能です


    ///</summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerWeapon"))
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Level/Jidu/hurt");
            if (other.tag == "PlayerWeapon")
            {
                curHealth -= getSwordDamage;
            }
            else if (other.tag == "PlayerBullet")
            {
                curHealth -= getShotDamage;
            }
            else if (other.tag == "PlayerMagic")
            {
                curHealth -= getMagicDamage;
            }
            else
            {
                return;
            }
            // FMODUnity.RuntimeManager.PlayOneShot("event:/Level/AoMan/qiziHit");
            if (curHealth <= 0)
            {
                if (stateBar != null)
                {
                    stateBar.DestroyThis();
                    stateBar = null;
                }
                Vector3 dir = (transform.position - target.position).normalized * 8;
                dir.y = 10.0f;
                gameObject.GetComponent<BoxCollider>().enabled = false;
                GetComponent<Rigidbody>().AddForce(dir, ForceMode.Impulse);
                GetComponent<Rigidbody>().useGravity = true;
                if (effect.activeSelf)
                {
                    isDead = true;
                    effect.SetActive(false);
                }
                Invoke("BreakThisFlower", 5.0f);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (stateBar != null) stateBar.UpdateState(curHealth, maxHealth);
        if (curHealth <= 0 || isDead)
        {
            effect.SetActive(false);
            return;
        }
        if (Time.time >= endTimer + intervalTime)
        {
            // print("effect prepare "+timer);
            if (effect.activeSelf && isEnd)
            {
                isEnd = false;
                isWidth = false;
                endTimer = Time.time;
                effect.SetActive(false);
                //パーティクルエフェクトをオフにする
                return;
                //粒子の衝突装置を閉じる必要があります。
            }
            if (!isWidth)//パーティクルエフェクトが最大幅に達していない時（間違いなく初期起動状態）
            {
                if (!effect.activeSelf)//まだ起動していない場合は、先に起動してください。
                {
                    //力を蓄えるアニメーションを再生し、状態を切り替えます。
                    effect.SetActive(true);
                    effect.GetComponent<LaserController>().LasertShoot();
                }
                effect.GetComponent<LaserController>().DrawLine();
                if (effect.transform.GetChild(0).GetComponent<LineRenderer>().widthMultiplier >= 10)//幅が最大値に達し、正式な攻撃状態に入ることができます。
                {
                    isWidth = true;
                }
            }
            else
            {
                effect.GetComponent<LaserController>().DrawLine();
                if (!effect.GetComponent<BoxCollider>().enabled)
                {
                    effect.GetComponent<BoxCollider>().enabled = true;
                }
                if (effect.transform.GetChild(0).GetComponent<LineRenderer>().widthMultiplier < 10)
                {
                    if (effect.GetComponent<BoxCollider>().enabled)
                    {
                        effect.GetComponent<BoxCollider>().enabled = false;
                    }
                }
                if (effect.transform.GetChild(0).GetComponent<LineRenderer>().widthMultiplier < 0.2f)
                {
                    isEnd = true;
                }
            }
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                Quaternion.LookRotation(new Vector3(target.position.x - transform.position.x, 0, target.position.z - transform.position.z)),
                                Time.deltaTime);
        }
    }
}
