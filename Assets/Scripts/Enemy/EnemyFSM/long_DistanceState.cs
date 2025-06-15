using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



///<summary>




////// 長距離の敵の状態




///</summary>
public class long_DistanceState : IState
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


public class longEnemy_IdleState : IState
{
    private long_DistanceFSM manager;
    private longEnemyParameter parameter;

    public longEnemy_IdleState(long_DistanceFSM _manager)
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

    public void OnUpDate()
    {
        // Debug.Log("敵は待機状態にいます。");
        if(parameter.isDead == true)
        {
            manager.TranstionState(E_EnemyStateType.Dead);
            return;
        }
        if(parameter.getHit)
        {
            manager.TranstionState(E_EnemyStateType.Hit);//攻撃を受けて状態を切り替えます
        }
        if(parameter.ableAttact)
        {
            if(parameter.enemyType == E_EnemyType.BOOM)
            {
                manager.TranstionState(E_EnemyStateType.Storage);
            }
            else
            {
                manager.TranstionState(E_EnemyStateType.Chase);//警戒距離に達したら追跡を開始します。
            }
        }
    }

    public void OnLateUpDade()
    {
        
    }

    public void OnExit()
    {
        
    }
}


public class longEnemy_ChaseState : IState
{
    private long_DistanceFSM manager;
    private longEnemyParameter parameter;
    public longEnemy_ChaseState(long_DistanceFSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }

    public void OnEnter()
    {
        // Debug.Log("追跡状態に入りました");
        parameter.agent.enabled = true;
        parameter.animator.Play("Chase");
    }

    public void OnExit()
    {
        
    }

    public void OnLateUpDade()
    {
        
    }

    public void OnUpDate()
    {
        if(parameter.isDead == true)
        {
            manager.TranstionState(E_EnemyStateType.Dead);
            return;
        }
        if(parameter.getHit)
        {
            manager.TranstionState(E_EnemyStateType.Hit);//攻撃を受け、攻撃受け状態に切り替えます。
            return;
        }
        parameter.agent.speed = parameter.moveSpeed;
        if(parameter.agent.isOnNavMesh)
        {
            parameter.agent.SetDestination(parameter.target.position);
        }
        if(Vector3.Distance(parameter.target.position,parameter.enemyPos.position) < parameter.enemyStateData.attackDistance)
        {
            // Debug.Log("攻撃状態に入ります");
            manager.TranstionState(E_EnemyStateType.Shot);

        }
        if(parameter.ableAttact == false)
        {
            manager.TranstionState(E_EnemyStateType.Idle);
        }
    }
}


public class longEnemy_ShootState :IState
{
    private long_DistanceFSM manager;
    private longEnemyParameter parameter;
    private float nextAttackTime;//次の攻撃イベントを一時保存する
    private float timeBtwAttck;//攻撃速度によって決定される間隔
    private bool isAttack;//攻撃しますか？

    public longEnemy_ShootState(long_DistanceFSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;

        //コンストラクタ内で値を割り当てることはできません。なぜなら、インスペクタパネルの内容はコンストラクタの時に割り当てられるからです。
    }

    public void OnEnter()
    {
        // Debug.Log("攻撃状態に入りました");
        parameter.agent.enabled = false;//攻撃状態に入る前に、パスファインドコンポーネントをオフにしてください。
        isAttack = true;
        //いくつかの属性の割り当て
        timeBtwAttck = parameter.enemyStateData.attackSpeed;
        //FmodManager.Instance.PlaySoundOnce(parameter.enemyStateData.attackSound);
        FMODUnity.RuntimeManager.PlayOneShot(parameter.enemyStateData.attackSound);
    }

    public void OnExit()
    {
        parameter.agent.enabled = true;
    }

    public void OnLateUpDade()
    {

    }

