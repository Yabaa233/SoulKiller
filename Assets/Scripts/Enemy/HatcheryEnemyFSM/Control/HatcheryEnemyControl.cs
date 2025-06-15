using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class HatcheryEnemyControl : BaseEnemyControl
{
    [Header("敵のステートマシン関連")]
    public HatcheryEnemyFSM enemyFSM;
    [Header("敵の攻撃状態SOテンプレート")]
    public HatcheryEmyStateData_SO tempEnemyStateData;
    public HatcheryEmyStateData_SO TempEnemyStateData
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
        // "すでに実行中"のデバッグログ
        base.Start();
    }
    void EnemyInit(string strTag)
    {
        //コンポーネントの取得
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        enemyFSM = transform.GetComponent<HatcheryEnemyFSM>();
        _mainCamera = GameObject.FindWithTag("MainCamera");
        enemyBody = transform.Find("body");
        baseEnemyFSM = enemyFSM;
        agent = GetComponent<NavMeshAgent>();

        //自身の属性を設定する
        enemyData.currentComboAttack = 1;//攻撃倍率を設定する
        transform.tag = strTag;
        enemyData = new CharacterData(Instantiate(tempCharaterData));
        room = transform.parent.parent.parent.GetComponent<FirstRoomTrigger>();
        //関連状態の初期化
        characterBuffManager = new CharacterBuffManager();
        enemyData = new CharacterData(Instantiate(tempCharaterData));
        characterBuffManager.Init(E_ChararcterType.enemy);

        //データの同期
        enemyFSM.parameter.enemyData = enemyData;
        enemyFSM.parameter.enemyStateData = tempEnemyStateData;
        enemyFSM.parameter.animator = animator;
        enemyFSM.parameter._mainCamera = _mainCamera;
        enemyFSM.parameter.body = enemyBody;
    }
    protected new void Update()//アップデート管理
    {
        base.Update();
        characterBuffManager.OnUpdate(Time.deltaTime);
    }
    public override void Damaged(float damage,bool isCritical = false)//ダメージマークを受けた
    {
        base.Damaged(damage,isCritical);
        enemyFSM.parameter.getHit = true;
    }

    public override void Die()
    {
        base.Die();
        enemyFSM.parameter.isDead = true;
    }
}
