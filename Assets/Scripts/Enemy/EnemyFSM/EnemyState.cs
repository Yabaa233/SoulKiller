using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


//ここでは、ステートインターフェースを継承し、全てのステートスキームを実装します。
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
    private EnemyFSM manager;//ステートマシン
    private EnemyParameter parameter;//設定された属性

    public Enemy_IdleState(EnemyFSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }

    public void OnEnter()
    {
        //メソッドに入る
        parameter.animator.Play("Idle");
    }

    public void OnExit()
    {
        //退出方法
    }

    public void OnLateUpDade()
    {
        //フレームが終了した後に何をすべきか教えてください。
    }

    public void OnUpDate()
    {
        // Debug.Log("敵は待機状態にいます。");
        if(parameter.isDead)
        {
            manager.TranstionState(E_EnemyStateType.Dead);
            return;
        }
        if(parameter.getHit)
        {
            manager.TranstionState(E_EnemyStateType.Hit);//攻撃を受け、攻撃受け状態に切り替えます。
            return;
        }

        if(parameter.ableChase)
        {
            manager.TranstionState(E_EnemyStateType.Find);//追跡状態を開始します
        }
    }

}


public class Enemy_FindState : IState
{
    private EnemyFSM manager;//ステートマシン
    private EnemyParameter parameter;//設定された属性

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
            manager.TranstionState(E_EnemyStateType.Hit);//攻撃を受け、攻撃受け状態に切り替えます。
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
                // Debug.Log("充電状態に入ります");
                manager.TranstionState(E_EnemyStateType.Storage);//プレイヤーが攻撃範囲内にいて、自分自身が画面範囲内にいる場合、蓄積状態になります。
            }
            else{
                // Debug.Log("追跡状態に入ります");
                manager.TranstionState(E_EnemyStateType.Chase);//プレイヤーが攻撃範囲内にいない場合、追撃状態になります。
            }
        }
    }
}


public class Enemy_ChaseState : IState
{
    private EnemyFSM manager;//ステートマシン
    private EnemyParameter parameter;//設定された属性
    private NavMeshAgent agent;//ナビゲーションコンポーネント
    public Enemy_ChaseState(EnemyFSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }

    public void OnEnter()
    {
        //メソッドに入る
        agent = parameter.agent;
        agent.enabled = true;
        parameter.animator.Play("Chase");
    }

    public void OnExit()
    {
        //退出方法
        agent.enabled = false;//stopはもう古いです
    }

    public void OnLateUpDade()
    {
        //フレームが終了した後に何をすべきか教えてください。
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
            // Debug.Log("充電状態に入ります");
            manager.TranstionState(E_EnemyStateType.Storage);
        }
    }

}

public class Enemy_StorageState : IState
{
    private EnemyFSM manager;//ステートマシン
    private EnemyParameter parameter;//設定された属性

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
    private EnemyFSM manager;//ステートマシン
    private EnemyParameter parameter;//設定された属性
    private float btwTime = 2f;
    private float nextStateTime;
    private Vector3 originalPos;//一時保存
    private Vector3 attackPos;//攻撃距離
    private Vector3 faceVector;//スプリントの方向に
    public Enemy_AttackState(EnemyFSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }

    public void OnEnter()
    {
        //メソッドに入る
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
        //フレームが終了した後に何をすべきか教えてください。
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
            // Debug.Log("一度ダッシュしました");
            manager.FaceToTarget();
            manager.rb.AddForce(faceVector.normalized * parameter.enemyStateData.dashPower, ForceMode.Impulse);
            parameter.isDash = false;
        }
        if(manager.rb.velocity.magnitude < 1.5f&&Time.time > nextStateTime)//速度が閾値以下になると、Find状態に移行します。
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
        //自分自身の一部のコンポーネントを破壊するために
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
    private float timeBtwState = 0.5f;//少なくとも0.5秒間、打たれた状態に留まる

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


