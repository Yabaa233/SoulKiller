using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StateType
{
    Idle,   //静止
    Patrol, //巡逻
    Chase,  //追击
    React,  //反应
    Attack, //攻击
    Hit,    //受到攻击
    Dead,   //死亡
}
[SerializeField]
public class Parameter
{
    public int health;
    public float moveSpeed;
    public float chaseSpeed;
    public float idleTime;
    public Transform[] patrolPoints;    //巡逻范围
    public Transform chaseLeftPoint;     //追击范围
    public Transform chaseRightPoint;     //追击范围
    public Transform target;
    public LayerMask targetLayer;   //攻击范围检测器的监测图层
    public Transform attackPoint;   //攻击点坐标
    public float attackArea;    //攻击范围半径
    public bool getHit; //受到攻击
    // public Animator Animator;   //获取动画器组件控制动画
}
public class FSM : MonoBehaviour
{
    public Parameter parameter;
    private IState currentState;
    private Dictionary<StateType, IState> states = new Dictionary<StateType, IState>();
    void Start()
    {
        states.Add(StateType.Idle, new IdleState(this));
        states.Add(StateType.Patrol, new PatrolState(this));
        states.Add(StateType.Chase, new ChaseState(this));
        states.Add(StateType.React, new ReactState(this));
        states.Add(StateType.Attack, new AttackState(this));
        states.Add(StateType.Hit, new HitState(this));
        states.Add(StateType.Dead, new DeadState(this));

        TranstionState(StateType.Idle); //初始化状态为Idle

        //获取动画控制器到parameter
    }

    // Update is called once per frame
    void Update()
    {
        // if(受到攻击)
        // {
        //     parameter.getHit = true;
        // }
        currentState.OnUpDate();
    }

    public void TranstionState(StateType state)
    {
        if (currentState != null)
        {
            currentState.OnExit();  //切换状态先退出当前状态
        }
        currentState = states[state];
        currentState.OnEnter();
    }

    public void FlipTo(Transform target)//转向函数
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

    private void OnDrawGizmos() //攻击距离监测器，用于观察攻击范围，需要在Enemy上创建一个物体作为判定的圆心坐标，记得指定图层
    {
        Gizmos.DrawSphere(parameter.attackPoint.position, parameter.attackArea);
    }
}
