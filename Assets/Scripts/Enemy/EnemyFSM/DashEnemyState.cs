using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        // Debug.Log("进入了待机状态");
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
            manager.TranstionState(E_EnemyStateType.Hit);//受到攻击切换状态
            return;
        }
        if(parameter.ableAttact)
        {
            manager.TranstionState(E_EnemyStateType.Find);//到了警戒距离，冲撞类敌人的弹跳提示
        }
    }
}


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
        // Debug.Log("进入了发现状态");
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
        //受到攻击直接进入受击状态
        if(parameter.getHit)
        {
            manager.TranstionState(E_EnemyStateType.Hit);//受到攻击，切换到受到攻击状态
            return;
        }
        //播放弹跳的动画
        AnimatorStateInfo animatorInfo;
        animatorInfo = parameter.animator.GetCurrentAnimatorStateInfo(0);
        if(animatorInfo.normalizedTime > 0.99f && animatorInfo.IsName("Find")&&Time.time > nextStateTime && manager.DashCD.flag)
        {
            manager.TranstionState(E_EnemyStateType.Chase);
        }
    }
}

public class DashEnemy_ChaseState : IState
{
    private DashEnemyFSM manager;
    private DashEnemyParameter parameter;
    // Vector3 faceVector;//朝向玩家的向量
    // private float timer = 1.5f;
    // private float nextStateTime;
    public DashEnemy_ChaseState(DashEnemyFSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }

    public void OnEnter()
    {
        // Debug.Log("进入冲刺状态");
        //得到这一时刻朝向角色的向量
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
        //新的追踪逻辑
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
        //     // Debug.Log("冲刺了一次");
        //     manager.FaceToTarget();
        //     manager.rb.AddForce(faceVector.normalized * parameter.enemyStateData.dashPower, ForceMode.Impulse);
        //     parameter.isDash = false;
        // }
        // if(parameter.isDizzy)
        // {
        //     manager.TranstionState(E_EnemyStateType.Dizzy);//撞到墙体进入眩晕状态
        //     return;
        // }
        // if(manager.rb.velocity.magnitude < 1.5f&&Time.time > nextStateTime)//速度小于一个阈值,进入Find状态
        // {
        //     manager.rb.velocity = new Vector3(0f,0f,0f);
        //     parameter.getHit = false;
        //     manager.DashCD.flag = false;
        //     manager.TranstionState(E_EnemyStateType.Find);
        // }
    }
}


//眩晕状态要做的只是单纯停留一段时间
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
        Debug.Log("进入了眩晕状态");
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
            manager.TranstionState(E_EnemyStateType.Hit);//受到攻击，切换到受到攻击状态
            return;
        }
        if(Time.time > nextStateTime)
        {
            manager.TranstionState(E_EnemyStateType.Idle);//这里先让它回到待机状态
            parameter.ableAttact = false;
        }
    }
}

public class DashEnemy_GetHitState : IState
{
    private DashEnemyFSM manager;
    private DashEnemyParameter parameter;
    
    private float timeBtwState = 0.5f;//在受击状态停留至少0.5s
    private float nextStateTime;

    private bool isHit = true;
    public DashEnemy_GetHitState(DashEnemyFSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }

    public void OnEnter()
    {
        //TODO受到攻击的方法
        isHit = true;
        parameter.getHit = false;
        nextStateTime = Time.time + timeBtwState;
        parameter.animator.Play("GetHit");
        //FMODUnity.RuntimeManager.PlayOneShot(parameter.enemyData.getHitSound);

        //FmodManager.Instance.PlaySoundOnce(parameter.enemyData.getHitSound);

        //减少CD
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
        if(parameter.getHit)//多次敲击
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
