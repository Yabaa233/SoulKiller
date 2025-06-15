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

//長距離敵の所有パラメータ
[System.Serializable]
public class longEnemyParameter
{
    public E_EnemyType enemyType;
    public CharacterData enemyData;
    public Animator animator;//アニメーションコントローラー
    public GameObject _mainCamera;//カメラの位置
    public Transform body;//身体の部分
    public Transform orientationObject;
    public Transform outPos;//点滅点
    public longEmyStateData_SO enemyStateData;
    public Transform target;
    public float moveSpeed;
    public bool ableAttact;
    public bool getHit;
    public NavMeshAgent agent;//ナビゲーションコンポーネント
    public Transform enemyPos;//自分の位置
    public Transform firePoint;//発火点
    public bool isDead;//すでに死亡していますか？
}

///<summary>


////// 長距離敵のFSM


///</summary>
public class long_DistanceFSM : BaseEnemyFSM
{
    public longEnemyParameter parameter;

    public CDClass BoomCD = new CDClass();
    public CDClass jumpCD = new CDClass();
    // private IState currentState;
    // private Dictionary<E_EnemyStateType, IState> states = new Dictionary<E_EnemyStateType, IState>();
    private void Start() {//ここで全ての状態機械を登録してください。
        //いくつかのデータの初期化
        BoomCD.maxCDTime = 4f;
        BoomCD.flag = true;
        GameManager.Instance.CDList.Add(BoomCD);

        jumpCD.maxCDTime = 2f;
        jumpCD.flag = true;
        GameManager.Instance.CDList.Add(jumpCD);

        //ステートマシンの初期化
        states.Add(E_EnemyStateType.Idle,new longEnemy_IdleState(this));
        states.Add(E_EnemyStateType.Chase,new longEnemy_ChaseState(this));
        states.Add(E_EnemyStateType.Shot,new longEnemy_ShootState(this));
        states.Add(E_EnemyStateType.Jump,new longEnemy_JumpState(this));
        states.Add(E_EnemyStateType.MoveAfter,new longEnemy_MoveAfterState(this));
        states.Add(E_EnemyStateType.Storage,new longEnemy_StorageState(this));
        states.Add(E_EnemyStateType.Hit,new longEnemy_GetHitState(this));
        states.Add(E_EnemyStateType.Dead,new longEnemy_DeadState(this));

        TranstionState(E_EnemyStateType.Idle); //初期状態はIdleに設定し、初期状態を待機状態に設定します。
    }

    //その他の属性
    private void Update() {//ここで現在のステートマシンの更新を実行します。
        FaceToTarget();
        RotateToTarget();
        FaceToCamera();
        currentState.OnUpDate();
    }

    // public void TranstionState(E_EnemyStateType state) //変換メソッド
    // {
    //     if (currentState != null)
    //     {
    //         currentState.OnExit();  //現在の状態を終了してから状態を切り替えます
    //     }
    //     currentState = states[state];
    //     currentState.OnEnter();
    // }

    ///<summary>


    ////// 敵をプレイヤーの左右に向けてください。


    ///</summary>
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

    ///<summary>


    ////// 親オブジェクトをプレイヤーの方向に向けます


    ///</summary>
    /// <param name="other"></param>
    public void FaceToTarget()
    {
        Vector3 lookVector = parameter.target.position - transform.position;
        lookVector.y = 0;
        parameter.orientationObject.transform.rotation = Quaternion.LookRotation(lookVector);
        transform.rotation = Quaternion.LookRotation(lookVector);
    }

    ///<summary>


    ////// 発砲方法


    ///</summary>
    public void Shot(E_EnemyType enemyType)
    {
        switch(enemyType)
        {
            case E_EnemyType.GUN:BulletShot();break;
            case E_EnemyType.BOOM:BoomShot();break;
            case E_EnemyType.STAFF:MagicballShot();break;
        }
    }

    ///プライベート属性
    private void OnTriggerEnter(Collider other) {
        // Debug.Log("トリガーが発動されました");
        if(other.tag == "Player")
        {
            parameter.ableAttact = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        
    }


    // private void OnDrawGizmos() //ここで距離関連のサポートを描画します
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawSphere(transform.position,10f);
    // }


///<summary>



////// 弾を発射する



///</summary>
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
        bullet.transform.forward = firePoint.forward;//前方向を修正します
        bullet.GetComponent<BulletTest>().SetShotter(parameter.enemyPos.gameObject);
    }

///<summary>


////// 砲弾を発射する


///</summary>
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
///<summary>

////// 発射法球

///</summary>
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
