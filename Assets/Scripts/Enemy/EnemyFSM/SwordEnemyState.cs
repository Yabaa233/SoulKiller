using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
///<summary>

////// 剣の敵の状態

///</summary>
public class SwordEnemyState : IState
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
public class SwordEnemy_IdleState : IState
{
    private SwordEnemyFSM manager;
    private SwordEnemyParameter parameter;

    public SwordEnemy_IdleState(SwordEnemyFSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }

    public void OnEnter()
    {
        // Debug.Log("スタンバイモードに入りました");
        parameter.agent.enabled = false;
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
            manager.TranstionState(E_EnemyStateType.Hit);
        }
        // Debug.Log("待機状態にあります");
        if(parameter.ableAttact&&manager.AttackCD.flag)
        {
            manager.TranstionState(E_EnemyStateType.Chase);//警戒距離に達したら追撃を開始します。
        }
    }
}

///<summary>


////// 状態の更新


///</summary>
public class SwordEnemy_ChaseState : IState
{
    private SwordEnemyFSM manager;
    private SwordEnemyParameter parameter;
    public SwordEnemy_ChaseState(SwordEnemyFSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }

    public void OnEnter()
    {
        // Debug.Log("追跡状態に入ります");
        parameter.agent.enabled = true;
        parameter.agent.updatePosition = false;
        parameter.animator.Play("Chase");
    }

    public void OnExit()
    {
        
    }

    public void OnLateUpDade()
    {
        //Vector3 offsetTarget = new Vector3(-3f, 0, -3f);
        if(parameter.agent.isOnNavMesh)
        {
            parameter.agent.SetDestination(parameter.target.position);
        }
        Vector3 realVelocity = parameter.agent.nextPosition - parameter.enemyPos.position;
        realVelocity.y = 0;
        manager.rb.velocity = realVelocity.normalized * parameter.moveSpeed;
        manager.RotateToTarget();
        manager.FaceToTarget();
        // if(Vector3.Distance(parameter.target.position,parameter.enemyPos.position) < parameter.enemyStateData.attackDistance && manager.AttackCD.flag)
        // {
        //     manager.TranstionState(E_EnemyStateType.Storage);
        // }
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
            manager.TranstionState(E_EnemyStateType.Hit);
            return;
        }
        // parameter.agent.SetDestination(parameter.target.position);
        // Vector3 realVelocity = parameter.agent.nextPosition - parameter.enemyPos.position;
        // realVelocity.y = 0;
        // manager.rb.velocity = realVelocity.normalized * 3.5f;
        // manager.FaceToTarget();
        // manager.RotateToTarget();
        if(Vector3.Distance(parameter.target.position,parameter.enemyPos.position) < parameter.enemyStateData.attackDistance && manager.AttackCD.flag)
        {
            manager.TranstionState(E_EnemyStateType.Storage);
        }
    }
}

/// <summary>
/// ステータスの終了
/// </summary>
public class SwordEnemy_StorageState :IState
{
    private SwordEnemyFSM manager;
    private SwordEnemyParameter parameter;
    private float btwTime = 0f;
    private float nextStateTime;
    
    public SwordEnemy_StorageState(SwordEnemyFSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }

    public void OnEnter()
    {
        parameter.agent.enabled = false;
        nextStateTime = Time.time + btwTime;
        manager.rb.velocity = new Vector3(0f,0f,0f);
        parameter.animator.Play("Storage");
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
        // if(parameter.getHit)//一時的に中断不可とする
        // {
        //     manager.TranstionState(E_EnemyStateType.Hit);
        //     return;
        // }
        manager.RotateToTarget();
        manager.animatorInfo = parameter.animator.GetCurrentAnimatorStateInfo(0);
        if(manager.animatorInfo.normalizedTime >0.99f && manager.animatorInfo.IsName("Storage"))
        {
            manager.TranstionState(E_EnemyStateType.Attack);
        }
    }
}

/// <summary>
/// ステータスの終了
/// </summary>
public class SwordEnemy_AttackState : IState
{
    private SwordEnemyFSM manager;
    private SwordEnemyParameter parameter;
    
    public SwordEnemy_AttackState(SwordEnemyFSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }

    public void OnEnter()
    {
        parameter.agent.enabled = false;//攻撃状態下でパスファインディングをオフにする
        manager.FaceToTarget();
        manager.RotateToTarget();
        parameter.animator.Play("TribleAttack");
        //FmodManager.Instance.PlaySoundOnce(parameter.enemyStateData.swordEffect,parameter.body.gameObject);
        //FmodManager.Instance.PlaySoundOnce(parameter.enemyStateData.swordEffect);
        FMODUnity.RuntimeManager.PlayOneShot(parameter.enemyStateData.swordEffect);


    }

    public void OnExit()
    {
        parameter.agent.enabled = true;//攻撃状態下でパスファインディングをオフにする
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
        AnimatorStateInfo animatorInfo;
        animatorInfo = parameter.animator.GetCurrentAnimatorStateInfo(0);
        if(animatorInfo.normalizedTime > 0.99f && animatorInfo.IsName("TribleAttack")&&manager.AttackCD.flag)
        {
            parameter.getHit = false;
            manager.AttackCD.flag = false;
            manager.TranstionState(E_EnemyStateType.Idle);
        }
    }
}

/// <summary>
/// ステータスの終了
/// </summary>
public class SwordEnemy_GetHitState : IState
{
    private SwordEnemyFSM manager;
    private SwordEnemyParameter parameter;
    private float nextStateTime;
    private float timeBtwState = 0.3f;//少なくとも被打撃状態に留まる
    public SwordEnemy_GetHitState(SwordEnemyFSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }

    public void OnEnter()
    {
        //攻撃を受ける方法
        nextStateTime = Time.time;
        parameter.agent.enabled = false;
        parameter.animator.Play("GetHit");
       // FMODUnity.RuntimeManager.PlayOneShot(parameter.enemyData.getHitSound);
        //FmodManager.Instance.PlaySoundOnce(parameter.enemyData.getHitSound);

    }

    public void OnExit()
    {
        parameter.getHit = false;
        parameter.agent.enabled = true;
    }

    public void OnLateUpDade()
    {
        
    }

    public void OnUpDate()
    {
        if(parameter.enemyData.currentHealth <=0)
        {
            manager.TranstionState(E_EnemyStateType.Dead);
            return;
        }

        AnimatorStateInfo animatorInfo;
        animatorInfo = parameter.animator.GetCurrentAnimatorStateInfo(0);
        if(Time.time - nextStateTime>timeBtwState && animatorInfo.normalizedTime > 0.99f && animatorInfo.IsName("GetHit")&& manager.AttackCD.flag)
        {
            manager.TranstionState(E_EnemyStateType.Chase);
        }
    }
}

/// <summary>
/// ステータスの終了
/// </summary>
public class SwordEnemy_DeadState : IState
{
    private SwordEnemyFSM manager;
    private SwordEnemyParameter parameter;
    public SwordEnemy_DeadState(SwordEnemyFSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }

    public void OnEnter()
    {
        parameter.isDead = true;
        parameter.agent.enabled = false;
        parameter.animator.Play("Dead");
        GameManager.Instance.CDList.Remove(manager.AttackCD);
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
