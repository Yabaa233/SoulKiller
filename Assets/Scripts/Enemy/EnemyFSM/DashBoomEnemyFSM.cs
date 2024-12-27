using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class DashBoomEnemyParameter
{
    public CharacterData enemyData;
    public Animator animator;//动画控制器
    public GameObject _mainCamera;//相机位置
    public Transform body;//身体部分
    public DBoomEmyStateData_SO enemyStateData;
    public Transform target;
    public bool ableAttact;
    public bool getHit;
    public NavMeshAgent agent;//导航组件
    public Transform enemyPos;//自己的位置
    public float moveSpeed;
    public bool isDead;//是否已经死亡
    public bool isDash;//是否施加过冲刺
    public bool isDizzy;//是否撞到了墙体
    public bool isBoom;//是否开启自爆
    public bool playerIsStay;//玩家是否还在爆炸区域内
}
public class DashBoomEnemyFSM : BaseEnemyFSM
{
    public DashBoomEnemyParameter parameter;
    // private IState currentState;
    // private Dictionary<E_EnemyStateType, IState> states = new Dictionary<E_EnemyStateType, IState>();
    //需要获取的组件
    public Rigidbody rb;

    //辅助计算的属性
    private Vector3 lastDir;
    
    private void Start()
    {
        //得到相应的组件
        rb = GetComponent<Rigidbody>();

        states.Add(E_EnemyStateType.Idle,new DBoomEnemy_IdleState(this));
        states.Add(E_EnemyStateType.Find,new DBoomEnemy_FindState(this));
        states.Add(E_EnemyStateType.Chase,new DBoomEnemy_ChaseState(this));
        states.Add(E_EnemyStateType.Dizzy,new DBoomEnemy_DizzyState(this));
        states.Add(E_EnemyStateType.Hit,new DBoomEnemy_GetHitState(this));
        states.Add(E_EnemyStateType.Dead,new DBoomEnemy_DeadState(this));
        states.Add(E_EnemyStateType.Boom,new DBoomEnemy_BoomState(this));


        TranstionState(E_EnemyStateType.Idle); //初始化状态为Idle，设置初始状态为待机状态
    }

    private void Update() {//在这里执行当前的状态机Update
        currentState.OnUpDate();
        FaceToCamera();
    }
    private void FixedUpdate() {
        lastDir = rb.velocity;
        // Debug.Log("BoomDash的速度为:"+ lastDir.magnitude);
    }
    // public void TranstionState(E_EnemyStateType state)//转换方法
    // {
    //     if (currentState != null)
    //     {
    //         currentState.OnExit();  //切换状态先退出当前状态
    //     }
    //     currentState = states[state];
    //     currentState.OnEnter();
    // }

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

    /// <summary>
    /// 自爆类敌人需要特殊处理
    /// </summary>
    public void RecycleStatePanel()
    {
        BaseEnemyControl baseEnemyControl = gameObject.GetComponent<BaseEnemyControl>();
        GameObject statePanel = baseEnemyControl.statePanel;
        if(statePanel!=null)
        {
            ObjectPool.Instance.RecycleObj("EnemyState", statePanel);
        }
        baseEnemyControl.statePanel = null;
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.tag == "Player")
        {
            if(parameter.ableAttact == false)
            {
                parameter.ableAttact = true;
            }
            else
            {
                parameter.isBoom = true;
            }
        }
    }

    private void OnTriggerStay(Collider other) {
        if(other.tag == "Player")
        {
            if(parameter.ableAttact == false)
            {
                parameter.ableAttact = true;
            }
            else
            {
                parameter.playerIsStay = true;
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        parameter.playerIsStay = false;
    }

    private void OnCollisionEnter(Collision other) {
        if(other.transform.tag == "Wall")
        {
            //碰撞时速度进行反弹
            Vector3 reflexAngle = Vector3.Reflect(lastDir,other.contacts[0].normal);
            rb.velocity = reflexAngle.normalized * lastDir.magnitude;
            parameter.isDizzy = true;
        }
        // if(other.transform.tag == "Player")
        // {
        //     parameter.isBoom = true;
        // }
    }

}
