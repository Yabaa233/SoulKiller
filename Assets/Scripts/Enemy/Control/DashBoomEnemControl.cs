using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// ダッシュ爆発で敵を制圧
/// </summary>
public class DashBoomEnemControl : BaseEnemyControl
{
    
    [Header("敵のステートマシン関連")]

    public DashBoomEnemyFSM enemyFSM;

    [Header("敵の攻撃状態SOテンプレート")]
    public DBoomEmyStateData_SO tempEnemyStateData; 
    public DBoomEmyStateData_SO TempEnemyStateData 
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
        enemyFSM = transform.GetComponent<DashBoomEnemyFSM>();
        agent = GetComponent<NavMeshAgent>();
         _mainCamera = GameObject.FindWithTag("MainCamera");
        enemyBody = transform.Find("body");
        warningArea = GetComponent<SphereCollider>();
        attackArea = transform.Find("weapon");
        orientationObject = transform.Find("OrientationObject");

        baseEnemyFSM  = enemyFSM;


        //自身の属性を設定する
        transform.tag = tagStr;

        //関連状態の初期化を行います
        characterBuffManager = new CharacterBuffManager();
        enemyData = new CharacterData(Instantiate(tempCharaterData));
        characterBuffManager.Init(E_ChararcterType.enemy);
        baseEnemyFSM = enemyFSM;
        enemyData.currentComboAttack = 1;//攻撃倍率を設定する
        moveSpeed = tempEnemyStateData.moveSpeed;
        agent.speed = tempEnemyStateData.moveSpeed;//移動速度を設定します


        //データ同期を行う
        enemyFSM.parameter.enemyData = enemyData;
        enemyFSM.parameter.enemyStateData = tempEnemyStateData;
        enemyFSM.parameter.animator = animator;
        enemyFSM.parameter._mainCamera = _mainCamera;
        enemyFSM.parameter.body = enemyBody;
        enemyFSM.parameter.agent = agent;
        enemyFSM.parameter.enemyPos = this.transform;
        enemyFSM.parameter.moveSpeed = moveSpeed;
    }

    protected new void Update(){
        base.Update();
        //各管理クラスのUpdateメソッド
        characterBuffManager.OnUpdate(Time.deltaTime);
        if(enemyFSM.parameter.ableAttact || enemyFSM.parameter.getHit)
        {
            warningArea.enabled = false;
            // FaceToTarget(orientationObject);
        }else{
            warningArea.enabled = true;
        }

        // Debug.Log("222:" + enemyBody.localRotation);
        // if(enemyFSM.parameter.isDead)
        // {
        //     Destroy(gameObject);
        // }
    }
    
    public override void Damaged(float damage,bool isCritical = false)
    {
        base.Damaged(damage,isCritical);
        enemyFSM.parameter.getHit = true;
    }

    public override void Die()
    {
        base.Die();
        enemyFSM.parameter.isBoom = true;
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
