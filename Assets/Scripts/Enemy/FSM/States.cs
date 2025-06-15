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
    private FSM manager;    //ステートマシン
    private Parameter parameter;    //設定された属性
    private float idleTimer;    //待機時間
    public IdleState(FSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }
    public void OnEnter()
    {
        //アニメーションの再生
    }

    public void OnExit()
    {
        idleTimer = 0;
    }

    public void OnLateUpDade()
    {
        //フレーム終了後の処理
    }

    public void OnUpDate()
    {
        idleTimer += Time.deltaTime;
        if (parameter.getHit)
        {
            manager.TranstionState(StateType.Hit);  //攻撃を受けたら、ダメージ受け状態に切り替えてください。
        }

        //プレイヤーが見つかったかどうかの判断
        if (parameter.target != null &&
        parameter.target.position.x >= parameter.chaseLeftPoint.position.x &&
        parameter.target.position.x <= parameter.chaseRightPoint.position.x)
        {
            manager.TranstionState(StateType.React);    //プレイヤーを見つけたら、反応状態に切り替えてください。
        }

        if (idleTimer >= parameter.idleTime) //設定時間に達したら、パトロール状態に切り替えます。
        {
            manager.TranstionState(StateType.Patrol);
        }
    }
}
public class PatrolState : IState
{
    private FSM manager;    //ステートマシン
    private Parameter parameter;    //設定された属性
    private int patrolPoint;    //現在の巡回ポイント
    public PatrolState(FSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }
    public void OnEnter()
    {
        //対応するアニメーションを再生する
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
            manager.TranstionState(StateType.Hit);  //攻撃を受けたら、ダメージ受け状態に切り替えてください。
        }

        //プレイヤーが見つかったかどうかの判断
        if (parameter.target != null &&
        parameter.target.position.x >= parameter.chaseLeftPoint.position.x &&
        parameter.target.position.x <= parameter.chaseRightPoint.position.x)
        {
            manager.TranstionState(StateType.React);    //プレイヤーを見つけたら、反応状態に切り替えてください。
        }

        //目的地へ向かって移動します
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
    private FSM manager;    //ステートマシン
    private Parameter parameter;    //設定された属性
    public ChaseState(FSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }
    public void OnEnter()
    {
        //アニメーションの再生
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
            manager.TranstionState(StateType.Hit);  //攻撃を受けたら、ダメージ受け状態に切り替えてください。
        }

        if (parameter.target)
        {
            manager.transform.position = Vector2.MoveTowards(
                manager.transform.position, parameter.target.position,
                parameter.chaseSpeed * Time.deltaTime);
        }
        if (parameter.target == null ||
        parameter.target.position.x < parameter.chaseLeftPoint.position.x ||
        parameter.target.position.x > parameter.chaseRightPoint.position.x)    //目標を見失ったり、最大追跡範囲を超えた場合は、パトロールに戻ります。
        {
            manager.TranstionState(StateType.Idle);
        }
        if (Physics2D.OverlapCircle(parameter.attackPoint.position, parameter.attackArea, parameter.targetLayer))   //攻撃範囲に入ったかどうかを検出する
        {
            manager.TranstionState(StateType.Attack);
        }
    }
}

public class ReactState : IState
{
    private FSM manager;    //ステートマシン
    private Parameter parameter;    //設定された属性
    //アニメーションの状態を取得する
    public ReactState(FSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }
    public void OnEnter()
    {
        //ステータス開始時にリアクションアニメーションを再生
    }

    public void OnExit()
    {

    }

    public void OnLateUpDade()
    {
    }

    public void OnUpDate()
    {
        //リアクションアニメーションの再生状態を取得する

        if (parameter.getHit)
        {
            manager.TranstionState(StateType.Hit);  //攻撃を受けたら、ダメージ受け状態に切り替えてください。
        }

        // if(反応アニメーションが終わりそうなら)
        // {
        //     if(parameter.target != null)
        //     {
        //         manager.TranstionState(StateType.Chase);    //目標がまだあれば追跡を開始
        //     }
        //     else
        //     {
        //         manager.TranstionState(StateType.Idle); //目標を見失ったら静止
        //     }
        // }
        if (parameter.target != null)
        {
            manager.FlipTo(parameter.target);   //まだ目標があるなら、方向を変えてください。
        }
    }
}

public class AttackState : IState
{
    private FSM manager;    //ステートマシン
    private Parameter parameter;    //設定された属性
    //アニメーションの状態を取得する
    public AttackState(FSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }
    public void OnEnter()
    {
        //アニメーションの再生
    }

    public void OnExit()
    {

    }

    public void OnUpDate()
    {
        //攻撃アニメーションの再生状態を取得

        if (parameter.getHit)
        {
            manager.TranstionState(StateType.Hit);  //攻撃を受けたら、ダメージ受け状態に切り替えてください。
        }

        // もし（アニメーションが終わりそうなら）
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
    private FSM manager;    //ステートマシン
    private Parameter parameter;    //設定された属性
    //アニメーション再生の進行状況を取得
    public HitState(FSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }
    public void OnEnter()
    {
        //ダメージ受けアニメーションを再生する
    }

    public void OnExit()
    {
        parameter.getHit = false;
    }

    public void OnUpDate()
    {
        //ダメージ受けアニメーションの再生状態を取得
        if (parameter.health <= 0)
        {
            manager.TranstionState(StateType.Dead);
        }
        else
        {
            parameter.target = GameObject.FindWithTag("Player").transform;  //TODO：攻撃を発したオブジェクトに基づいて追跡対象をロックする
            // if(傷つきアニメーションが終わりそう)
            // {
            //     manager.TranstionState(StateType.Chase);    //追跡状態に移行
            // }
        }
    }
    public void OnLateUpDade()
    {

    }
}

public class DeadState : IState
{
    private FSM manager;    //ステートマシン
    private Parameter parameter;    //設定された属性
    public DeadState(FSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }
    public void OnEnter()
    {
        //アニメーションの再生
    }

    public void OnExit()
    {

    }

    public void OnUpDate()
    {
        //TODO：一定時間後に敵を破壊する
    }
    public void OnLateUpDade()
    {
    }
}
