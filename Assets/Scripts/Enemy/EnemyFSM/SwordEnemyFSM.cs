using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



[System.Serializable]
public class SwordEnemyParameter
{
    public CharacterData enemyData;
    public Animator animator;//アニメーションコントローラー
    public GameObject _mainCamera;//カメラの位置
    public Transform body;//体の部分
    public SwordEmyStateData_SO enemyStateData;
    public Transform target;
    public Transform orientationObject;
    public float moveSpeed;
    public bool ableAttact;
    public bool getHit;
    public NavMeshAgent agent;//ナビゲーションコンポーネント
    public Transform enemyPos;//自分の位置
    public bool isDead;//あなたはすでに死んでいますか？
    public bool isDash;//スプリントを適用したことがありますか？
    public bool isDizzy;//壁にぶつかりましたか？
}
///<summary>

////// 剣の敵のFSM

///</summary>
public class SwordEnemyFSM : BaseEnemyFSM
{
    public SwordEnemyParameter parameter;
    public Rigidbody rb;
    public CDClass AttackCD = new CDClass();
    // private IState currentState;
    // private Dictionary<E_EnemyStateType, IState> states = new Dictionary<E_EnemyStateType, IState>();
    /// <summary>
    /// FSMの初期化
    /// </summary>
    private void Start()
    {
        AttackCD.maxCDTime = 1.5f;
        rb = GetComponent<Rigidbody>();
        GameManager.Instance.CDList.Add(AttackCD);
        AttackCD.flag = true;

        //対応するコンポーネントを取得する
        states.Add(E_EnemyStateType.Idle,new SwordEnemy_IdleState(this));
        states.Add(E_EnemyStateType.Chase,new SwordEnemy_ChaseState(this));
        states.Add(E_EnemyStateType.Storage,new SwordEnemy_StorageState(this));
        states.Add(E_EnemyStateType.Attack,new SwordEnemy_AttackState(this));
        states.Add(E_EnemyStateType.Hit,new SwordEnemy_GetHitState(this));
        states.Add(E_EnemyStateType.Dead,new SwordEnemy_DeadState(this));


        TranstionState(E_EnemyStateType.Idle); //初期状態はIdleに設定し、初期状態を待機状態に設定します。
    }
    /// <summary>
    /// FSMの更新
    /// </summary>
    private void Update() {//ここで現在のステートマシンの更新を実行します。
        FaceToCamera();
        currentState.OnUpDate();
    }

    private void FixedUpdate() {
        currentState.OnLateUpDade();
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
    private void OnTriggerEnter(Collider other) 
    {
        // Debug.Log("親オブジェクトの検出が呼び出されました");
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

    private void OnCollisionEnter(Collision other) {
        
    }
    /// <summary>
    /// FSMの終了
    /// </summary>
}
