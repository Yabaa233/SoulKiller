using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



[System.Serializable]
public class SwordEnemyParameter
{
    public CharacterData enemyData;
    public Animator animator;//动画控制器
    public GameObject _mainCamera;//相机位置
    public Transform body;//身体部分
    public SwordEmyStateData_SO enemyStateData;
    public Transform target;
    public Transform orientationObject;
    public float moveSpeed;
    public bool ableAttact;
    public bool getHit;
    public NavMeshAgent agent;//导航组件
    public Transform enemyPos;//自己的位置
    public bool isDead;//是否已经死亡
    public bool isDash;//是否施加过冲刺
    public bool isDizzy;//是否撞到了墙体
}
public class SwordEnemyFSM : BaseEnemyFSM
{
    public SwordEnemyParameter parameter;
    public Rigidbody rb;
    public CDClass AttackCD = new CDClass();
    // private IState currentState;
    // private Dictionary<E_EnemyStateType, IState> states = new Dictionary<E_EnemyStateType, IState>();
    private void Start()
    {
        AttackCD.maxCDTime = 1.5f;
        rb = GetComponent<Rigidbody>();
        GameManager.Instance.CDList.Add(AttackCD);
        AttackCD.flag = true;

        //得到相应的组件
        states.Add(E_EnemyStateType.Idle,new SwordEnemy_IdleState(this));
        states.Add(E_EnemyStateType.Chase,new SwordEnemy_ChaseState(this));
        states.Add(E_EnemyStateType.Storage,new SwordEnemy_StorageState(this));
        states.Add(E_EnemyStateType.Attack,new SwordEnemy_AttackState(this));
        states.Add(E_EnemyStateType.Hit,new SwordEnemy_GetHitState(this));
        states.Add(E_EnemyStateType.Dead,new SwordEnemy_DeadState(this));


        TranstionState(E_EnemyStateType.Idle); //初始化状态为Idle，设置初始状态为待机状态
    }
    private void Update() {//在这里执行当前的状态机Update
        FaceToCamera();
        currentState.OnUpDate();
    }

    private void FixedUpdate() {
        currentState.OnLateUpDade();
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
    /// 让父物体指向玩家方向
    /// </summary>
    /// <param name="other"></param>
    public void FaceToTarget()
    {
        Vector3 lookVector = parameter.target.position - transform.position;
        lookVector.y = 0;
        parameter.orientationObject.transform.rotation = Quaternion.LookRotation(lookVector);
        transform.rotation = Quaternion.LookRotation(lookVector);
    }
    private void OnTriggerEnter(Collider other) 
    {
        // Debug.Log("父物体的检测被调用");
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

    private void OnCollisionEnter(Collision other) {
        
    }
}
