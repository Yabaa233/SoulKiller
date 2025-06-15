using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class StaffEnemyContorl : BaseEnemyControl
{
    [Header("敵の状態機関に関連する")]
    public long_DistanceFSM enemyFSM;
    public Transform firePoint;

    [Header("敵の攻撃状態SOテンプレート")]
    public StaffEmyStateData_SO tempEnemyStateData; 
    public StaffEmyStateData_SO TempEnemyStateData 
    {
        get
        {
            return tempEnemyStateData;
        }
    }

    private void Awake() {
        EnemyInit("Enemy");
    }
    protected new void Start() {
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

        //自分の属性の設定
        transform.tag = tagStr;

        //いくつかの関連する状態を初期化します
        characterBuffManager = new CharacterBuffManager();
        enemyData = new CharacterData(Instantiate(tempCharaterData));
        characterBuffManager.Init(E_ChararcterType.enemy);
        baseEnemyFSM  = enemyFSM;
        enemyData.currentComboAttack = 1;//攻撃倍率の設定
        moveSpeed = tempEnemyStateData.moveSpeed;
        agent.speed = tempEnemyStateData.moveSpeed;//移動速度の設定

        baseEnemyFSM  = enemyFSM;



        //データの同期
        enemyFSM.parameter.enemyData = enemyData;
        enemyFSM.parameter.enemyStateData = tempEnemyStateData;
        enemyFSM.parameter.enemyType = E_EnemyType.STAFF;
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

////// 追跡目標の設定

///</summary>
/// <param name="transform"></param>
    public override void SetTarget(Transform transform)
    {
        enemyFSM.parameter.target = transform;
    }
}
