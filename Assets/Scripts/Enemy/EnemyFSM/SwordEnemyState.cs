using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
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
        // Debug.Log("进入了待机状态");
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
        // Debug.Log("处于待机状态");
        if(parameter.ableAttact&&manager.AttackCD.flag)
        {
            manager.TranstionState(E_EnemyStateType.Chase);//到了警戒距离开始追击
        }
    }
}

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
        // Debug.Log("进入追踪状态");
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
        // if(parameter.getHit)//暂定为不可被打断
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
        parameter.agent.enabled = false;//攻击状态下关闭寻路
        manager.FaceToTarget();
        manager.RotateToTarget();
        parameter.animator.Play("TribleAttack");
        //FmodManager.Instance.PlaySoundOnce(parameter.enemyStateData.swordEffect,parameter.body.gameObject);
        //FmodManager.Instance.PlaySoundOnce(parameter.enemyStateData.swordEffect);
        FMODUnity.RuntimeManager.PlayOneShot(parameter.enemyStateData.swordEffect);


    }

    public void OnExit()
    {
        parameter.agent.enabled = true;//攻击状态下关闭寻路
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

public class SwordEnemy_GetHitState : IState
{
    private SwordEnemyFSM manager;
    private SwordEnemyParameter parameter;
    private float nextStateTime;
    private float timeBtwState = 0.3f;//至少在受击状态停留
    public SwordEnemy_GetHitState(SwordEnemyFSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }

    public void OnEnter()
    {
        //TODO受到攻击的方法
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
