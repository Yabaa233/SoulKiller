using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[System.Serializable]
public class DashEnemyParameter
{
    public CharacterData enemyData;
    public Animator animator;//动画控制器
    public GameObject _mainCamera;//相机位置
    public Transform body;//身体部分
    public Transform orientationObject;
    public DashEmyStateData_SO enemyStateData;
    public Transform target;
    public bool ableAttact;
    public bool getHit;
    public float moveSpeed;
    public NavMeshAgent agent;//导航组件
    public Transform enemyPos;//自己的位置
    public bool isDead;//是否已经死亡
    public bool isDash;//是否施加过冲刺
    public bool isDizzy;//是否撞到了墙体
}
public class DashEnemyFSM : BaseEnemyFSM
{
    public DashEnemyParameter parameter;
    public CDClass DashCD = new CDClass();
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
        DashCD.maxCDTime = 1.5f;
        DashCD.flag = true;
        GameManager.Instance.CDList.Add(DashCD);

        states.Add(E_EnemyStateType.Idle,new DashEnemy_IdleState(this));
        states.Add(E_EnemyStateType.Find,new DashEnemy_FindState(this));
        states.Add(E_EnemyStateType.Chase,new DashEnemy_ChaseState(this));
        states.Add(E_EnemyStateType.Dizzy,new DashEnemy_DizzyState(this));
        states.Add(E_EnemyStateType.Hit,new DashEnemy_GetHitState(this));
        states.Add(E_EnemyStateType.Dead,new DashEnemy_DeadState(this));


        TranstionState(E_EnemyStateType.Idle); //初始化状态为Idle，设置初始状态为待机状态
    }
    private void Update() {//在这里执行当前的状态机Update
        currentState.OnUpDate();
        FaceToCamera();
    }

    private void FixedUpdate() {
        lastDir = rb.velocity;
    }

    private void LateUpdate() {
        FaceToCamera();
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
        parameter.orientationObject.transform.rotation = Quaternion.LookRotation(lookVector);
        parameter.enemyPos.transform.Find("weapon").rotation = Quaternion.LookRotation(lookVector);
        // transform.rotation = Quaternion.LookRotation(lookVector);
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
                GameManager.Instance.EnemyAttack(parameter.enemyPos.gameObject.GetComponent<BaseEnemyControl>());
                // Debug.Log("攻击了玩家");
            }
        }
    }

    private void OnTriggerStay(Collider other) {
        if(other.tag == "Player")
        {
            parameter.ableAttact = true;
        }
    }

    private void OnCollisionEnter(Collision other) {
        if(other.transform.tag == "Wall")
        {
            //碰撞时速度进行反弹
            Vector3 reflexAngle = Vector3.Reflect(lastDir,other.contacts[0].normal);
            rb.velocity = reflexAngle.normalized * lastDir.magnitude;
            parameter.isDizzy = true;
            //FmodManager.Instance.PlaySoundOnce(parameter.enemyStateData.dashedEffect);
            FMODUnity.RuntimeManager.PlayOneShot(parameter.enemyStateData.dashedEffect);
        }
    }
}
