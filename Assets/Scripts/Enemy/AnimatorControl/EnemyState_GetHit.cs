using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵の被撃状態
/// </summary>
public class EnemyState_GetHit : StateMachineBehaviour
{
    [Header("卡肉暂停开始时间")]
    public float pauseTime = 0.2f;
    private BaseEnemyControl baseEnemyControl;
    private float stopTime;
    private float curTime;
    private bool paused;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    /// <summary>
    /// 状態の初期化
    /// </summary>
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (baseEnemyControl == null)
        {
            baseEnemyControl = animator.GetComponent<BaseEnemyControl>();
            stopTime = baseEnemyControl.enemyData.currentStopTime;
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    /// <summary>
    /// 状態の更新
    /// </summary>
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!paused && stateInfo.normalizedTime > pauseTime)
        {
            paused = true;
            animator.speed = 0.0f;
        }
        if (paused)
        {
            curTime += Time.deltaTime;
            if (curTime > stopTime)
            {
                animator.speed = 1;
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    /// <summary>
    /// 状態の終了
    /// </summary>
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        curTime = 0;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
