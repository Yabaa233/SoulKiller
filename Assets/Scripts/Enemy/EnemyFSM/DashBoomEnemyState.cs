using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ダッシュ爆発の敵の状態
/// </summary>
public class DashBoomEnemyState : IState
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

///<summary>


////// 状態の初期化


///</summary>
public class DBoomEnemy_IdleState : IState
{
    private DashBoomEnemyFSM manager;
    private DashBoomEnemyParameter parameter;

    public DBoomEnemy_IdleState(DashBoomEnemyFSM _manager)
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
        if(parameter.isBoom)
        {
            manager.TranstionState(E_EnemyStateType.Boom);
            return;
        }
        if(parameter.getHit)
        {
            manager.TranstionState(E_EnemyStateType.Hit);//攻撃を受けて状態を切り替えます
            return;
        }
        if(parameter.ableAttact)
        {
            manager.TranstionState(E_EnemyStateType.Find);//警戒距離に到達し、衝突型の敵に対する跳躍のヒント
            return;
        }
    }
}

///<summary>


////// 状態の更新


///</summary>
public class DBoomEnemy_FindState : IState
{
    private DashBoomEnemyFSM manager;
    private DashBoomEnemyParameter parameter;
    private float timer = 1f;
    private float nextStateTime;
    public DBoomEnemy_FindState(DashBoomEnemyFSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }

    public void OnEnter()
    {
        Debug.Log("ディスカバリーモードに入りました");
        nextStateTime = Time.time + timer;
        parameter.animator.Play("Find");
    }

    public void OnExit()
    {
        
    }
    public void OnLateUpDade()
    {
        
    }

    public void OnUpDate()
    {
        manager.FaceToTarget();
        manager.RotateToTarget();
        //攻撃を受けると直接被弾状態に入ります。
        if(parameter.isBoom)
        {
             manager.TranstionState(E_EnemyStateType.Boom);
             return;
        }
        if(parameter.getHit)
        {
            manager.TranstionState(E_EnemyStateType.Hit);//攻撃を受け、攻撃受け状態に切り替えます。
        }
        //跳ねるアニメーションを再生する
        AnimatorStateInfo animatorInfo;
        animatorInfo = parameter.animator.GetCurrentAnimatorStateInfo(0);
        if(animatorInfo.normalizedTime > 0.99f && animatorInfo.IsName("Find")&&Time.time > nextStateTime)
        {
            manager.TranstionState(E_EnemyStateType.Chase);
        }
    }
}


/// <summary>
/// ステータスの終了
/// </summary>
public class DBoomEnemy_ChaseState : IState
{
    private DashBoomEnemyFSM manager;
    private DashBoomEnemyParameter parameter;
    // Vector3 faceVector;//プレイヤーに向かうベクトル
    // private float timer = 1.5f;
    // private float nextStateTime;
    public DBoomEnemy_ChaseState(DashBoomEnemyFSM _manager)
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
        // parameter.animator.Play("Chase");
        parameter.animator.Play("Chase");
        parameter.agent.enabled = true;
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
        if(parameter.isBoom)
        {
             manager.TranstionState(E_EnemyStateType.Boom);
             return;
        }
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
        parameter.agent.speed = parameter.moveSpeed;
        if(parameter.agent.isOnNavMesh)
        {
            parameter.agent.SetDestination(parameter.target.position);
        }
        manager.FaceToTarget();
        manager.RotateToTarget();
        //元のスプリントロジック
        // if(parameter.isDash)
        // {
        //     // Debug.Log("一度ダッシュしました");
        //     manager.FaceToTarget();
        //     manager.RotateToTarget();
        //     manager.rb.AddForce(faceVector.normalized * parameter.enemyStateData.dashPower, ForceMode.Impulse);
        //     parameter.isDash = false;
        // }
        // if(parameter.isBoom)
        // {
        //      manager.TranstionState(E_EnemyStateType.Boom);
        //      return;
        // }
        // if(parameter.getHit)
        // {
        //      manager.TranstionState(E_EnemyStateType.Hit);
        //      return;
        // }
        // if(parameter.isDizzy)
        // {
        //     manager.TranstionState(E_EnemyStateType.Dizzy);//壁にぶつかってめまい状態に入る
        //     return;
        // }
        // if(manager.rb.velocity.magnitude < 1.5f && Time.time > nextStateTime)//速度が閾値より小さい場合、Find状態に入ります
        // {   
        //     manager.rb.velocity = new Vector3(0f,0f,0f);
        //     manager.TranstionState(E_EnemyStateType.Find);
        // }
    }
}

//めまい状態でやるべきことは、ただしばらく静止するだけです。
/// <summary>
/// めまい状態
/// </summary>
public class DBoomEnemy_DizzyState : IState
{
    private DashBoomEnemyFSM manager;
    private DashBoomEnemyParameter parameter;
    private float timer = 1.5f;
    private float nextStateTime;
    public DBoomEnemy_DizzyState(DashBoomEnemyFSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }

