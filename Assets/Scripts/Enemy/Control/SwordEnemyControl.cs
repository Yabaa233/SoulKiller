using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class SwordEnemyControl : BaseEnemyControl
{
    // Start is called before the first frame update
    //新增变量
    [Header("敌人状态机相关")]
    public SwordEnemyFSM enemyFSM;

    [Header("敌人攻击状态SO模板")]
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
        // Debug.Log("已经运行");
        base.Start();
        SetTarget(GameManager.Instance.currentPlayer.transform);
    }

    private void EnemyInit(string tagStr)
    {
        //组件获取
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        enemyFSM = transform.GetComponent<SwordEnemyFSM>();
        agent = GetComponent<NavMeshAgent>();
        _mainCamera = GameObject.FindWithTag("MainCamera");
        enemyBody = transform.Find("body");
        warningArea = GetComponent<SphereCollider>();
        attackArea = transform.Find("weapon");
        orientationObject = transform.Find("OrientationObject");


        //设置自身属性
        transform.tag = tagStr;

        //初始化一些相关状态
        characterBuffManager = new CharacterBuffManager();
        enemyData = new CharacterData(Instantiate(tempCharaterData));
        characterBuffManager.Init(E_ChararcterType.enemy);
        baseEnemyFSM = enemyFSM;
        enemyData.currentComboAttack = 1;//设置攻击倍率
        moveSpeed = tempEnemyStateData.moveSpeed;
        agent.speed = tempEnemyStateData.moveSpeed;//设置移动速度
        // attackArea.gameObject.SetActive(false);//最开始禁用该组件


        //做数据同步
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
        //各个管理类的Update方法
        characterBuffManager.OnUpdate(Time.deltaTime);
        if(enemyFSM.parameter.ableAttact)
        {
            warningArea.enabled = false;
            // FaceToTarget(orientationObject);
        }else{
            warningArea.enabled = true;
        }
        // transform.LookAt(enemyFSM.parameter.target);
        // if(enemyFSM.parameter.isDead)//挪到死亡状态中执行
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
/// <summary>
/// 设置追踪目标
/// </summary>
/// <param name="transform"></param>
    public override void SetTarget(Transform transform)
    {
        enemyFSM.parameter.target = transform;
    }

}
