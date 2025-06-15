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
    [Header("敵の剛体コンポーネント")]
    public Rigidbody rb;
    [Header("アニメーションコントローラー")]
    public Animator animator;
    [Tooltip("キャラクターBUffManager")] public CharacterBuffManager characterBuffManager;
    [Header("敵のAIのナビゲーションコンポーネント")] public NavMeshAgent agent;

    [Header("（使用しない）キャラクター数値テンプレート")]
    public CharacterData_SO tempCharaterData;
    [Tooltip("敵の属性を設定するSOファイルのインスタンス")] public CharacterData enemyData;
    [Tooltip("敵の一般的な状態マシン")] public BaseEnemyFSM baseEnemyFSM;
    [Header("敵の警戒範囲")] public SphereCollider warningArea;//警戒範囲
    [Header("敵の攻撃範囲（トリガー）")] public Transform attackArea;//攻撃範囲
    [Header("敵の足元の光輪")] public Transform orientationObject;
    [Header("敵の体")] public Transform enemyBody;
    [Header("敵はカメラに向かう必要があります")] public GameObject _mainCamera;
    [Header("敵の移動速度")] public float moveSpeed;
    [Header("敵のステータスパネルインスタンスの参照")] public GameObject statePanel;
    [Header("モンスターの素材")] public Material enemyMaterial;
    [Header("死亡しましたか？")] public bool isDead = false;
    public float biasY;//Spriteのバイアス値
    public Image hpImage;
    public Image hpEffect;
    public Image shieldImage;
    public Image shieldEffect;
    public Image shieldBackGround;
    // public Image buffImage;
    public RoomTrigger room; //モンスターが生成される部屋
    private bool isHurting = false; //現在、ダメージを受けている状態ですか？
    private bool isStoping = false; //アニメーションは一時停止中ですか？
    [Header("モンスターが被弾した際の振動回数")] public float enemyHurtCount = 2;
    [Header("モンスターが攻撃を受けた時のワード振動時間")] public float enemyHurtTime = 0.1f;
    [Header("振動とダメージ値の反比例係数")] public float enemyHurtPer = 200;
    [Header("被打撃効果とダメージの比率曲線")] public AnimationCurve hurtEffCurve;
    [Header("被ダメージ軽減効果係数")] public float hurtBackForce = 10;
    [Header("ダメージ値に反比例する撃退係数")] public float enemyBackHurtPer = 200;
    private Coroutine currentBackEff = null;
    private Vector3 forceBackDir;
    protected void Start()
    {
        room = transform.parent.parent.parent.GetComponent<RoomTrigger>();
        enemyMaterial = enemyBody.GetComponent<SpriteRenderer>().material;//材質を得る
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
        GenerateStatePanel();//ステータスパネルを設定する
    }

    ///<summary>


    ////// 小さなモンスターがダメージを受けたときのヒット振動効果


    ///</summary>
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
    /// モンスターがダメージを受ける表現方法
    /// </summary>
    /// <param name="damage"> ダメージ値 </param>
    /// <param name="isCritical"> クリティカルヒットかどうか </param>
    public virtual void Damaged(float damage, bool isCritical = false)
    {
        // enemyBody.GetComponent<SpriteRenderer>().color = Color.black;
        ShowGetHitEffect();
        if (!isHurting)
        {
            StartCoroutine(IE_EnemyHurt(damage));
        }
        PanelManager.Instance.GenerateDamageNum(damage, transform, isCritical);//ダメージ数値を生成する
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
    /// モンスターがダメージを受ける表現方法
    /// 撃退された攻撃を受け取りました
    /// </summary>
    /// <param name="damage"> ダメージ値 </param>
    /// <param name="attackerPos"> 攻撃者の座標 </param>
    /// <param name="isCritical"> クリティカルヒットかどうか </param>
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
        PanelManager.Instance.GenerateDamageNum(damage, transform, isCritical);//ダメージ数値を生成する
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
    /// ヒットバック効果
    /// </summary>
    /// <param name="damage"> ダメージ値 </param>
    /// <param name="forceDir"> ノックバック方向 </param>
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
    /// アニメーションの再生速度を回復する
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
        //リサイクルすべきものをいくつかリサイクルしました。
        if(statePanel!=null)
        {
            ObjectPool.Instance.RecycleObj("EnemyState", statePanel);
        }
        statePanel = null;
        //衝突ボックスを閉じて、死亡後の多重打撃を避けます。
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
            statePanel.transform.localPosition = PanelManager.Instance.UIFollow(this.transform, biasY * 0.5f);//フォローを続ける
            hpImage.fillAmount = enemyData.currentHealth / enemyData.maxHealth;//ヘルスバーのパーセンテージを設定する
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
        ShieldRipples shieldRipples = characterBuffManager.shieldRipples;//引用を得る
        shieldImage.fillAmount = shieldRipples.currentHealth / shieldRipples.maxHealth;//シールドのパーセンテージを設定する
        if (shieldEffect.fillAmount > shieldImage.fillAmount)
        {
            shieldEffect.fillAmount -= 0.015f;
        }
        else
        {
            shieldEffect.fillAmount = shieldImage.fillAmount;
        }
    }


    //シールドの可視性を設定します。これはBuff側で管理され、パフォーマンスを節約します。
    public void SetShieldVisble(bool state)
    {
        shieldImage.gameObject.SetActive(state);
        shieldEffect.gameObject.SetActive(state);
        shieldBackGround.gameObject.SetActive(state);
    }

    public void GenerateStatePanel()
    {
        //Spriteの高さを取得し、サブクラスに移動しました。
        biasY = enemyBody.GetComponent<SpriteRenderer>().bounds.size.y;

        // statePanel = PanelManager.Instance.GenerateStatePanel(this.transform,biasY);
        statePanel = ObjectPool.Instance.GetObject("EnemyState", true, true);
        statePanel.SetActive(true);
        statePanel.transform.localPosition = PanelManager.Instance.UIFollow(this.transform, biasY * 0.5f);//フォローを続ける
        hpImage = statePanel.transform.Find("Hp").GetComponent<Image>();
        hpEffect = statePanel.transform.Find("HpEffect").GetComponent<Image>();
        shieldImage = statePanel.transform.Find("Shield").GetComponent<Image>();
        shieldEffect = statePanel.transform.Find("ShieldEffect").GetComponent<Image>();
        shieldBackGround = statePanel.transform.Find("ShieldBack").GetComponent<Image>();
        //Buffアイコンを作成する
        // buffImage = statePanel.transform.Find("BuffImage").GetComponent<Image>();
        // CreateImage();


        ///まず、非表示に設定してください。
        SetShieldVisble(false);
    }

    ///<summary>


    ////// 画像を作成する


    ///</summary>
    // private void CreateImage()
    // {
        
    //     for (int i = 0;i<BuffDataManager.Instance.enemyCurrentBuff.Count;i++)
    //     {   
    //         Vector3 bias = new Vector3(i*12,0,0);
    //         GameObject gameObject = Instantiate(buffImage.gameObject,buffImage.transform.position + bias,Quaternion.identity,buffImage.transform.parent);
    //         Image curBuffImage = gameObject.GetComponent<Image>();
    //         curBuffImage.enabled = true;
    //         curBuffImage.color = Color.yellow;
    //         //画像を設定する
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
