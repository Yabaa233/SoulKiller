using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

//敌人状态的注册
public enum E_EnemyStateType
{
    Idle,   //静止
    Chase,  //追击
    Attack, //攻击
    Hit,    //受到攻击
    Dead,   //死亡
    Shot,   //射击
    Find, //发现状态
    Storage,//蓄力状态
    Dizzy,//眩晕状态
    Boom,//自爆状态
    Jump,//跳跃状态
    MoveAfter,//走位状态
}


//敌人的所有参数
[Serializable]
public class EnemyParameter
{
    public CharacterData enemyData;//得到一份基础属性引用
    public Animator animator;//动画控制器
    public GameObject _mainCamera;//相机位置
    public Transform body;//身体部分
    public EzEmyStateData_SO enemyStateData;//得到一份属性状态引用
    public Transform target;
    public bool ableChase;
    public bool getHit; //受到攻击
    public NavMeshAgent agent;//导航组件
    public Transform enemyPos;//自己的位置
    public bool isDead;//是否已经死亡
    public bool isDash;//是否已经冲刺过
}

public class EnemyFSM : BaseEnemyFSM
{
    public EnemyParameter parameter;
    public Rigidbody rb;
    
    public CDClass DashCD = new CDClass();

    private void Start() {//在这里注册所有的状态机
        DashCD.maxCDTime = 3f;
        rb = GetComponent<Rigidbody>();
        GameManager.Instance.CDList.Add(DashCD);
        DashCD.flag = true;

        states.Add(E_EnemyStateType.Idle, new Enemy_IdleState(this));
        states.Add(E_EnemyStateType.Find, new Enemy_FindState(this));
        states.Add(E_EnemyStateType.Chase, new Enemy_ChaseState(this));
        states.Add(E_EnemyStateType.Storage,new Enemy_StorageState(this));
        states.Add(E_EnemyStateType.Attack, new Enemy_AttackState(this));
        states.Add(E_EnemyStateType.Dead, new Enemy_DeadState(this));
        states.Add(E_EnemyStateType.Hit, new Enemy_HitState(this));

        
        TranstionState(E_EnemyStateType.Idle); //初始化状态为Idle，设置初始状态为待机状态
    }

    private void Update() {//在这里执行当前的状态机Update
        FaceToCamera();
        currentState.OnUpDate();
    }

    /// <summary>
    /// 让敌人朝向玩家的左右
    /// </summary>
    public void RotateToTarget()
    {
        float  weight = parameter.target.transform.position.x - transform.position.x;
        float scaleRes_Move = weight > 0 ? -1 : 1;
        transform.localScale = new Vector3(scaleRes_Move,1,1);
    }

    public void FaceToCamera()
    {
        var rotation = Quaternion.LookRotation(parameter._mainCamera.transform.TransformVector(Vector3.forward),
            parameter._mainCamera.transform.TransformVector(Vector3.up));
        rotation = new Quaternion(0f, rotation.y, 0, rotation.w);
        var rotationx = Quaternion.Euler(45f,0f,0f);
        rotation *= rotationx;
        parameter.body.transform.rotation = rotation;
    }

    /// <summary>
    /// 让物体指向玩家方向
    /// </summary>
    /// <param name="other"></param>
    public void FaceToTarget()
    {
        Vector3 lookVector = parameter.target.position - transform.position;
        lookVector.y = 0;
        transform.rotation = Quaternion.LookRotation(lookVector);
    }

    private void OnTriggerEnter(Collider other) {//第一个敌人的状态机为见到了玩家就直接追击不停止
        // Debug.Log("触发碰撞器");
        if(other.tag == "Player")
        {
            if(parameter.ableChase == false)
            {
                // Debug.Log("触发状态");
                parameter.ableChase = true;
            }
            else
            {   
                // Debug.Log("掉血");
                GameManager.Instance.EnemyAttack(parameter.enemyPos.gameObject.GetComponent<BaseEnemyControl>());
            }
        }
    }

    private void OnCollisionEnter(Collision other) {
        // Debug.Log("产生碰撞");
        if(other.transform.tag == "Player")
        {
            
        }
    }

    // private void OnDrawGizmos() //在这里绘制距离相关的支持
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawSphere(transform.position,10f);
    // }
}
