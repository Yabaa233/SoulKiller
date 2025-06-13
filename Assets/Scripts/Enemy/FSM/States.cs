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
    private FSM manager;    //状態マシン
    private Parameter parameter;    //設定された属性
    private float idleTimer;    //待機時間
    public IdleState(FSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }
    public void OnEnter()
    {
        //アニメーション再生
    }

    public void OnExit()
    {
        idleTimer = 0;
    }

    public void OnLateUpDade()
    {
        //1フレーム終了後の処理
    }

    public void OnUpDate()
    {
        idleTimer += Time.deltaTime;
        if (parameter.getHit)
        {
            manager.TranstionState(StateType.Hit);  //攻撃を受けたら被ダメージ状態に切り替え
        }

        //プレイヤーを発見したかどうかの判定
        if (parameter.target != null &&
        parameter.target.position.x >= parameter.chaseLeftPoint.position.x &&
        parameter.target.position.x <= parameter.chaseRightPoint.position.x)
        {
            manager.TranstionState(StateType.React);    //プレイヤーを発見したら反応状態に切り替え
        }

        if (idleTimer >= parameter.idleTime) //設定時間に達したらパトロール状態に切り替え
        {
            manager.TranstionState(StateType.Patrol);
        }
    }
}
public class PatrolState : IState
{
    private FSM manager;    //状態マシン
    private Parameter parameter;    //設定された属性
    private int patrolPoint;    //現在のパトロールポイント
    public PatrolState(FSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }
    public void OnEnter()
    {
        //対応するアニメーションを再生
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
            manager.TranstionState(StateType.Hit);  //攻撃を受けたら被ダメージ状態に切り替え
        }

        //プレイヤーを発見したかどうかの判定
        if (parameter.target != null &&
        parameter.target.position.x >= parameter.chaseLeftPoint.position.x &&
        parameter.target.position.x <= parameter.chaseRightPoint.position.x)
        {
            manager.TranstionState(StateType.React);    //プレイヤーを発見したら反応状態に切り替え
        }

        //目標地点に向かって移動
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
    private FSM manager;    //状態マシン
    private Parameter parameter;    //設定された属性
    public ChaseState(FSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }
    public void OnEnter()
    {
        //アニメーション再生
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
            manager.TranstionState(StateType.Hit);  //攻撃を受けたら被ダメージ状態に切り替え
        }

        if (parameter.target)
        {
            manager.transform.position = Vector2.MoveTowards(
                manager.transform.position, parameter.target.position,
                parameter.chaseSpeed * Time.deltaTime);
        }
        if (parameter.target == null ||
        parameter.target.position.x < parameter.chaseLeftPoint.position.x ||
        parameter.target.position.x > parameter.chaseRightPoint.position.x)    //目標を見失ったか最大追跡範囲を超えたらパトロールに戻る
        {
            manager.TranstionState(StateType.Idle);
        }
        if (Physics2D.OverlapCircle(parameter.attackPoint.position, parameter.attackArea, parameter.targetLayer))   //攻撃範囲に入ったかどうかを検出
        {
            manager.TranstionState(StateType.Attack);
        }
    }
}

public class ReactState : IState
{
    private FSM manager;    //状態マシン
    private Parameter parameter;    //設定された属性
    //アニメーション状態を取得
    public ReactState(FSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }
    public void OnEnter()
    {
        //状態開始時に反応アニメーションを再生
    }

    public void OnExit()
    {

    }

    public void OnLateUpDade()
    {
    }

    public void OnUpDate()
    {
        //反応アニメーションの再生状態を取得

        if (parameter.getHit)
        {
            manager.TranstionState(StateType.Hit);  //攻撃を受けたら被ダメージ状態に切り替え
        }

        // if(反応アニメーションが終わりそう)
        // {
        //     if(parameter.target != null)
        //     {
        //         manager.TranstionState(StateType.Chase);    //目標がまだあれば追跡開始
        //     }
        //     else
        //     {
        //         manager.TranstionState(StateType.Idle); //目標を見失ったら静止
        //     }
        // }
        if (parameter.target != null)
        {
            manager.FlipTo(parameter.target);   //目標がまだあれば向きを変える
        }
    }
}

public class AttackState : IState
{
    private FSM manager;    //状態マシン
    private Parameter parameter;    //設定された属性
    //アニメーション状態を取得
    public AttackState(FSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }
    public void OnEnter()
    {
        //アニメーション再生
    }

    public void OnExit()
    {

    }

    public void OnUpDate()
    {
        //攻撃アニメーションの再生状態を取得

        if (parameter.getHit)
        {
            manager.TranstionState(StateType.Hit);  //攻撃を受けたら被ダメージ状態に切り替え
        }

        // if(アニメーションが終わりそう)
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
    private FSM manager;    //状態マシン
    private Parameter parameter;    //設定された属性
    //アニメーション再生の進捗を取得
    public HitState(FSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }
    public void OnEnter()
    {
        //被ダメージアニメーションを再生
    }

    public void OnExit()
    {
        parameter.getHit = false;
    }

    public void OnUpDate()
    {
        //被ダメージアニメーションの再生状態を取得
        if (parameter.health <= 0)
        {
            manager.TranstionState(StateType.Dead);
        }
        else
        {
            parameter.target = GameObject.FindWithTag("Player").transform;  //TODO:攻撃を発したオブジェクトに基づいて追跡対象をロック
            // if(受傷アニメーションが終わりそう)
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
    private FSM manager;    //状態マシン
    private Parameter parameter;    //設定された属性
    public DeadState(FSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }
    public void OnEnter()
    {
        //アニメーション再生
    }

    public void OnExit()
    {

    }

    public void OnUpDate()
    {
        //TODO:一定時間後に敵を破壊
    }
    public void OnLateUpDade()
    {
    }
}
