using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack_Normal : StateMachineBehaviour
{
    public BossControl boss;
    [Header("抬手动作占比")]
    public float speedUpPer = 0.5f;
    [Header("期望抬手动画播放速度")]
    public float animSpeed = 0.02f;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss = animator.GetComponent<BossControl>();
        // animator.SetBool("attacking", true);
        animator.ResetTrigger("normalAttack");
        //FMODUnity.RuntimeManager.PlayOneShot("event:/BOSS/吟唱蓄力");
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime < speedUpPer)
        {
            animator.speed = animSpeed  / stateInfo.speed;
        }
        else
        {
            animator.speed = stateInfo.speed;
        }

    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.speed = 1f;
        animator.SetBool("attacking", false);
        boss.bossCD.canNormalAttack.flag = false;
    }
}
