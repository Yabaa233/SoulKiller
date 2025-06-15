using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class DashBoomEnemyParameter
{
    public CharacterData enemyData;
    public Animator animator;//アニメーションコントローラー
    public GameObject _mainCamera;//カメラの位置
    public Transform body;//身体の部分
    public DBoomEmyStateData_SO enemyStateData;
    public Transform target;
    public bool ableAttact;
    public bool getHit;
    public NavMeshAgent agent;//ナビゲーションコンポーネント
    public Transform enemyPos;//自分の位置
    public float moveSpeed;
    public bool isDead;//死んでいるかどうか
    public bool isDash;//スティックを押したかどうか
    public bool isDizzy;//壁に衝突したかどうか
    public bool isBoom;//自爆が開始されたかどうか
    public bool playerIsStay;//プレイヤーが爆発範囲内にいるかどうか
}
public class DashBoomEnemyFSM : BaseEnemyFSM
{
    public DashBoomEnemyParameter parameter;
    // private IState currentState;
    // private Dictionary<E_EnemyStateType, IState> states = new Dictionary<E_EnemyStateType, IState>();
    //必要なコンポーネントを取得する
    public Rigidbody rb;

    //補助計算の属性
    private Vector3 lastDir;
    
    private void Start()
    {
        //必要なコンポーネントを取得する
        rb = GetComponent<Rigidbody>();

        states.Add(E_EnemyStateType.Idle,new DBoomEnemy_IdleState(this));
        states.Add(E_EnemyStateType.Find,new DBoomEnemy_FindState(this));
        states.Add(E_EnemyStateType.Chase,new DBoomEnemy_ChaseState(this));
        states.Add(E_EnemyStateType.Dizzy,new DBoomEnemy_DizzyState(this));
        states.Add(E_EnemyStateType.Hit,new DBoomEnemy_GetHitState(this));
        states.Add(E_EnemyStateType.Dead,new DBoomEnemy_DeadState(this));
        states.Add(E_EnemyStateType.Boom,new DBoomEnemy_BoomState(this));


        TranstionState(E_EnemyStateType.Idle); //初期状態をIdleに設定し、初期状態を待機状態にします。
    }

    private void Update() {//ここで現在の状態機のアップデートを実行します
        currentState.OnUpDate();
        FaceToCamera();
    }
    private void FixedUpdate() {
        lastDir = rb.velocity;
        // Debug.Log("BoomDashの速度は：" + lastDir.magnitude);
    }
    // public void TranstionState(E_EnemyStateType state)//状態変換メソッド
    // {
    //     if (currentState != null)
    //     {
    //         currentState.OnExit();  //現在の状態を終了させる前に状態を切り替えます
    //     }
    //     currentState = states[state];
    //     currentState.OnEnter();
    // }

    ///<summary>


    ////// 敵をプレイヤーの左右に向ける


    ///</summary>
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
    /// オブジェクトをプレイヤーの方向に向けます
    /// </summary>
    /// <param name="other"></param>
    public void FaceToTarget()
    {
        Vector3 lookVector = parameter.target.position - transform.position;
        lookVector.y = 0;
        transform.rotation = Quaternion.LookRotation(lookVector);
    }

    ///<summary>


    ////// 自爆クラスの敵は特別な処理が必要です。


    ///</summary>
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
            //衝突時の速度を反転させる
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
