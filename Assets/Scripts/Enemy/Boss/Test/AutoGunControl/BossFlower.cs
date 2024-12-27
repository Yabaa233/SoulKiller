using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFlower : MonoBehaviour
{
    [Header("粒子特效")]
    public GameObject effect;
    [Header("间隔时间")]
    public float intervalTime;
    [Header("目标")]
    public Transform target;
    [Header("最大生命值")]
    public float maxHealth = 500.0f;
    public float curHealth = 500.0f;
    [Tooltip("受到近战攻击时伤害值")] public float getSwordDamage = 20.0f;
    [Tooltip("受到子弹攻击时伤害值")] public float getShotDamage = 1.0f;
    [Tooltip("受到魔法攻击时伤害值")] public float getMagicDamage = 15.0f;
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
        //获取当前玩家
        target = GameManager.Instance.currentPlayer.transform;
        //开始时向Boss注册
        boss = GameManager.Instance.currentBoss;
        boss.hasFlower = true;
        curHealth = maxHealth;
    }
    private void OnDestroy()
    {
        //被击败时向Boss注销
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

    /// <summary>
    /// 摧毁当前花
    /// </summary>
    private void BreakThisFlower()
    {
        Destroy(this.gameObject);
    }

    /// <summary>
    /// 花可能被玩家摧毁
    /// 也可以攻击玩家
    /// </summary>
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
                /*Debug.Log(effect.transform.GetChild(0).GetComponent<LineRenderer>().isVisible);*/
                effect.SetActive(false);
                //关闭粒子特效
                return;
                //粒子的碰撞体要关闭
            }
            if (!isWidth)//在粒子特效尚未达到最大宽度时(肯定是初启动状态)
            {
                if (!effect.activeSelf)//还没有启动话要先启动
                {
                    //播放蓄力动画，切换状态
                    effect.SetActive(true);
                    effect.GetComponent<LaserController>().LasertShoot();
                }
                effect.GetComponent<LaserController>().DrawLine();
                if (effect.transform.GetChild(0).GetComponent<LineRenderer>().widthMultiplier >= 10)//宽度达到最大值，可以进入正式的攻击状态
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
