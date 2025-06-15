using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーが攻撃を受けている状態
/// </summary>
public class PlayerState_Hurt : StateMachineBehaviour
{
    [Range(0.0f, 1.0f)]
    [Tooltip("硬直時間 硬直時間が大きいほど長い")] public float forceProgress = 0.95f;
    [Range(0.0f, 10.0f)]
    [Tooltip("被ダメージ時のブレーキ力 この値が大きいほど、ブレーキの効果が早くなります")] public float hurtStopLerpValue = 5.0f; //ダメージ制御補間用
    private PlayerControl currentPlayer; //現在のキャラクター
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
