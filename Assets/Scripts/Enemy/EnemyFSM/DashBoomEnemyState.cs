using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if(parameter.isBoom)
        {
            manager.TranstionState(E_EnemyStateType.Boom);
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
            return;
        }
    }
}

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
        Debug.Log("进入了发现状态");
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
        //受到攻击直接进入受击状态
        if(parameter.isBoom)
        {
             manager.TranstionState(E_EnemyStateType.Boom);
             return;
        }
        if(parameter.getHit)
        {
            manager.TranstionState(E_EnemyStateType.Hit);//受到攻击，切换到受到攻击状态
        }
        //播放弹跳的动画
        AnimatorStateInfo animatorInfo;
        animatorInfo = parameter.animator.GetCurrentAnimatorStateInfo(0);
        if(animatorInfo.normalizedTime > 0.99f && animatorInfo.IsName("Find")&&Time.time > nextStateTime)
        {
            manager.TranstionState(E_EnemyStateType.Chase);
        }
    }
}


public class DBoomEnemy_ChaseState : IState
{
    private DashBoomEnemyFSM manager;
    private DashBoomEnemyParameter parameter;
    // Vector3 faceVector;//朝向玩家的向量
    // private float timer = 1.5f;
    // private float nextStateTime;
    public DBoomEnemy_ChaseState(DashBoomEnemyFSM _manager)
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
        //新的追踪逻辑
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
        //原本的冲刺逻辑
        // if(parameter.isDash)
        // {
        //     // Debug.Log("冲刺了一次");
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
        //     manager.TranstionState(E_EnemyStateType.Dizzy);//撞到墙体进入眩晕状态
        //     return;
        // }
        // if(manager.rb.velocity.magnitude < 1.5f&&Time.time > nextStateTime)//速度小于一个阈值,进入Find状态
        // {   
        //     manager.rb.velocity = new Vector3(0f,0f,0f);
        //     manager.TranstionState(E_EnemyStateType.Find);
        // }
    }
}

//眩晕状态要做的只是单纯停留一段时间
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
        // Debug.Log("进入了眩晕状态");
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
            manager.TranstionState(E_EnemyStateType.Hit);//受到攻击，切换到受到攻击状态
        }
        if(Time.time > nextStateTime)
        {
            manager.TranstionState(E_EnemyStateType.Idle);//这里先让它回到待机状态
        }
    }
}


public class  DBoomEnemy_GetHitState: IState
{
    private DashBoomEnemyFSM manager;
    private DashBoomEnemyParameter parameter;
    
    private float timeBtwState = 0.3f;//在受击状态停留至少0.5s
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
        // Debug.Log("进入了受击状态");
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
        //  Debug.Log("进入Boom");
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


public class DBoomEnemy_BoomState : IState
{
    private DashBoomEnemyFSM manager;
    private DashBoomEnemyParameter parameter;
    private float timer = 1f;
    private float nextStateTime;
    private Vector3 nowPos;//暂存现在的位置
    public DBoomEnemy_BoomState(DashBoomEnemyFSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }

    public void OnEnter()
    {
        // Debug.Log("进入了爆炸状态");
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

        //在这里做自爆的伤害判定
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

