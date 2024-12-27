using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum E_EnemyType
{
    GUN,
    BOOM,
    STAFF,
}

//远程类敌人的所有参数
[System.Serializable]
public class longEnemyParameter
{
    public E_EnemyType enemyType;
    public CharacterData enemyData;
    public Animator animator;//动画控制器
    public GameObject _mainCamera;//相机位置
    public Transform body;//身体部分
    public Transform orientationObject;
    public Transform outPos;//闪烁点
    public longEmyStateData_SO enemyStateData;
    public Transform target;
    public float moveSpeed;
    public bool ableAttact;
    public bool getHit;
    public NavMeshAgent agent;//导航组件
    public Transform enemyPos;//自己的位置
    public Transform firePoint;//开火点
    public bool isDead;//是否已经死亡
}

/// <summary>
/// 将所有远程攻击角色的状态机统合到一块
/// </summary>
public class long_DistanceFSM : BaseEnemyFSM
{
    public longEnemyParameter parameter;

    public CDClass BoomCD = new CDClass();
    public CDClass jumpCD = new CDClass();
    // private IState currentState;
    // private Dictionary<E_EnemyStateType, IState> states = new Dictionary<E_EnemyStateType, IState>();
    private void Start() {//在这里注册所有的状态机
        //一些数据初始化
        BoomCD.maxCDTime = 4f;
        BoomCD.flag = true;
        GameManager.Instance.CDList.Add(BoomCD);

        jumpCD.maxCDTime = 2f;
        jumpCD.flag = true;
        GameManager.Instance.CDList.Add(jumpCD);

        //初始化状态机
        states.Add(E_EnemyStateType.Idle,new longEnemy_IdleState(this));
        states.Add(E_EnemyStateType.Chase,new longEnemy_ChaseState(this));
        states.Add(E_EnemyStateType.Shot,new longEnemy_ShootState(this));
        states.Add(E_EnemyStateType.Jump,new longEnemy_JumpState(this));
        states.Add(E_EnemyStateType.MoveAfter,new longEnemy_MoveAfterState(this));
        states.Add(E_EnemyStateType.Storage,new longEnemy_StorageState(this));
        states.Add(E_EnemyStateType.Hit,new longEnemy_GetHitState(this));
        states.Add(E_EnemyStateType.Dead,new longEnemy_DeadState(this));

        TranstionState(E_EnemyStateType.Idle); //初始化状态为Idle，设置初始状态为待机状态
    }

    //其它属性
    private void Update() {//在这里执行当前的状态机Update
        FaceToTarget();
        RotateToTarget();
        FaceToCamera();
        currentState.OnUpDate();
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
        Vector3 lastScale = transform.localScale;
        float  weight = parameter.target.transform.position.x - transform.position.x;
        float scaleRes_Move = weight > 0 ? -1 : 1;
        transform.localScale = new Vector3(scaleRes_Move,1f,1f);
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

    /// <summary>
    /// 开枪方法
    /// </summary>
    public void Shot(E_EnemyType enemyType)
    {
        switch(enemyType)
        {
            case E_EnemyType.GUN:BulletShot();break;
            case E_EnemyType.BOOM:BoomShot();break;
            case E_EnemyType.STAFF:MagicballShot();break;
        }
    }

    ///私有属性
    private void OnTriggerEnter(Collider other) {
        // Debug.Log("产生了触发");
        if(other.tag == "Player")
        {
            parameter.ableAttact = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        
    }


    // private void OnDrawGizmos() //在这里绘制距离相关的支持
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawSphere(transform.position,10f);
    // }


/// <summary>
/// 发射子弹
/// </summary>
    private void BulletShot()
    {
        for(int i = 0;i<3;i++)
        {
            Invoke("GunShot",i*0.1f);
        }
        // Transform firePoint = parameter.firePoint;
        // // GameObject bullet = Instantiate(ItemPrefab,firePoint.position,firePoint.rotation);

        // GameObject bullet = ObjectPool.Instance.GetObject("Bullet");
        // bullet.SetActive(true);
        // bullet.transform.position = firePoint.position;
        // bullet.transform.rotation = parameter.enemyPos.transform.rotation;
        // bullet.GetComponent<BulletTest>().SetShotter(parameter.enemyPos.gameObject);
        
        // Destroy(bullet,10);
    }

    public void GunShot()
    {
        Transform firePoint = parameter.firePoint;
        // GameObject bullet = Instantiate(ItemPrefab,firePoint.position,firePoint.rotation);

        GameObject bullet = ObjectPool.Instance.GetObject("Bullet",EffectManager.Instance.transform,true,true);
        bullet.SetActive(true);
        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = parameter.enemyPos.transform.rotation;
        bullet.transform.forward = firePoint.forward;//将前方向矫正
        bullet.GetComponent<BulletTest>().SetShotter(parameter.enemyPos.gameObject);
    }

/// <summary>
/// 发射炮弹
/// </summary>
    private void BoomShot()
    {
        Transform firePoint = parameter.firePoint;
        GameObject boom = ObjectPool.Instance.GetObject("Boom", EffectManager.Instance.transform,true);
        BoomTest boomTest = boom.GetComponent<BoomTest>();
        boomTest.pointA = firePoint;
        boomTest.pointB = parameter.target;
        boom.transform.position = firePoint.position;
        boom.transform.rotation = Quaternion.identity;
        boomTest.SetShotter(parameter.enemyPos.gameObject);
        boom.SetActive(true);
        // Destroy(boom,10);
    }
/// <summary>
/// 发射法球
/// </summary>
    private void  MagicballShot()
    {
        Transform firePoint = parameter.firePoint;
        GameObject magicBall = ObjectPool.Instance.GetObject("Magic",EffectManager.Instance.transform,true,true);
        MagicTest magicTest = magicBall.GetComponent<MagicTest>();
        magicBall.transform.position = firePoint.position;
        magicBall.transform.rotation = Quaternion.identity;
        magicTest.SetShotter(parameter.enemyPos.gameObject);
        // BoomTest boomTest = ItemPrefab.GetComponent<BoomTest>();
        // boomTest.pointA = firePoint;
        // boomTest.pointB = parameter.target;
        // GameObject boom = Instantiate(ItemPrefab,firePoint.position,Quaternion.identity);
        // Destroy(boom,10);
        // Destroy(bullet,10);
    }

}
