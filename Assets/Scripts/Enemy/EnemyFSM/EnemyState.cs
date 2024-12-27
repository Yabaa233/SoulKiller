using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


//在这里继承状态接口，实现所有的状态方案
public class EnemyState : IState
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


public class Enemy_IdleState : IState
{
    private EnemyFSM manager;//状态机
    private EnemyParameter parameter;//设置的属性

    public Enemy_IdleState(EnemyFSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }

    public void OnEnter()
    {
        //进入方法
        parameter.animator.Play("Idle");
    }

    public void OnExit()
    {
        //退出方法
    }

    public void OnLateUpDade()
    {
        //一帧结束之后要做的东西给
    }

    public void OnUpDate()
    {
        // Debug.Log("敌人处于待机状态");
        if(parameter.isDead)
        {
            manager.TranstionState(E_EnemyStateType.Dead);
            return;
        }
        if(parameter.getHit)
        {
            manager.TranstionState(E_EnemyStateType.Hit);//受到攻击，切换到受到攻击状态
            return;
        }

        if(parameter.ableChase)
        {
            manager.TranstionState(E_EnemyStateType.Find);//开始追击状态
        }
    }

}


public class Enemy_FindState : IState
{
    private EnemyFSM manager;//状态机
    private EnemyParameter parameter;//设置的属性

    public Enemy_FindState(EnemyFSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }

    public void OnEnter()
    {
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
        if(parameter.isDead)
        {
            manager.TranstionState(E_EnemyStateType.Dead);
            return;
        }
        if(parameter.getHit)
        {
            manager.TranstionState(E_EnemyStateType.Hit);//受到攻击，切换到受到攻击状态
            return;
        }
        manager.FaceToTarget();
        manager.RotateToTarget();
        manager.animatorInfo = parameter.animator.GetCurrentAnimatorStateInfo(0);
        if(manager.animatorInfo.normalizedTime > 0.99f && manager.animatorInfo.IsName("Find"))
        {
            if(Vector3.Distance(parameter.target.position,parameter.enemyPos.position) < parameter.enemyStateData.attackDistance&&
            manager.IsVisableInCamera)
            {
                // Debug.Log("进入蓄力状态");
                manager.TranstionState(E_EnemyStateType.Storage);//玩家在攻击范围之内，且自身在屏幕范围内，进入蓄力状态
            }
            else{
                // Debug.Log("进入追击状态");
                manager.TranstionState(E_EnemyStateType.Chase);//玩家不在攻击范围内则进入追击状态
            }
        }
    }
}


public class Enemy_ChaseState : IState
{
    private EnemyFSM manager;//状态机
    private EnemyParameter parameter;//设置的属性
    private NavMeshAgent agent;//导航组件
    public Enemy_ChaseState(EnemyFSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }

    public void OnEnter()
    {
        //进入方法
        agent = parameter.agent;
        agent.enabled = true;
        parameter.animator.Play("Chase");
    }

    public void OnExit()
    {
        //退出方法
        agent.enabled = false;//stop已经过时了
    }

    public void OnLateUpDade()
    {
        //一帧结束之后要做的东西给
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
        agent.speed = parameter.enemyStateData.moveSpeed;
        if(agent.isOnNavMesh)
        {
            agent.SetDestination(parameter.target.position);
        }
        manager.FaceToTarget();
        // manager.RotateToTarget();
        if(Vector3.Distance(parameter.target.position,parameter.enemyPos.position) < parameter.enemyStateData.attackDistance)
        {
            // Debug.Log("进入蓄力状态");
            manager.TranstionState(E_EnemyStateType.Storage);
        }
    }

}

public class Enemy_StorageState : IState
{
    private EnemyFSM manager;//状态机
    private EnemyParameter parameter;//设置的属性

    private float btwTime = 1.5f;
    private float nextStateTime;

    public Enemy_StorageState(EnemyFSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }

