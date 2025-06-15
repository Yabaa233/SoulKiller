using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ダッシュ敵の状態
/// </summary>
public class DashEnemyState : IState
{
    public void OnEnter()
    {
        
    }

    public void OnExit()
    {
        
    }

    public void OnLateUpDade()
    {

    }

    public void OnUpDate()
    {

    }
}

///<summary>


////// 状態の初期化


///</summary>
public class DashEnemy_IdleState : IState
{
    private DashEnemyFSM manager;
    private DashEnemyParameter parameter;

    public DashEnemy_IdleState(DashEnemyFSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }

    public void OnEnter()
    {
        // Debug.Log("スタンバイモードに入りました");
        parameter.agent.enabled = false;
        parameter.animator.Play("Idle");
    }

    public void OnExit()
    {
        
    }

    public void OnLateUpDade()
    {
        
    }

    public void OnUpDate()
    {
        if(parameter.isDead)
        {
            manager.TranstionState(E_EnemyStateType.Dead);
            return;
        }
        if(parameter.getHit)
        {
            manager.TranstionState(E_EnemyStateType.Hit);//攻撃を受けて状態を切り替えます
            return;
        }
        if(parameter.ableAttact)
        {
            manager.TranstionState(E_EnemyStateType.Find);//警戒距離に到達し、衝突型の敵に対するジャンプのヒント
        }
    }
}


///<summary>



////// 状態の更新



///</summary>
public class DashEnemy_FindState : IState
{
    private DashEnemyFSM manager;
    private DashEnemyParameter parameter;
    private float timer = 1.5f;
    private float nextStateTime;
    public DashEnemy_FindState(DashEnemyFSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }

    public void OnEnter()
    {
        // Debug.Log("発見状態に入りました");
        nextStateTime = Time.time + timer;
        parameter.animator.Play("Find");
        // FmodManager.Instance.PlaySoundOnce(parameter.enemyData.)
        //FMODUnity.RuntimeManager.PlayOneShot("{d27a9bd4-6103-4727-8fb6-99e714db9599}");
    }

    public void OnExit()
    {
        
    }

    public void OnLateUpDade()
    {
        
    }

    public void OnUpDate()
    {
        manager.RotateToTarget();
        manager.FaceToTarget();
        //攻撃を受けると、直接に被弾状態になります。
        if(parameter.getHit)
        {
            manager.TranstionState(E_EnemyStateType.Hit);//攻撃を受け、攻撃受け状態に切り替えます。
            return;
        }
        //跳ねるアニメーションを再生する
        AnimatorStateInfo animatorInfo;
        animatorInfo = parameter.animator.GetCurrentAnimatorStateInfo(0);
        if(animatorInfo.normalizedTime > 0.99f && animatorInfo.IsName("Find")&&Time.time > nextStateTime && manager.DashCD.flag)
        {
            manager.TranstionState(E_EnemyStateType.Chase);
        }
    }
}

/// <summary>
/// ステータスの終了
/// </summary>
public class DashEnemy_ChaseState : IState
{
    private DashEnemyFSM manager;
    private DashEnemyParameter parameter;
    // Vector3 faceVector; //プレイヤーに向かうベクトル
    // private float timer = 1.5f;
    // private float nextStateTime;
    public DashEnemy_ChaseState(DashEnemyFSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }

    public void OnEnter()
    {
        // Debug.Log("スプリント状態に入ります");
        //この瞬間にキャラクターに向かっているベクトルを得る
        // faceVector = parameter.target.position - parameter.enemyPos.position;
        // parameter.isDash = true;
        // nextStateTime = Time.time + timer;
        parameter.animator.Play("Chase");
        parameter.agent.enabled = true;
        parameter.agent.updateRotation = false;
    }

    public void OnExit()
    {
        parameter.agent.enabled = false;
    }

    public void OnLateUpDade()
    {
        
    }

    public void OnUpDate()
    {
        //新しい追跡ロジック
        if(parameter.isDead)
        {
            manager.TranstionState(E_EnemyStateType.Dead);
            return;
        }
        if(parameter.getHit)
        {
            manager.TranstionState(E_EnemyStateType.Hit);
            return;
        }
        if (Vector3.Distance(parameter.target.position, parameter.enemyPos.position) < parameter.enemyStateData.chaseDistance)
        {
            manager.TranstionState(E_EnemyStateType.Idle);
        }
        parameter.agent.speed = parameter.moveSpeed;
        if(parameter.agent.isOnNavMesh)
        {
            parameter.agent.SetDestination(parameter.target.position);
        }
        manager.FaceToTarget();
        manager.RotateToTarget();
        // if(parameter.isDash)
        // {
        //     // Debug.Log("一度ダッシュしました");
        //     manager.FaceToTarget();
        //     manager.rb.AddForce(faceVector.normalized * parameter.enemyStateData.dashPower, ForceMode.Impulse);
        //     parameter.isDash = false;
        // }
        // if(parameter.isDizzy)
        // {
        //     manager.TranstionState(E_EnemyStateType.Dizzy);//壁にぶつかってめまい状態になる
        //     return;
        // }
        // if(manager.rb.velocity.magnitude < 1.5f && Time.time > nextStateTime)//速度が閾値より小さい場合、Find状態に移行します
        // {
        //     manager.rb.velocity = new Vector3(0f,0f,0f);
        //     parameter.getHit = false;
        //     manager.DashCD.flag = false;
        //     manager.TranstionState(E_EnemyStateType.Find);
        // }
    }
}