    public void OnUpDate()
    {
        if(parameter.isDead == true)
        {
            manager.TranstionState(E_EnemyStateType.Dead);
            return;
        }
        if(parameter.getHit == true)
        {
            manager.TranstionState(E_EnemyStateType.Hit);
            return;
        }
        manager.FaceToTarget();
        if(Time.time > nextAttackTime && isAttack)
        {
            // Debug.Log("一度攻撃する");
            parameter.animator.Play("Attack");
            manager.Shot(parameter.enemyType);
            nextAttackTime = Time.time + timeBtwAttck;
            isAttack = false;
            if(parameter.enemyType == E_EnemyType.BOOM)
            {
                manager.BoomCD.flag = false;
                manager.TranstionState(E_EnemyStateType.Storage);
            }
        }
        if(parameter.ableAttact == false)
        {
            manager.TranstionState(E_EnemyStateType.Idle);
            return;
        }
        if(Vector3.Distance(parameter.target.position,parameter.enemyPos.position) > parameter.enemyStateData.attackDistance)
        {
            if(parameter.enemyType == E_EnemyType.BOOM)
            {
                manager.TranstionState(E_EnemyStateType.Storage);
                return;
            }
            else
            {
                manager.TranstionState(E_EnemyStateType.Chase);
            }
        }

        manager.animatorInfo = parameter.animator.GetCurrentAnimatorStateInfo(0);
        if(manager.animatorInfo.normalizedTime > 0.99f && manager.animatorInfo.IsName("Attack") && (parameter.enemyType != E_EnemyType.BOOM))
        {
            manager.TranstionState(E_EnemyStateType.MoveAfter);
            return;
        }
    }
}



public class longEnemy_GetHitState :IState
{
    private long_DistanceFSM manager;
    private longEnemyParameter parameter;

    private float nextStateTime;
    private float timeBtwState = 0.5f;//少なくとも被打撃状態に留まる
    public longEnemy_GetHitState(long_DistanceFSM _manager)
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
        //FMODUnity.RuntimeManager.PlayOneShot(parameter.enemyData.getHitSound);
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
        if(parameter.isDead)
        {
            manager.TranstionState(E_EnemyStateType.Dead);
            return;
        }
        if(parameter.enemyData.currentHealth <=0)
        {
            manager.TranstionState(E_EnemyStateType.Dead);
            return;
        }
        if(Time.time - nextStateTime>timeBtwState)
        {
            if(parameter.enemyType == E_EnemyType.BOOM)
            {
                manager.TranstionState(E_EnemyStateType.Storage);
            }
            else
            {
                manager.TranstionState(E_EnemyStateType.Chase);
            }
        }
    }
}



public class longEnemy_DeadState :IState
{
    private long_DistanceFSM manager;
    private longEnemyParameter parameter;
    public longEnemy_DeadState(long_DistanceFSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }

    public void OnEnter()
    {
        parameter.isDead = true;
        parameter.animator.Play("Dead");
        GameManager.Instance.CDList.Remove(manager.BoomCD);
        GameManager.Instance.CDList.Remove(manager.jumpCD);
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


/////////追加の状態
public class longEnemy_StorageState : IState
{
    private long_DistanceFSM manager;
    private longEnemyParameter parameter;
    private float jumpTime = 2f;//時間を飛び越える
    private float nextStateTime;//次の状態の時間を大まかに計算する
    public longEnemy_StorageState(long_DistanceFSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }

    public void OnEnter()
    {
        nextStateTime = Time.time + jumpTime;
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
            manager.TranstionState(E_EnemyStateType.Hit);
            return;
        }
        if(manager.BoomCD.flag)
        {
            manager.TranstionState(E_EnemyStateType.Shot);
        }
        else if(manager.jumpCD.flag)
        {
            manager.TranstionState(E_EnemyStateType.Jump);
        }
    }
}
public class longEnemy_JumpState : IState
{
    private long_DistanceFSM manager;
    private longEnemyParameter parameter;
    //次のジャンプポイント
    private Vector3 nextPoint;
    private Vector3 sourcePoint;//原始点
    private float jumpTime = 0.5f;//時間を飛び越える
    private float nextStateTime;//次の状態の時間を大まかに計算する
    private float distance;//移動距離を一時保存
    private float speed;//一時保存ジャンプ速度
    private float StartTime;//開始時間
    private Vector3 faceVector;//二点間の単位ベクトル
    public longEnemy_JumpState(long_DistanceFSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }

