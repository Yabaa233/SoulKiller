using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FlowerRotate : MonoBehaviour
{
    [Header("Light Cannon Particle Effects")]
    public GameObject effect;
    [Header("Large Flower Death Explosion Special Effect")]
    public GameObject boom;
    [Header("Interval Time")]
    public float intervalTime;
    [Header("Large Flower's Health Points")]
    public float maxHealth;
    public float currentHealth;
    [Header("Skeleton arm")]
    public GameObject hands;
    [Header("Stop notifications")]
    public UnityAction broadcastStart, broadcastEnd, broadcastDead, broadcastDestory;
    [Tooltip("Damage value when attacked in close combat")] public float getSwordDamage = 20.0f;
    [Tooltip("Damage value when hit by a bullet")] public float getShotDamage = 1.0f;
    [Tooltip("Damage value when attacked by magic")] public float getMagicDamage = 15.0f;
    [Header("Objective")]
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
        roomTrigger.clearCheck += () => currentHealth <= 0;  //クリア条件
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
                        //パーティクルエフェクトをオフにする
                        return;
                        //粒子の衝突装置を閉じる必要があります。
                    }
                    if (!isWidth)//パーティクルエフェクトが最大幅に達していない時（間違いなく初期起動状態）
                    {
                        if (broadcastStart != null)
                        {
                            broadcastStart();
                        }
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
