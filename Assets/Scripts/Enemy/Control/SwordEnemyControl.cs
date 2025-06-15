using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class SwordEnemyControl : BaseEnemyControl
{
    // Start is called before the first frame update
    //新しい変数を追加する
    [Header("敵のステートマシン関連")]
    public SwordEnemyFSM enemyFSM;

    [Header("敵の攻撃状態SOテンプレート")]
    public SwordEmyStateData_SO tempEnemyStateData; 
    public SwordEmyStateData_SO TempEnemyStateData 
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
        // Debug.Log("既に実行されています");
        base.Start();
        SetTarget(GameManager.Instance.currentPlayer.transform);
    }

    private void EnemyInit(string tagStr)
    {
        //コンポーネントの取得
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        enemyFSM = transform.GetComponent<SwordEnemyFSM>();
        agent = GetComponent<NavMeshAgent>();
        _mainCamera = GameObject.FindWithTag("MainCamera");
        enemyBody = transform.Find("body");
        warningArea = GetComponent<SphereCollider>();
        attackArea = transform.Find("weapon");
        orientationObject = transform.Find("OrientationObject");


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
        // attackArea.gameObject.SetActive(false);//最初はこのコンポーネントを無効にします


        //データ同期を行う
        enemyFSM.parameter.enemyData = enemyData;
        enemyFSM.parameter.enemyStateData = tempEnemyStateData;
        enemyFSM.parameter.animator = animator;
        enemyFSM.parameter._mainCamera = _mainCamera;
        enemyFSM.parameter.body = enemyBody;
        enemyFSM.parameter.moveSpeed = moveSpeed;
        enemyFSM.parameter.agent = agent;
        enemyFSM.parameter.enemyPos = this.transform;
        enemyFSM.parameter.orientationObject = orientationObject;
    }

    protected new void Update(){
        base.Update();
        //各管理クラスのUpdateメソッド
        characterBuffManager.OnUpdate(Time.deltaTime);
        if(enemyFSM.parameter.ableAttact)
        {
            warningArea.enabled = false;
            // FaceToTarget(orientationObject);
        }else{
            warningArea.enabled = true;
        }
        // transform.LookAt(enemyFSM.parameter.target);
        // if(enemyFSM.parameter.isDead)//死亡状態に移行して実行します
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
