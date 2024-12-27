using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Hurt : StateMachineBehaviour
{
    [Range(0.0f, 1.0f)]
    [Tooltip("硬直时间 越大硬直时间越长")] public float forceProgress = 0.95f;
    [Range(0.0f, 10.0f)]
    [Tooltip("受击时制动 此值越大制动速度越快")] public float hurtStopLerpValue = 5.0f; //用于受击制动插值
    private PlayerControl currentPlayer; //当前角色
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (currentPlayer == null)
        {
            currentPlayer = animator.gameObject.GetComponent<PlayerControl>();
        }
        animator.SetBool("canMove", false);
        animator.SetBool("canAttack", false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        currentPlayer.PlayerStopMove(hurtStopLerpValue);
        if (stateInfo.normalizedTime > forceProgress)
        {
            animator.SetBool("canMove", true);
            animator.SetBool("canAttack", true);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
}