//目まい状態でやるべきことは、ただしばらく静止するだけです。
/// <summary>
/// めまい状態
/// </summary>
public class DashEnemy_DizzyState : IState
{
    private DashEnemyFSM manager;
    private DashEnemyParameter parameter;
    private float timer = 2f;
    private float nextStateTime;
    public DashEnemy_DizzyState(DashEnemyFSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }

    public void OnEnter()
    {
        Debug.Log("めまいの状態になりました");
        parameter.isDizzy = false;
        nextStateTime = Time.time + timer;
        manager.DashCD.curTime = 0;
        parameter.animator.Play("Dizzy");
    }

    public void OnExit()
    {
        
    }

    public void OnLateUpDade()
    {
        
    }

    public void OnUpDate()
    {
        if(parameter.getHit)
        {
            manager.TranstionState(E_EnemyStateType.Hit);//攻撃を受け、攻撃受け状態に切り替えます。
            return;
        }
        if(Time.time > nextStateTime)
        {
            manager.TranstionState(E_EnemyStateType.Idle);//まず、ここで待機状態に戻してください。
            parameter.ableAttact = false;
        }
    }
}

///<summary>


////// 被打击的状态


///</summary>
public class DashEnemy_GetHitState : IState
{
    private DashEnemyFSM manager;
    private DashEnemyParameter parameter;
    
    private float timeBtwState = 0.5f;//少なくとも0.5秒間、打撃状態に留まる
    private float nextStateTime;

    private bool isHit = true;
    public DashEnemy_GetHitState(DashEnemyFSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }

    public void OnEnter()
    {
        //攻撃を受ける方法
        isHit = true;
        parameter.getHit = false;
        nextStateTime = Time.time + timeBtwState;
        parameter.animator.Play("GetHit");
        //FMODUnity.RuntimeManager.PlayOneShot(parameter.enemyData.getHitSound);

        //FmodManager.Instance.PlaySoundOnce(parameter.enemyData.getHitSound);

        //CDを減らす
        manager.DashCD.curTime -= 0.5f;
        if(manager.DashCD.curTime < 0)
        {
            manager.DashCD.curTime = 0;
        }
    }

    public void OnExit()
    {
        
    }

    public void OnLateUpDade()
    {
        
    }

    public void OnUpDate()
    {
        if(parameter.isDead)
        {
            manager.TranstionState(E_EnemyStateType.Dead);
        }
        Vector3 backVector = parameter.enemyPos.position - parameter.target.position;
        if(isHit)
        {
            backVector.y = 0;
            // manager.rb.AddForce(backVector.normalized * 5, ForceMode.Impulse);
            isHit = false;
        }
        if(parameter.getHit)//何度もノックする
        {
            parameter.animator.Play("GetHit",0,0f);
            manager.TranstionState(E_EnemyStateType.Hit);
        }
        if(Time.time - nextStateTime>timeBtwState && manager.rb.velocity.magnitude < 1.5f)
        {
            manager.TranstionState(E_EnemyStateType.Find);
        }
    }
}

///<summary>


////// 死亡状態


///</summary>
public class DashEnemy_DeadState :IState
{
    private DashEnemyFSM manager;
    private DashEnemyParameter parameter;
    public DashEnemy_DeadState(DashEnemyFSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }

    public void OnEnter()
    {
        parameter.isDead = true;
        parameter.agent.enabled = false;
        parameter.animator.Play("Dead");
        GameManager.Instance.CDList.Remove(manager.DashCD);
    }

    public void OnExit()
    {
        
    }

    public void OnLateUpDade()
    {
        
    }

    public void OnUpDate()
    {
        AnimatorStateInfo animatorInfo;
        animatorInfo = parameter.animator.GetCurrentAnimatorStateInfo(0);
        if(animatorInfo.normalizedTime > 0.99f && animatorInfo.IsName("Dead"))
        {
            GameObject.Destroy(parameter.enemyPos.gameObject);
        }
    }
}