    public void OnEnter()
    {
         parameter.animator.Play("Storage");
        nextStateTime = Time.time + btwTime;
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
            return;
        }
        manager.animatorInfo = parameter.animator.GetCurrentAnimatorStateInfo(0);
       // if(manager.animatorInfo.normalizedTime >0.99f && manager.animatorInfo.IsName("Storage")&&)
            if (manager.animatorInfo.normalizedTime > 0.99f && manager.animatorInfo.IsName("Storage")&& manager.DashCD.flag)

            {
                manager.TranstionState(E_EnemyStateType.Attack);
        }
    }
}


public class Enemy_AttackState : IState
{
    private EnemyFSM manager;//状态机
    private EnemyParameter parameter;//设置的属性
    private float btwTime = 2f;
    private float nextStateTime;
    private Vector3 originalPos;//暂存
    private Vector3 attackPos;//攻击距离
    private Vector3 faceVector;//冲刺朝向
    public Enemy_AttackState(EnemyFSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }

    public void OnEnter()
    {
        //进入方法
        faceVector = parameter.target.position - parameter.enemyPos.position;
        nextStateTime = Time.time + btwTime;
        parameter.isDash = true;
        parameter.agent.enabled = false;
        parameter.animator.Play("Attack");
        //FmodManager.Instance.PlaySoundOnce(parameter.enemyStateData.dashEffect);
        FMODUnity.RuntimeManager.PlayOneShot(parameter.enemyStateData.dashEffect);
    }

    public void OnExit()
    {
        parameter.agent.enabled = true;

        //退出方法
    }

    public void OnLateUpDade()
    {
        //一帧结束之后要做的东西给
    }

    public void OnUpDate()
    {
        if(parameter.isDead)
        {
            manager.TranstionState(E_EnemyStateType.Dead);
            return;
        }
        if(parameter.isDash)
        {
            // Debug.Log("冲刺了一次");
            manager.FaceToTarget();
            manager.rb.AddForce(faceVector.normalized * parameter.enemyStateData.dashPower, ForceMode.Impulse);
            parameter.isDash = false;
        }
        if(manager.rb.velocity.magnitude < 1.5f&&Time.time > nextStateTime)//速度小于一个阈值,进入Find状态
        {
            manager.DashCD.flag = false;
            parameter.getHit = false;
            manager.rb.velocity = new Vector3(0f,0f,0f);
            manager.TranstionState(E_EnemyStateType.Find);
        }
    }

}

public class Enemy_DeadState : IState
{
    private EnemyFSM manager;
    private EnemyParameter parameter;

    public Enemy_DeadState(EnemyFSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }
    public void OnEnter()
    {
        //TO 销毁自身的一些组件
        parameter.isDead = true;
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


public class Enemy_HitState : IState
{
    private EnemyFSM manager;
    private EnemyParameter parameter;

    private float nextStateTime;
    private float timeBtwState = 0.5f;//至少在受击状态停留0.5s

    public Enemy_HitState(EnemyFSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }
    public void OnEnter()
    {
        manager.DashCD.curTime -= 0.5f;
        if(manager.DashCD.curTime < 0)
        {
            manager.DashCD.curTime = 0;
        }
        nextStateTime = Time.time;
        parameter.agent.enabled = false;
        parameter.animator.Play("GetHit");
        // FMODUnity.RuntimeManager.PlayOneShot("{f4bd562f-55ab-4be3-81fb-3a7d35511326}");
        //FmodManager.Instance.PlaySoundOnce(parameter.enemyData.getHitSound);
        //FMODUnity.RuntimeManager.PlayOneShot(parameter.enemyData.getHitSound);


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
        if(parameter.isDead)
        {
            manager.TranstionState(E_EnemyStateType.Dead);
        }
        manager.animatorInfo = parameter.animator.GetCurrentAnimatorStateInfo(0);
        if(Time.time - nextStateTime>timeBtwState)
        {
            manager.TranstionState(E_EnemyStateType.Find);
        }
    }
}


