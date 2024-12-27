using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyControl : BaseEnemyControl
{
    [Header("敌人状态机相关")]
    public EnemyFSM enemyFSM;

    //属性状态描述类文件
    [Header("敌人攻击状态SO模板")]
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

    /// <summary>
    /// 初始化敌人上的功能组件
    /// </summary>
    /// <param name="tagStr"></param>
    private void EnemyInit(string tagStr)
    {
        //组件获取
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

        //设置自身属性
        transform.tag = tagStr;

        //初始化一些相关状态
        characterBuffManager = new CharacterBuffManager();
        enemyData = new CharacterData(Instantiate(tempCharaterData));
        characterBuffManager.Init(E_ChararcterType.enemy);
        baseEnemyFSM = enemyFSM;
        enemyData.currentComboAttack = 1;//设置攻击倍率
        // agent.speed = tempEnemyStateData.moveSpeed;//设置移动速度


        //做数据同步
        // Debug.Log("数据已经同步");
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
        //各个管理类的Update方法
        characterBuffManager.OnUpdate(Time.deltaTime);
        if(enemyFSM.parameter.ableChase)
        {
            warningArea.enabled = false;
            // FaceToTarget(orientationObject);
        }else{
            warningArea.enabled = true;
        }


        /// 测试Buff
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

/// <summary>
/// 设置追踪目标
/// </summary>
/// <param name="transform"></param>
    public override void SetTarget(Transform transform)
    {
        enemyFSM.parameter.target = transform;
    }

}
