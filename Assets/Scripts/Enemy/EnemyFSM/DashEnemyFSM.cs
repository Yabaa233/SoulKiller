using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[System.Serializable]
public class DashEnemyParameter
{
    public CharacterData enemyData;
    public Animator animator;//アニメーションコントローラー
    public GameObject _mainCamera;//カメラの位置
    public Transform body;//身体の部分
    public Transform orientationObject;
    public DashEmyStateData_SO enemyStateData;
    public Transform target;
    public bool ableAttact;
    public bool getHit;
    public float moveSpeed;
    public NavMeshAgent agent;//ナビゲーションコンポーネント
    public Transform enemyPos;//自分の位置
    public bool isDead;//すでに死亡していますか？
    public bool isDash;//スプリントを適用したことがありますか？
    public bool isDizzy;//壁にぶつかりましたか？
}
/// <summary>
/// ダッシュ敵のFSM
/// </summary>
public class DashEnemyFSM : BaseEnemyFSM
{
    public DashEnemyParameter parameter;
    public CDClass DashCD = new CDClass();
    // private IState currentState;
    // private Dictionary<E_EnemyStateType, IState> states = new Dictionary<E_EnemyStateType, IState>();

    //取得が必要なコンポーネント
    public Rigidbody rb;

    //補助計算の属性
    private Vector3 lastDir;
    /// <summary>
    /// FSMの初期化
    /// </summary>
    private void Start()
    {
        //対応するコンポーネントを取得する
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


        TranstionState(E_EnemyStateType.Idle); //初期状態はIdleに設定し、初期状態を待機状態に設定します。
    }
    /// <summary>
    /// FSMの更新
    /// </summary>
    private void Update() {//ここで現在のステートマシンの更新を実行します。
        currentState.OnUpDate();
        FaceToCamera();
    }

    private void FixedUpdate() {
        lastDir = rb.velocity;
    }

    private void LateUpdate() {
        FaceToCamera();
    }

    ///<summary>


    ////// 敵をプレイヤーの左右に向けてください。


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
    /// オブジェクトをプレイヤーの方向に向ける
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
                // Debug.Log("プレイヤーが攻撃されました");
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
            //衝突時の速度で反発する
            Vector3 reflexAngle = Vector3.Reflect(lastDir,other.contacts[0].normal);
            rb.velocity = reflexAngle.normalized * lastDir.magnitude;
            parameter.isDizzy = true;
            //FmodManager.Instance.PlaySoundOnce(parameter.enemyStateData.dashedEffect);
            FMODUnity.RuntimeManager.PlayOneShot(parameter.enemyStateData.dashedEffect);
        }
    }
}