    public void OnEnter()
    {
        //元のデータを保存する
        sourcePoint = parameter.enemyPos.position;
        nextStateTime = Time.time + jumpTime;
        //次のランダムポイント領域をサンプリングします
        float range = 20f;
        Vector3 randomPoint;
        if(RandomPoint(parameter.target.position,range,out randomPoint))
        {
            nextPoint = randomPoint;
            // Debug.Log("サンプリングが成功しました");
        }
        else
        {
            nextPoint = parameter.enemyPos.localPosition;
            // Debug.Log("サンプリングに失敗しました");
        }
        // Debug.Log( "サンプリング：次のポイントは" + nextPoint + "  プレイヤーポイント：" + parameter.target.position);
        nextPoint = parameter.enemyPos.parent.TransformPoint(nextPoint);
        distance = Vector3.Distance(sourcePoint,nextPoint);//距離を計算する
        speed = distance / jumpTime;//計算速度
        faceVector = Vector3.Normalize(nextPoint - sourcePoint);
        // Debug.Log(nextPoint + "   sourcepoint" + sourcePoint);
        StartTime = Time.time;
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
            parameter.outPos.position = sourcePoint;
            manager.TranstionState(E_EnemyStateType.Hit);
            return;
        }
        float disCovered = (Time.time - StartTime) * speed;
        float fracJourney = disCovered/distance;
        if(distance >= 0.1f)
        {
            parameter.outPos.position = Vector3.Lerp(sourcePoint,nextPoint,fracJourney);
        }
        if(Time.time > nextStateTime)
        {
            //FmodManager.Instance.PlaySoundOnce(parameter.enemyStateData.shiftSound);
            FMODUnity.RuntimeManager.PlayOneShot(parameter.enemyStateData.shiftSound);
            parameter.enemyPos.position = nextPoint;
            parameter.outPos.position = nextPoint;
            manager.jumpCD.flag = false;
            // Debug.Log(parameter.enemyPos.position);
            manager.TranstionState(E_EnemyStateType.Storage);
            return;
        }
    }


    bool RandomPoint(Vector3 center,float range,out Vector3 result)
    {
        Vector3 randomPoint = new Vector3(center.x + Random.insideUnitCircle.x * range, parameter.enemyPos.position.y, center.z + Random.insideUnitCircle.y * range);
        randomPoint.y = 0;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(parameter.enemyPos.parent.InverseTransformPoint(randomPoint), out hit, 5f, NavMesh.AllAreas)) {
            result = hit.position;
            // Debug.Log(result);
            return true;
        }
        result = new Vector3(5f,0f,5f);
		return false;
    }
}


public class longEnemy_MoveAfterState : IState
{
    private long_DistanceFSM manager;
    private longEnemyParameter parameter;
    private Vector3 backPos;
    private Vector3 backVector;
    private Vector3 sourcePos;
    private float moveDistance = 5f;//移動距離
    private float startTime;
    private float distance;//移動距離を一時保存
    private float moveTime = 1f;//ポジショニングタイム
    private float speed;
    private float nextStateTime;
    public longEnemy_MoveAfterState(long_DistanceFSM _manager)
    {
        this.manager = _manager;
        this.parameter = _manager.parameter;
    }

    public void OnEnter()
    {
        parameter.animator.Play("Chase");
        sourcePos = parameter.enemyPos.position;
        distance = Vector3.Distance(parameter.target.position,parameter.enemyPos.position);
        backVector = Vector3.Normalize(parameter.target.position - parameter.enemyPos.position);
        backVector.y = 0;
        backPos = parameter.enemyPos.position - backVector * moveDistance;
        startTime = Time.time;
        speed = distance/moveTime;
        nextStateTime = Time.time + moveTime;
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
            manager.TranstionState(E_EnemyStateType.Hit);
        }
        float disCovered = (Time.time - startTime) * speed;
        float fracJourney = disCovered/distance;
        if(distance >= 0.1f)
        {
            parameter.enemyPos.position = Vector3.Lerp(sourcePos,backPos,fracJourney);

        }
        if(Vector3.Distance(parameter.target.position,parameter.enemyPos.position) < 1f || Time.time > nextStateTime )
        {
            manager.TranstionState(E_EnemyStateType.Chase);
        }
    }
}
