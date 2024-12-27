using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class BoomEnemyControl : BaseEnemyControl
{
    [Header("敌人状态机相关")]
    public long_DistanceFSM enemyFSM;
    public Transform firePoint;
    public Transform outPos;

    [Header("敌人攻击状态SO模板")]
    public BoomEmyStateData_SO tempEnemyStateData; 
    public BoomEmyStateData_SO TempEnemyStateData 
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
        //组件获取
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        enemyFSM = transform.GetComponent<long_DistanceFSM>();
        agent = GetComponent<NavMeshAgent>();
        _mainCamera = GameObject.FindWithTag("MainCamera");
        enemyBody = transform.Find("body");
        warningArea = GetComponent<SphereCollider>();
        attackArea = transform.Find("weapon");
        orientationObject = transform.Find("OrientationObject");
        outPos = transform.Find("outPos");
        firePoint = transform.Find("firepoint");

        //转换一次
        baseEnemyFSM  = enemyFSM;

        //设置自身属性
        transform.tag = tagStr;

        //初始化一些相关状态
        characterBuffManager = new CharacterBuffManager();
        enemyData = new CharacterData(Instantiate(tempCharaterData));
        characterBuffManager.Init(E_ChararcterType.enemy);
        baseEnemyFSM  = enemyFSM;
        enemyData.currentComboAttack = 1;//设置攻击倍率
        moveSpeed = tempEnemyStateData.moveSpeed;//初始化移动速度
        agent.speed = tempEnemyStateData.moveSpeed;//设置移动速度


        //做数据同步
        enemyFSM.parameter.enemyData = enemyData;
        enemyFSM.parameter.enemyStateData = tempEnemyStateData;
        enemyFSM.parameter.enemyType = E_EnemyType.BOOM;
        enemyFSM.parameter.animator = animator;
        enemyFSM.parameter._mainCamera = _mainCamera;
        enemyFSM.parameter.body = enemyBody;
        enemyFSM.parameter.agent = agent;
        enemyFSM.parameter.firePoint = firePoint;
        enemyFSM.parameter.enemyPos = this.transform;
        enemyFSM.parameter.moveSpeed = moveSpeed;
        enemyFSM.parameter.orientationObject = orientationObject;
        enemyFSM.parameter.outPos = outPos;

    }

    protected new void Update() {
        base.Update();
        //各个管理类的Update方法
        characterBuffManager.OnUpdate(Time.deltaTime);

        // characterBuffManager.AddBuff(new HpUp(gameObject,characterBuffManager.type));//加血方法
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

/// <summary>
/// 设置追踪目标
/// </summary>
/// <param name="transform"></param>
    public override void SetTarget(Transform transform)
    {
        enemyFSM.parameter.target = transform;
    }
}
