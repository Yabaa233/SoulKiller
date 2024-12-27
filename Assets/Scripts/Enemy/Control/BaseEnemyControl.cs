using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class BaseEnemyControl : MonoBehaviour
{
    [Header("敌人刚体组件")]
    public Rigidbody rb;
    [Header("动画控制器")]
    public Animator animator;
    [Tooltip("角色BUffManager")] public CharacterBuffManager characterBuffManager;
    [Header("敌人AI的导航组件")] public NavMeshAgent agent;

    [Header("角色数值模板（不会使用的）")]
    public CharacterData_SO tempCharaterData;
    [Tooltip("敌人属性配置SO文件的实例")] public CharacterData enemyData;
    [Tooltip("敌人通用状态机")] public BaseEnemyFSM baseEnemyFSM;
    [Header("敌人的警戒范围")] public SphereCollider warningArea;//警戒范围
    [Header("敌人的攻击范围(Trigger)")] public Transform attackArea;//攻击范围
    [Header("敌人的脚底光圈")] public Transform orientationObject;
    [Header("敌人的身体(body)")] public Transform enemyBody;
    [Header("敌人需要朝向的相机")] public GameObject _mainCamera;
    [Header("敌人的移动速度")] public float moveSpeed;
    [Header("敌人状态面板实例的引用")] public GameObject statePanel;
    [Header("小怪身上的材质")] public Material enemyMaterial;
    [Header("是否死亡")] public bool isDead = false;
    public float biasY;//Sprite的偏置值
    public Image hpImage;
    public Image hpEffect;
    public Image shieldImage;
    public Image shieldEffect;
    public Image shieldBackGround;
    // public Image buffImage;
    public RoomTrigger room; //小怪生成的房间
    private bool isHurting = false; //是否正处于受击状态
    private bool isStoping = false; //是否正处于动画暂停
    [Header("怪物受击震动次数")] public float enemyHurtCount = 2;
    [Header("怪物受击单词震动时间")] public float enemyHurtTime = 0.1f;
    [Header("震动与伤害值反比例系数")] public float enemyHurtPer = 200;
    [Header("受击效果与伤害的比例曲线")] public AnimationCurve hurtEffCurve;
    [Header("受击退效果系数")] public float hurtBackForce = 10;
    [Header("击退与伤害值反比例系数")] public float enemyBackHurtPer = 200;
    private Coroutine currentBackEff = null;
    private Vector3 forceBackDir;
    protected void Start()
    {
        room = transform.parent.parent.parent.GetComponent<RoomTrigger>();
        enemyMaterial = enemyBody.GetComponent<SpriteRenderer>().material;//得到材质
    }

    private void ShowGetHitEffect()
    {
        GameObject enemyGetHit = ObjectPool.Instance.GetObject("Enemy_Attacked", EffectManager.Instance.transform, true, false);
        enemyGetHit.transform.position = transform.position + Vector3.up;
        enemyGetHit.SetActive(true);
        EffectManager.Instance.LetRecycleEffect("Enemy_Attacked", enemyGetHit, 1.5f);
    }

    private void OnEnable()
    {
        statePanel = null;
        GenerateStatePanel();//设置状态面板
    }

    /// <summary>
    /// 小怪受伤时的受击震动效果
    /// </summary>
    /// <param name="damage"></param>
    /// <returns></returns>
    IEnumerator IE_EnemyHurt(float damage)
    {
        isHurting = true;
        float time = 0;
        Vector3 deviation = enemyBody.position - GameManager.Instance.currentPlayer.transform.position;
        deviation.y = 0;
        deviation.Normalize();
        deviation *= hurtEffCurve.Evaluate(damage / enemyHurtPer);
        // enemyMaterial.SetFloat("_TimeStamp", Time.timeSinceLevelLoad + SceneLoadManager.Instance.asyncTime);
        // Debug.Log(Time.timeSinceLevelLoad);

        for (int i = 0; i < enemyHurtCount; i++)
        {
            enemyMaterial.SetColor("_Color", Color.red);
            enemyBody.position += deviation;
            while (time < enemyHurtTime)
            {
                time += Time.deltaTime;
                yield return null;
            }
            time = 0;
            enemyBody.position = transform.position;
            enemyMaterial.SetColor("_Color", Color.black);
            while (time < enemyHurtTime)
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
    /// 小怪受击效果表现方法
    /// </summary>
    /// <param name="damage"> 伤害值 </param>
    /// <param name="isCritical"> 是否暴击 </param>
    public virtual void Damaged(float damage, bool isCritical = false)
    {
        // enemyBody.GetComponent<SpriteRenderer>().color = Color.black;
        ShowGetHitEffect();
        if (!isHurting)
        {
            StartCoroutine(IE_EnemyHurt(damage));
        }
        PanelManager.Instance.GenerateDamageNum(damage, transform, isCritical);//产生伤害数字
        if (enemyData.currentHealth <= 0 && !isDead)
        {
            Die();
        }
        if (!isStoping)
        {
            isStoping = true;
            animator.speed = 0.1f;
            Invoke("ResetAnimatorSpeed", enemyData.BaseStopTime);
        }
    }

    /// <summary>
    /// 小怪受击效果表现方法
    /// 收到有击退的攻击
    /// </summary>
    /// <param name="damage"> 伤害值 </param>
    /// <param name="attackerPos"> 攻击方坐标 </param>
    /// <param name="isCritical"> 是否暴击 </param>
    public virtual void Damaged(float damage, Vector3 attackerPos, bool isCritical = false)
    {
        // enemyBody.GetComponent<SpriteRenderer>().color = Color.black;
        ShowGetHitEffect();
        if (currentBackEff != null)
        {
            StopCoroutine(currentBackEff);
        }
        agent.enabled = false;
        rb.velocity = Vector3.zero;
        forceBackDir = transform.position - attackerPos;
        forceBackDir.y = 0;
        currentBackEff = StartCoroutine(IE_HurtForceBack(damage, forceBackDir.normalized));
        PanelManager.Instance.GenerateDamageNum(damage, transform, isCritical);//产生伤害数字
        if (enemyData.currentHealth <= 0 && !isDead)
        {
            Die();
            return;
        }
        if (!isHurting)
        {
            StartCoroutine(IE_EnemyHurt(damage));
        }
        if (!isStoping)
        {
            isStoping = true;
            animator.speed = 0.3f;
            Invoke("ResetAnimatorSpeed", enemyData.BaseStopTime);
        }
    }

    /// <summary>
    /// 受击击退效果
    /// </summary>
    /// <param name="damage"> 伤害值 </param>
    /// <param name="forceDir"> 击退方向 </param>
    /// <returns></returns>
    IEnumerator IE_HurtForceBack(float damage, Vector3 forceDir)
    {
        float time = 0;
        rb.AddForce(forceDir * hurtEffCurve.Evaluate(damage / enemyBackHurtPer) * hurtBackForce, ForceMode.Impulse);
        // Debug.Log(forceDir * hurtEffCurve.Evaluate(damage / enemyBackHurtPer) * hurtBackForce);
        while (time < enemyData.BaseStopTime)
        {
            rb.velocity /= 10;
            time += Time.deltaTime;
            yield return null;
        }
        agent.enabled = true;
        yield break;
    }

    /// <summary>
    /// 恢复动画播放速度
    /// </summary>
    private void ResetAnimatorSpeed()
    {
        if (animator != null)
        {
            animator.speed = 1.0f;
        }
        isStoping = false;
    }

    public virtual void Die()
    {
        room.GetComponent<RoomTrigger>().EnemyDie();
        //回收掉一些应该回收的东西
        if(statePanel!=null)
        {
            ObjectPool.Instance.RecycleObj("EnemyState", statePanel);
        }
        statePanel = null;
        //关闭碰撞盒，避免死亡后多次敲击
        // this.transform.Find("bodyCollider").GetComponent<Collider>().enabled = false;
        isDead = true;
    }

    public virtual void SetTarget(Transform transform)
    {

    }

    protected void Update()
    {
        UpdateState();
    }


    public void UpdateState()
    {
        if (statePanel != null)
        {
            statePanel.transform.localPosition = PanelManager.Instance.UIFollow(this.transform, biasY * 0.5f);//保持跟随
            hpImage.fillAmount = enemyData.currentHealth / enemyData.maxHealth;//设置血条的百分比
            if (hpEffect.fillAmount > hpImage.fillAmount)
            {
                hpEffect.fillAmount -= 0.015f;
            }
            else
            {
                hpEffect.fillAmount = hpImage.fillAmount;
            }

            if (characterBuffManager.FindBuff(E_BuffKind.ShieldBuff))
            {
                UpDateShield();
            }
        }
    }

    public void UpDateShield()
    {
        ShieldRipples shieldRipples = characterBuffManager.shieldRipples;//得到引用
        shieldImage.fillAmount = shieldRipples.currentHealth / shieldRipples.maxHealth;//设置护盾百分比
        if (shieldEffect.fillAmount > shieldImage.fillAmount)
        {
            shieldEffect.fillAmount -= 0.015f;
        }
        else
        {
            shieldEffect.fillAmount = shieldImage.fillAmount;
        }
    }


    //设置Shiled的可见性，这个是Buff那边管理的，节省性能
    public void SetShieldVisble(bool state)
    {
        shieldImage.gameObject.SetActive(state);
        shieldEffect.gameObject.SetActive(state);
        shieldBackGround.gameObject.SetActive(state);
    }

    public void GenerateStatePanel()
    {
        //获取Sprite的高度,放到子类里面去了
        biasY = enemyBody.GetComponent<SpriteRenderer>().bounds.size.y;

        // statePanel = PanelManager.Instance.GenerateStatePanel(this.transform,biasY);
        statePanel = ObjectPool.Instance.GetObject("EnemyState", true, true);
        statePanel.SetActive(true);
        statePanel.transform.localPosition = PanelManager.Instance.UIFollow(this.transform, biasY * 0.5f);//保持跟随
        hpImage = statePanel.transform.Find("Hp").GetComponent<Image>();
        hpEffect = statePanel.transform.Find("HpEffect").GetComponent<Image>();
        shieldImage = statePanel.transform.Find("Shield").GetComponent<Image>();
        shieldEffect = statePanel.transform.Find("ShieldEffect").GetComponent<Image>();
        shieldBackGround = statePanel.transform.Find("ShieldBack").GetComponent<Image>();
        //创建Buff图标
        // buffImage = statePanel.transform.Find("BuffImage").GetComponent<Image>();
        // CreateImage();


        ///先设置为隐藏
        SetShieldVisble(false);
    }

    /// <summary>
    /// 创建图片
    /// </summary>
    // private void CreateImage()
    // {
        
    //     for (int i = 0;i<BuffDataManager.Instance.enemyCurrentBuff.Count;i++)
    //     {   
    //         Vector3 bias = new Vector3(i*12,0,0);
    //         GameObject gameObject = Instantiate(buffImage.gameObject,buffImage.transform.position + bias,Quaternion.identity,buffImage.transform.parent);
    //         Image curBuffImage = gameObject.GetComponent<Image>();
    //         curBuffImage.enabled = true;
    //         curBuffImage.color = Color.yellow;
    //         //设置图片
    //         E_BuffKind curBuffKind = BuffDataManager.Instance.enemyCurrentBuff[i].buffKind;
    //         foreach (var buffItem in PanelManager.Instance.buffInfoListSO.buffItems)
    //         {
    //             if (buffItem.buffKind == curBuffKind)
    //             {
    //                 buffImage.sprite = buffItem.buffSprite;
    //             }
    //         }
    //     }
    // }


    private void OnDestroy()
    {
        if(statePanel!=null)
        {
            ObjectPool.Instance.RecycleObj("EnemyState", statePanel);
        }
        statePanel = null;
    }

}
