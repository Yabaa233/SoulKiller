using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StateType
{
    Idle,   //静止
    Patrol, //パトロール
    Chase,  //追撃
    React,  //反応
    Attack, //攻撃
    Hit,    //攻撃を受ける
    Dead,   //死亡
}
[SerializeField]
public class Parameter
{
    public int health;
    public float moveSpeed;
    public float chaseSpeed;
    public float idleTime;
    public Transform[] patrolPoints;    //巡回範囲
    public Transform chaseLeftPoint;     //追撃範囲
    public Transform chaseRightPoint;     //追撃範囲
    public Transform target;
    public LayerMask targetLayer;   //攻撃範囲検出器の監視レイヤー
    public Transform attackPoint;   //攻撃ポイントの座標
    public float attackArea;    //攻撃範囲の半径
    public bool getHit; //攻撃を受ける
    // public Animator Animator;   //アニメーターコンポーネントを取得し、アニメーションを制御します。
}
///<summary>

////// 敵のFSM

///</summary>
public class FSM : MonoBehaviour
{
    public Parameter parameter;
    private IState currentState;
    private Dictionary<StateType, IState> states = new Dictionary<StateType, IState>();
    /// <summary>
    /// FSMの初期化
    /// </summary>
    void Start()
    {
        states.Add(StateType.Idle, new IdleState(this));
        states.Add(StateType.Patrol, new PatrolState(this));
        states.Add(StateType.Chase, new ChaseState(this));
        states.Add(StateType.React, new ReactState(this));
        states.Add(StateType.Attack, new AttackState(this));
        states.Add(StateType.Hit, new HitState(this));
        states.Add(StateType.Dead, new DeadState(this));

        TranstionState(StateType.Idle); //初期状態はIdleです

        //アニメーションコントローラーからパラメータを取得する
    }

    /// <summary>
    /// FSMの更新
    /// </summary>
    // Update is called once per frame
    void Update()
    {
        // もし（攻撃を受けたら）
        // {
        //     parameter.getHit = true;
        // }
        currentState.OnUpDate();
    }

    public void TranstionState(StateType state)
    {
        if (currentState != null)
        {
            currentState.OnExit();  //ステータスを切り替える前に、現在のステータスを終了してください。
        }
        currentState = states[state];
        currentState.OnEnter();
    }

    public void FlipTo(Transform target)//方向変更関数
    {
        if (target != null)
        {
            if (transform.position.x > target.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            parameter.target = other.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            parameter.target = null;
        }
    }

    private void OnDrawGizmos() //攻撃距離モニターは、攻撃範囲の観察に使用します。敵（Enemy）上にオブジェクトを作成し、判定の中心座標とする必要があります。レイヤーの指定を忘れないでください。
    {
        Gizmos.DrawSphere(parameter.attackPoint.position, parameter.attackArea);
    }
    /// <summary>
    /// FSMの終了
    /// </summary>
}
