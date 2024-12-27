using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FlowerRotate : MonoBehaviour
{
    [Header("光炮粒子特效")]
    public GameObject effect;
    [Header("大花死亡爆炸特效")]
    public GameObject boom;
    [Header("间隔时间")]
    public float intervalTime;
    [Header("大花血量")]
    public float maxHealth;
    public float currentHealth;
    [Header("骷髅手臂")]
    public GameObject hands;
    [Header("停止通知")]
    public UnityAction broadcastStart, broadcastEnd, broadcastDead, broadcastDestory;
    [Tooltip("受到近战攻击时伤害值")] public float getSwordDamage = 20.0f;
    [Tooltip("受到子弹攻击时伤害值")] public float getShotDamage = 1.0f;
    [Tooltip("受到魔法攻击时伤害值")] public float getMagicDamage = 15.0f;
    [Header("目标")]
    private Transform target;
    private float endTimer;
    private Vector3 decline, beginPosition;
    private bool isWidth, isEnd, trapStart;
    private StateBar stateBar;
    private bool isAttack;
    RoomTrigger roomTrigger;
    private Transform kulou;
    private void Start()
    {
        kulou = transform.parent.GetChild(0);
        roomTrigger = transform.parent.parent.GetComponent<RoomTrigger>();
        roomTrigger.clearCheck += () => currentHealth <= 0;  //通关条件
        currentHealth = maxHealth;
        decline = new Vector3(0, -7, 0);
        target = GameManager.Instance.currentPlayer.transform;
        isWidth = false;
        isEnd = false;
        beginPosition = hands.transform.position;
        endTimer = 0;
        stateBar = PanelManager.Instance.GenerateCommonStatePanel(this.transform);
        stateBar.SetPositionBias(new Vector3(0, 5, 0) + transform.forward * 15);
        stateBar.SetlocalScale(Vector3.one * 3);
    }
    private void OnEnable()
    {
        transform.parent.GetComponent<TrapTrigger>().openTarp += () => trapStart = true;
    }
    private void OnDestroy()
    {
        if (stateBar != null)
        {
            stateBar.DestroyThis();
            stateBar = null;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (currentHealth > 0 && stateBar != null) stateBar.UpdateState(currentHealth, maxHealth);
        if (trapStart)
        {
            if (currentHealth > 0)
            {
                if (endTimer >= intervalTime)
                {
                    if (effect.activeSelf && isEnd)
                    {
                        isEnd = false;
                        isWidth = false;
                        endTimer = 0;
                        effect.SetActive(false);
                        if (broadcastEnd != null)
                        {
                            broadcastEnd();
                        }
                        //关闭粒子特效
                        return;
                        //粒子的碰撞体要关闭
                    }
                    if (!isWidth)//在粒子特效尚未达到最大宽度时(肯定是初启动状态)
                    {
                        if (broadcastStart != null)
                        {
                            broadcastStart();
                        }
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
                        hands.transform.position = Vector3.Lerp(beginPosition, beginPosition + decline, effect.transform.GetChild(0).GetComponent<LineRenderer>().widthMultiplier / 10);
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
                            hands.transform.position = Vector3.Lerp(beginPosition + decline, beginPosition, (10 - effect.transform.GetChild(0).GetComponent<LineRenderer>().widthMultiplier) / 9.8f);
                            if (effect.GetComponent<BoxCollider>().enabled)
                            {
                                effect.GetComponent<BoxCollider>().enabled = false;
                            }
                        }
                        if (effect.transform.GetChild(0).GetComponent<LineRenderer>().widthMultiplier < 0.2f)
                        {
                            isEnd = true;
                            isAttack = false;
                            kulou.GetComponent<SkeletonShow>().CloseEyes();
                        }
                        else
                        {
                            if (!isAttack)
                            {
                                isAttack = true;
                                kulou.GetComponent<SkeletonShow>().OpenEyes();
                            }
                        }
                    }
                }
                else
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation,
                                        Quaternion.LookRotation(new Vector3(target.position.x - transform.position.x, 0, target.position.z - transform.position.z)),
                                        Time.deltaTime);
                    endTimer += Time.deltaTime;
                }
            }
            else
            {
                kulou.GetComponent<SkeletonShow>().Move();
                Destroy(gameObject,5f);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (currentHealth > 0)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("PlayerWeapon"))
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/Level/JiDu/hurt");
                if (other.tag == "PlayerWeapon")
                {
                    currentHealth -= getSwordDamage;
                }
                else if (other.tag == "PlayerBullet")
                {
                    currentHealth -= getShotDamage;
                }
                else if (other.tag == "PlayerMagic")
                {
                    currentHealth -= getMagicDamage;
                }
                else
                {
                    return;
                }
                if (currentHealth <= 0)
                {
                    if (stateBar != null)
                    {
                        stateBar.DestroyThis();
                        stateBar = null;
                    }
                    Instantiate(boom, transform);
                    gameObject.GetComponent<BoxCollider>().enabled = false;
                    gameObject.transform.GetChild(0).gameObject.AddComponent<Rigidbody>();
                    gameObject.transform.GetChild(0).gameObject.GetComponent<Rigidbody>().useGravity = true;
                    gameObject.transform.GetChild(0).gameObject.GetComponent<Rigidbody>().AddForce(-transform.forward * 10 + transform.up * 20, ForceMode.Impulse);
                    if (effect.activeSelf)
                    {
                        effect.SetActive(false);
                    }
                    if (broadcastDead != null)
                    {
                        broadcastDead();
                    }
                    roomTrigger.TrapClear();
                }
            }
        }
    }
}
