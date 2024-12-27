using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class States : IState
{
    public void OnEnter()
    {
        throw new System.NotImplementedException();
    }

    public void OnExit()
    {
        throw new System.NotImplementedException();
    }

    public void OnLateUpDade()
    {
        throw new System.NotImplementedException();
    }

    public void OnUpDate()
    {
        throw new System.NotImplementedException();
    }
}

public class IdleState : IState
{
    private FSM manager;    //状态机
    private Parameter parameter;    //设置的属性
    private float idleTimer;    //原地等待时间
    public IdleState(FSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }
    public void OnEnter()
    {
        //播放动画
    }

    public void OnExit()
    {
        idleTimer = 0;
    }

    public void OnLateUpDade()
    {
        //一帧结束后要做的事情
    }

    public void OnUpDate()
    {
        idleTimer += Time.deltaTime;
        if (parameter.getHit)
        {
            manager.TranstionState(StateType.Hit);  //受到攻击，切换到受击状态
        }

        //判断是否发现玩家
        if (parameter.target != null &&
        parameter.target.position.x >= parameter.chaseLeftPoint.position.x &&
        parameter.target.position.x <= parameter.chaseRightPoint.position.x)
        {
            manager.TranstionState(StateType.React);    //发现玩家，切换到反应状态
        }

        if (idleTimer >= parameter.idleTime) //到达设定时间，切换到巡逻状态
        {
            manager.TranstionState(StateType.Patrol);
        }
    }
}
public class PatrolState : IState
{
    private FSM manager;    //状态机
    private Parameter parameter;    //设置的属性
    private int patrolPoint;    //当前巡逻点
    public PatrolState(FSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }
    public void OnEnter()
    {
        //播放相应动画
    }

    public void OnExit()
    {
        patrolPoint++;
        if (patrolPoint >= parameter.patrolPoints.Length)
        {
            patrolPoint = 0;
        }
    }

    public void OnLateUpDade()
    {
    }

    public void OnUpDate()
    {
        Transform target = parameter.patrolPoints[patrolPoint];
        manager.FlipTo(target);

        if (parameter.getHit)
        {
            manager.TranstionState(StateType.Hit);  //受到攻击，切换到受击状态
        }

        //判断是否发现玩家
        if (parameter.target != null &&
        parameter.target.position.x >= parameter.chaseLeftPoint.position.x &&
        parameter.target.position.x <= parameter.chaseRightPoint.position.x)
        {
            manager.TranstionState(StateType.React);    //发现玩家，切换到反应状态
        }

        //朝目标点移动
        manager.transform.position = Vector2.MoveTowards(
            manager.transform.position, target.position,
            parameter.moveSpeed * Time.deltaTime);

        if (Vector2.Distance(manager.transform.position, target.position) < .1f)
        {
            manager.TranstionState(StateType.Idle);
        }
    }
}
public class ChaseState : IState
{
    private FSM manager;    //状态机
    private Parameter parameter;    //设置的属性
    public ChaseState(FSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }
    public void OnEnter()
    {
        //播放动画
    }

    public void OnExit()
    {

    }

    public void OnLateUpDade()
    {
    }

    public void OnUpDate()
    {
        manager.FlipTo(parameter.target);

        if (parameter.getHit)
        {
            manager.TranstionState(StateType.Hit);  //受到攻击，切换到受击状态
        }

        if (parameter.target)
        {
            manager.transform.position = Vector2.MoveTowards(
                manager.transform.position, parameter.target.position,
                parameter.chaseSpeed * Time.deltaTime);
        }
        if (parameter.target == null ||
        parameter.target.position.x < parameter.chaseLeftPoint.position.x ||
        parameter.target.position.x > parameter.chaseRightPoint.position.x)    //如果目标丢失或者超出最大追踪范围则返回巡逻
        {
            manager.TranstionState(StateType.Idle);
        }
        if (Physics2D.OverlapCircle(parameter.attackPoint.position, parameter.attackArea, parameter.targetLayer))   //检测是否进入攻击范围
        {
            manager.TranstionState(StateType.Attack);
        }
    }
}

public class ReactState : IState
{
    private FSM manager;    //状态机
    private Parameter parameter;    //设置的属性
    //获取动画状态
    public ReactState(FSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }
    public void OnEnter()
    {
        //进入状态时播放反应动画
    }

    public void OnExit()
    {

    }

    public void OnLateUpDade()
    {
    }

    public void OnUpDate()
    {
        //获取反应动画播放状态

        if (parameter.getHit)
        {
            manager.TranstionState(StateType.Hit);  //受到攻击，切换到受击状态
        }

        // if(反应动画快播放完了)
        // {
        //     if(parameter.target != null)
        //     {
        //         manager.TranstionState(StateType.Chase);    //如果目标未丢失则开始追击
        //     }
        //     else
        //     {
        //         manager.TranstionState(StateType.Idle); //如果目标丢失则静止
        //     }
        // }
        if (parameter.target != null)
        {
            manager.FlipTo(parameter.target);   //如果还有目标就朝向目标
        }
    }
}

public class AttackState : IState
{
    private FSM manager;    //状态机
    private Parameter parameter;    //设置的属性
    //获取动画状态
    public AttackState(FSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }
    public void OnEnter()
    {
        //播放动画
    }

    public void OnExit()
    {

    }

    public void OnUpDate()
    {
        //获取攻击动画播放状态

        if (parameter.getHit)
        {
            manager.TranstionState(StateType.Hit);  //受到攻击，切换到受击状态
        }

        // if(动画快播放完成了)
        // {
        //     manager.TranstionState(StateType.Chase);
        // }
    }
    public void OnLateUpDade()
    {
    }
}

public class HitState : IState
{
    private FSM manager;    //状态机
    private Parameter parameter;    //设置的属性
    //获取动画播放进度
    public HitState(FSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }
    public void OnEnter()
    {
        //播放受伤动画
    }

    public void OnExit()
    {
        parameter.getHit = false;
    }

    public void OnUpDate()
    {
        //获取受伤动画播放状态
        if (parameter.health <= 0)
        {
            manager.TranstionState(StateType.Dead);
        }
        else
        {
            parameter.target = GameObject.FindWithTag("Player").transform;  //TODO:根据攻击发出的对象锁定将要追击的对象
            // if(受伤动画快要播放完)
            // {
            //     manager.TranstionState(StateType.Chase);    //进入追击状态
            // }
        }
    }
    public void OnLateUpDade()
    {

    }
}

public class DeadState : IState
{
    private FSM manager;    //状态机
    private Parameter parameter;    //设置的属性
    public DeadState(FSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }
    public void OnEnter()
    {
        //播放死亡动画
    }

    public void OnExit()
    {

    }

    public void OnUpDate()
    {
        //TODO:在一段时间后销毁敌人
    }
    public void OnLateUpDade()
    {
    }
}
