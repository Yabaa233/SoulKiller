using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


// [RequireComponent(typeof(Animator))]
// [RequireComponent(typeof(Rigidbody))]
// [RequireComponent(typeof(NavMeshAgent))]
public class GunEnemyControl : BaseEnemyControl
{
    // [Header("敵のリジッドボディコンポーネント")]
    // public Rigidbody rb;
    // new public Collider collider;
    // [Header("アニメーションコントローラー")]
    // public Animator animator;
    // [Header("敵のステートマシン関連")]
    // public GunEnemyFSM enemyFSM;
    // [Header("使用しないキャラクター数値テンプレート")]
    // public GunEnemyData_SO tempCharaterData;
    // [Tooltip("敵の属性設定のSOファイルインスタンス")]public GunEnemyData enemyData;
    // [Tooltip("キャラクターBUffManager")]public CharacterBuffManager characterBuffManager;
    // [Header("敵AIのナビゲーションコンポーネント")] public NavMeshAgent agent;

    [Header("敵のステートマシン関連")]
    public long_DistanceFSM enemyFSM;
    [Header("発射点")]
    public Transform firePoint;

    [Header("敵の攻撃状態SOテンプレート")]
    public GunEmyStateData_SO tempEnemyStateData; 
    public GunEmyStateData_SO TempEnemyStateData 
    {
        get
        {
            return tempEnemyStateData;
        }
    }

    private void Awake() {
        EnemyInit("Enemy");
    }

    protected new void Start()
    {
        base.Start();
        SetTarget(GameManager.Instance.currentPlayer.transform);
    }

    private void EnemyInit(string tagStr)
    {
        //コンポーネントの取得
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        enemyFSM = transform.GetComponent<long_DistanceFSM>();
        agent = GetComponent<NavMeshAgent>();
        _mainCamera = GameObject.FindWithTag("MainCamera");
        enemyBody = transform.Find("body");
        warningArea = GetComponent<SphereCollider>();
        attackArea = transform.Find("weapon");
        orientationObject = transform.Find("OrientationObject");
        firePoint = transform.Find("firepoint");

        //自身の属性を設定する
        transform.tag = tagStr;

        //関連状態の初期化を行います
        characterBuffManager = new CharacterBuffManager();
        enemyData = new CharacterData(Instantiate(tempCharaterData));
        characterBuffManager.Init(E_ChararcterType.enemy);
        baseEnemyFSM  = enemyFSM;
        enemyData.currentComboAttack = 1;//攻撃倍率を設定する
        moveSpeed = tempEnemyStateData.moveSpeed;//移動速度の初期化
        agent.speed = tempEnemyStateData.moveSpeed;//移動速度を設定します


        //データ同期を行う
        enemyFSM.parameter.enemyData = enemyData;
        enemyFSM.parameter.enemyStateData = tempEnemyStateData;
        enemyFSM.parameter.enemyType = E_EnemyType.GUN;
        enemyFSM.parameter.animator = animator;
        enemyFSM.parameter._mainCamera = _mainCamera;
        enemyFSM.parameter.body = enemyBody;
        enemyFSM.parameter.agent = agent;
        enemyFSM.parameter.firePoint = firePoint;
        enemyFSM.parameter.enemyPos = this.transform;
        enemyFSM.parameter.moveSpeed = moveSpeed;
        enemyFSM.parameter.orientationObject = orientationObject;
    }

    protected new void Update() {
        base.Update();
        //各管理クラスのUpdateメソッド
        characterBuffManager.OnUpdate(Time.deltaTime);

        // characterBuffManager.AddBuff(new HpUp(gameObject,characterBuffManager.type));//HPを上げる方法
        if(enemyFSM.parameter.ableAttact)
        {
            warningArea.enabled = false;
            // FaceToTarget(orientationObject);
        }else{
            warningArea.enabled = true;
        }
    }

    public override void Damaged(float damage,bool isCritical = false)
    {
        base.Damaged(damage,isCritical);
        enemyFSM.parameter.getHit = true;
    }


    public override void Die()
    {
        base.Die();
        enemyFSM.parameter.isDead = true;
    }
///<summary>

////// 追跡目標を設定する

///</summary>
/// <param name="transform"></param>
    public override void SetTarget(Transform transform)
    {
        enemyFSM.parameter.target = transform;
    }

}