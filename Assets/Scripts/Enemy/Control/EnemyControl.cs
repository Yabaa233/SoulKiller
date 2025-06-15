using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyControl : BaseEnemyControl
{
    [Header("敵のステートマシン関連")]
    public EnemyFSM enemyFSM;

    //属性状態記述クラスファイル
    [Header("敵の攻撃状態SOテンプレート")]
    public EzEmyStateData_SO tempEnemyStateData; 
    public EzEmyStateData_SO TempEnemyStateData 
    {
        get
        {
            return tempEnemyStateData;
        }
    }


    private void Awake()
    {
        EnemyInit("Enemy");
    }

    protected new void Start() {
        base.Start();
        SetTarget(GameManager.Instance.currentPlayer.transform);
    }

    ///<summary>


    ////// 敵の機能コンポーネントを初期化する


    ///</summary>
    /// <param name="tagStr"></param>
    private void EnemyInit(string tagStr)
    {
        //コンポーネントの取得
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        enemyFSM = transform.GetComponent<EnemyFSM>();
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
        // agent.speed = tempEnemyStateData.moveSpeed;//移動速度を設定します


        //データ同期を行う
        // Debug.Log("データはすでに同期されています。");
        enemyFSM.parameter.enemyData = enemyData;
        enemyFSM.parameter.enemyStateData = tempEnemyStateData;
        enemyFSM.parameter.animator = animator;
        enemyFSM.parameter._mainCamera = _mainCamera;
        enemyFSM.parameter.body = enemyBody;
        enemyFSM.parameter.agent = agent;
        enemyFSM.parameter.enemyPos = this.transform;
    }

    protected new void Update() {
        base.Update();
        //各管理クラスのUpdateメソッド
        characterBuffManager.OnUpdate(Time.deltaTime);
        if(enemyFSM.parameter.ableChase)
        {
            warningArea.enabled = false;
            // FaceToTarget(orientationObject);
        }else{
            warningArea.enabled = true;
        }


        /// バフのテスト
        // if(enemyData.currentHealth <75)
        // {
        //     // characterBuffManager.RemoveBuff(E_BuffKind.HpUp);
        //     // characterBuffManager.RemoveAllBuff();
        //     // characterBuffManager.AddBuff(new HpUp(this.gameObject,E_ChararcterType.enemy,2));
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
