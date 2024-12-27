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
        ///����״̬,��һ״̬����Ϊ:1.���� 2.�ܻ�
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
        ///��һִ֡��
    }
    public void OnExit()
    {
        ///�˳�״̬
    }
}
/// <summary>
/// ��������״̬���������ܻ�����������������
/// </summary>
public class HatcheryEnemy_ProductionState : IState
{
    private HatcheryEnemyFSM manager;
    private HatcheryEnemyParameter parameter;
    private float beginProductionTime;///��ʼ������ʱ��
    private float needTime = 2f;///�������̵�ʱ�䣬�ɱ����
    public HatcheryEnemy_ProductionState(HatcheryEnemyFSM _manager)
    {
        manager = _manager;
        parameter = _manager.parameter;
    }
    public void OnEnter()
    {
        parameter.animator.Play("Attack");
        beginProductionTime = Time.time;///��¼��ʼ����ʱ��
        
    }
    public void OnUpDate()
    {
        if (parameter.getHit)///���������ܻ��������ᱻ��ϡ�
        {
            manager.TranstionState(E_EnemyStateType.Hit);
            return;
        }
        if (Time.time >= needTime + beginProductionTime)
        {
            
            ///����һ��1������
            GameObject son = manager.SonProduction(parameter.sonBorn);///���ɵ�λ�þ�������֮ǰԤ��׼���õ�����        
            /*FMODUnity.RuntimeManager.PlayOneShot("event:/Monster/NorMal/duiduiBorn");                                                                        ///���Զ����ɵĶ��ӽ��в���*/
            son.transform.parent = manager.transform.parent;
            manager.gameObject.GetComponent<HatcheryEnemyControl>().room.enemyCount++;
            son.SetActive(true);
            parameter.nowSonCount++;
            manager.TranstionState(E_EnemyStateType.Idle);
        }
    }
    public void OnLateUpDade()
    {
        ///��һִ֡��
    }
    public void OnExit()
    {
        ///������˳��������Է���Idle����
    }

}
public class HatcheryEnemy_HitState : IState
{
    private HatcheryEnemyFSM manager;
    private HatcheryEnemyParameter parameter;
    private float nextTime;
    private float timeBtwState = 0.5f;///Ӳֱͣ��ʱ��
    public HatcheryEnemy_HitState(HatcheryEnemyFSM _manager)
    {
        manager = _manager;
        parameter = _manager.parameter;
    }
    public void OnEnter()
    {
        parameter.animator.Play("Hit");
        nextTime = Time.time;
        ///�˺����������Ѿ�Ų����control�ű�������ʡ�ԣ�parameter.enemyData.currentHealth -= 1;///�����������Ӧ�滻Ϊplayer��Attack��ֵ
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
        //��һִ֡��
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
    //TODO:ĸ����������
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