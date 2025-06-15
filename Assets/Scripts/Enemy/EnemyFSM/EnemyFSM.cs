using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

//敵の状態の登録
public enum E_EnemyStateType
{
    Idle,   //静止
    Chase,  //追撃
    Attack, //攻撃
    Hit,    //攻撃を受ける
    Dead,   //死亡
    Shot,   //射撃
    Find, //状態を発見する
    Storage,//蓄積状態
    Dizzy,//めまい状態
    Boom,//自爆状態
    Jump,//ジャンプ状態
    MoveAfter,//位置決め状態
}


//敵のすべてのパラメータ
[Serializable]
public class EnemyParameter
{
    public CharacterData enemyData;//基本属性の参照を取得する
    public Animator animator;//アニメーションコントローラー
    public GameObject _mainCamera;//カメラの位置
    public Transform body;//体の部分
    public EzEmyStateData_SO enemyStateData;//属性状態の参照を取得する
    public Transform target;
    public bool ableChase;
    public bool getHit; //攻撃を受ける
    public NavMeshAgent agent;//ナビゲーションコンポーネント
    public Transform enemyPos;//自分の位置
    public bool isDead;//あなたはすでに死んでいますか？
    public bool isDash;//あなたはすでにスプリントを行ったことがありますか？
}

public class EnemyFSM : BaseEnemyFSM
{
    public EnemyParameter parameter;
    public Rigidbody rb;
    
    public CDClass DashCD = new CDClass();

    private void Start() {//ここで全ての状態機械を登録してください。
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

        
        TranstionState(E_EnemyStateType.Idle); //初期状態はIdleに設定し、初期状態を待機状態に設定します。
    }

    private void Update() {//ここで現在のステートマシンの更新を実行します。
        FaceToCamera();
        currentState.OnUpDate();
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
    /// オブジェクトをプレイヤーの方向に向けます
    /// </summary>
    /// <param name="other"></param>
    public void FaceToTarget()
    {
        Vector3 lookVector = parameter.target.position - transform.position;
        lookVector.y = 0;
        transform.rotation = Quaternion.LookRotation(lookVector);
    }

    private void OnTriggerEnter(Collider other) {//最初の敵の状態マシンは、プレイヤーを見つけるとすぐに追いかけて止まりません。
        // Debug.Log("コライダーがトリガーされました");
        if(other.tag == "Player")
        {
            if(parameter.ableChase == false)
            {
                // Debug.Log("トリガー状態");
                parameter.ableChase = true;
            }
            else
            {   
                // Debug.Log("血を失う");
                GameManager.Instance.EnemyAttack(parameter.enemyPos.gameObject.GetComponent<BaseEnemyControl>());
            }
        }
    }

    private void OnCollisionEnter(Collision other) {
        // Debug.Log("衝突が発生しました");
        if(other.transform.tag == "Player")
        {
            
        }
    }

    // private void OnDrawGizmos() //ここでは距離関連のサポートを描画します
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawSphere(transform.position,10f);
    // }
}