    public void OnEnter()
    {
        // Debug.Log("めまいの状態になりました");
        parameter.isDizzy = false;
        nextStateTime = Time.time + timer;
        parameter.animator.Play("GetHit");
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
        }
        if(Time.time > nextStateTime)
        {
            manager.TranstionState(E_EnemyStateType.Idle);//ここでは、まず待機状態に戻してください。
        }
    }
}


///<summary>



////// 打撃を受けた状態



///</summary>
public class  DBoomEnemy_GetHitState: IState
{
    private DashBoomEnemyFSM manager;
    private DashBoomEnemyParameter parameter;
    
    private float timeBtwState = 0.3f;//被打撃状態に少なくとも0.5秒間留まる
    private float nextStateTime;
    public DBoomEnemy_GetHitState(DashBoomEnemyFSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }

    public void OnEnter()
    {
        nextStateTime = Time.time + timeBtwState;
        parameter.animator.Play("GetHit");
        // Debug.Log("打たれる状態に入りました。");
         //FMODUnity.RuntimeManager.PlayOneShot(parameter.enemyData.getHitSound);
        // FmodManager.Instance.PlaySoundOnce(parameter.enemyData.getHitSound);

    }

    public void OnExit()
    {
        
    }

    public void OnLateUpDade()
    {
        
    }

    public void OnUpDate()
    {
        //  Debug.Log("ブームに入る");
        if(parameter.enemyData.currentHealth <=0)
        {
            manager.TranstionState(E_EnemyStateType.Boom);
            return;
        }
        // if(Time.time - nextStateTime>timeBtwState)
        // {
        //     manager.TranstionState(E_EnemyStateType.Find);
        // }
        if(Time.time>nextStateTime)
        {
            manager.TranstionState(E_EnemyStateType.Boom);
            return;
        }
        manager.TranstionState(E_EnemyStateType.Boom);
    }
}


///<summary>



////// 爆発状態



///</summary>
public class DBoomEnemy_BoomState : IState
{
    private DashBoomEnemyFSM manager;
    private DashBoomEnemyParameter parameter;
    private float timer = 1f;
    private float nextStateTime;
    private Vector3 nowPos;//現在の位置を一時保存する
    public DBoomEnemy_BoomState(DashBoomEnemyFSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }

    public void OnEnter()
    {
        // Debug.Log("爆発状態に入りました");
        timer = parameter.enemyStateData.boomDelay;
        nextStateTime = Time.time + timer;
        nowPos = parameter.enemyPos.position;
        manager.RecycleStatePanel();
        parameter.animator.Play("Boom");
        //FmodManager.Instance.PlaySoundOnce(parameter.enemyStateData.boomEffect);
        FMODUnity.RuntimeManager.PlayOneShot(parameter.enemyStateData.boomEffect);
    }

    public void OnExit()
    {
        
    }

    public void OnLateUpDade()
    {
        
    }

    public void OnUpDate()
    {
        parameter.enemyPos.position = 
        new Vector3(nowPos.x,Random.Range(nowPos.y,nowPos.y + 1),nowPos.z);
        if(Time.time > nextStateTime)
        {
            parameter.isBoom = false;
            manager.TranstionState(E_EnemyStateType.Dead);
        }
    }
}


///<summary>



////// 死亡状態



///</summary>
public class DBoomEnemy_DeadState :IState
{
    private DashBoomEnemyFSM manager;
    private DashBoomEnemyParameter parameter;
    public DBoomEnemy_DeadState(DashBoomEnemyFSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }

    public void OnEnter()
    {
        parameter.isDead = true;
        parameter.agent.enabled = false;
        parameter.animator.Play("Dead");
        GameObject boomEffect = ObjectPool.Instance.GetObject("Effect_BoomShock",parameter.enemyPos.transform,true,true);
        boomEffect.SetActive(true);
        boomEffect.transform.position = parameter.enemyPos.position;
        boomEffect.transform.GetChild(0).GetComponent<ParticleSystem>().Play();

        //ここで自爆のダメージ判定を行います
        if(parameter.playerIsStay)
        {
            GameManager.Instance.EnemyAttack(parameter.enemyPos.gameObject.GetComponent<BaseEnemyControl>());
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
        AnimatorStateInfo animatorInfo;
        animatorInfo = parameter.animator.GetCurrentAnimatorStateInfo(0);
        if(animatorInfo.normalizedTime > 0.99f && animatorInfo.IsName("Dead"))
        {
            if(parameter.enemyData.currentHealth >0)
            {
                manager.GetComponent<BaseEnemyControl>().Die();
            }
            GameObject.Destroy(parameter.enemyPos.gameObject);
        }
    }
}

