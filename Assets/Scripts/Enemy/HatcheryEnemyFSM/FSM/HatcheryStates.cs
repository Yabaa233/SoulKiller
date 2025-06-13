using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatcheryStates : IState
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
public class HatcheryEnemy_IdleState : IState
{
    private HatcheryEnemyFSM manager;
    private HatcheryEnemyParameter parameter;
    public HatcheryEnemy_IdleState(HatcheryEnemyFSM _manager)
    {
        manager = _manager;
        parameter = _manager.parameter;
    }
    public void OnEnter()
    {
        parameter.animator.Play("Idle");
        /*parameter.intervalTime = Time.time;*/
        ///待機状態、第一状態として設定:1.待機 2.逃走
    }
    public void OnUpDate()
    {
        if(parameter.isDead)
        {
            manager.TranstionState(E_EnemyStateType.Dead);
            return;
        }
        if (parameter.getHit)
        {
            manager.TranstionState(E_EnemyStateType.Hit);
            return;
        }
        if (manager.hatcheryEnemyCD.flag&&parameter.nowSonCount<=parameter.enemyStateData.sonMaxCount)
        {
            manager.hatcheryEnemyCD.flag = false;
            manager.TranstionState(E_EnemyStateType.Attack);
        }
    }
    public void OnLateUpDade()
    {
        ///最初のフレーム実行
    }
    public void OnExit()
    {
        ///状態終了
    }
}
/// <summary>
/// 敵の状態、攻撃を受けると逃走状態に切り替わる
/// </summary>
public class HatcheryEnemy_ProductionState : IState
{
    private HatcheryEnemyFSM manager;
    private HatcheryEnemyParameter parameter;
    private float beginProductionTime;///生産開始時間
    private float needTime = 2f;///生産プロセスの時間、変更可能
    public HatcheryEnemy_ProductionState(HatcheryEnemyFSM _manager)
    {
        manager = _manager;
        parameter = _manager.parameter;
    }
    public void OnEnter()
    {
        parameter.animator.Play("Attack");
        beginProductionTime = Time.time;///生産開始時間を記録
        
    }
    public void OnUpDate()
    {
        if (parameter.getHit)///攻撃を受けると逃走状態に切り替わる
        {
            manager.TranstionState(E_EnemyStateType.Hit);
            return;
        }
        if (Time.time >= needTime + beginProductionTime)
        {
            
            ///1体の敵を生成
            GameObject son = manager.SonProduction(parameter.sonBorn);///生成位置は事前に設定された位置を使用        
            /*FMODUnity.RuntimeManager.PlayOneShot("event:/Monster/NorMal/duiduiBorn");                                                                        ///敵を生成した後に敵の音を再生*/
            son.transform.parent = manager.transform.parent;
            manager.gameObject.GetComponent<HatcheryEnemyControl>().room.enemyCount++;
            son.SetActive(true);
            parameter.nowSonCount++;
            manager.TranstionState(E_EnemyStateType.Idle);
        }
    }
    public void OnLateUpDade()
    {
        ///最初のフレーム実行
    }
    public void OnExit()
    {
        ///生産が終了したらIdle状態に戻る
    }

}
public class HatcheryEnemy_HitState : IState
{
    private HatcheryEnemyFSM manager;
    private HatcheryEnemyParameter parameter;
    private float nextTime;
    private float timeBtwState = 0.5f;///硬直停止時間
    public HatcheryEnemy_HitState(HatcheryEnemyFSM _manager)
    {
        manager = _manager;
        parameter = _manager.parameter;
    }
    public void OnEnter()
    {
        parameter.animator.Play("Hit");
        nextTime = Time.time;
        ///ダメージ処理は既にcontrolスクリプトに移動したため省略、parameter.enemyData.currentHealth -= 1;///この部分はplayerのAttack値に置き換える必要がある
    }
    public void OnUpDate()
    {
        if(parameter.isDead)
        {
            manager.TranstionState(E_EnemyStateType.Dead);
            return;
        }
        if (parameter.enemyData.currentHealth <= 0)
        {
            manager.TranstionState(E_EnemyStateType.Dead);
            return;
        }
        else
        {
            if (Time.time >= timeBtwState + nextTime)
            {
                manager.TranstionState(E_EnemyStateType.Idle);
            }
        }
    }
    public void OnLateUpDade()
    {
        //最初のフレーム実行
    }
    public void OnExit()
    {
        parameter.getHit = false;
    }
}
public class HatcheryEnemy_DeadState : IState
{
    private HatcheryEnemyFSM manager;
    private HatcheryEnemyParameter parameter;
    public HatcheryEnemy_DeadState(HatcheryEnemyFSM _manager)
    {
        manager = _manager;
        parameter = _manager.parameter;
    }
    //TODO:母体の死亡処理
    public void OnEnter()
    {
        parameter.isDead = true;
        parameter.animator.Play("Death");
        GameManager.Instance.CDList.Remove(manager.hatcheryEnemyCD);
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
        if (animatorInfo.normalizedTime > 0.99f && animatorInfo.IsName("Death"))
        {
            GameObject.Destroy(parameter.body.parent.gameObject);
        }
    }
}