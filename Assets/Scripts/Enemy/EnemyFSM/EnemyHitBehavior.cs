using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyHitBehavior : StateMachineBehaviour
{
    public float startTime = 0.3f;//卡肉开始时间
    public BaseEnemyControl keeper;//当前怪物的引用
    public bool isHitStop = true;//是否停顿
    private float storageSpeed;//原本的时间
    private CDClass HitTime = new CDClass();

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       isHitStop = true;
       keeper = animator.gameObject.GetComponent<BaseEnemyControl>();
       HitTime.maxCDTime = keeper.enemyData.currentStopTime;
       HitTime.flag = false;
       GameManager.Instance.CDList.Add(HitTime);
       storageSpeed = animator.speed;//记录原本的播放速度
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       if(stateInfo.normalizedTime > startTime && isHitStop)
       {
            animator.speed = 0f;
            isHitStop = false;
       }
       if(HitTime.flag)
       {
            animator.speed = storageSpeed;
       }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       animator.speed = storageSpeed;
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
