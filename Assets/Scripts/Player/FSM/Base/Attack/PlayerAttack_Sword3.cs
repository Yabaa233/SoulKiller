using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack_Sword3 : StateMachineBehaviour
{
    private PlayerControl currentPlayer;    //当前角色
    [Header("冲刺开始的时间点")]
    public float dodgeStartPer = 0.4f;
    [Header("冲刺速度")]
    public float dodgeSpeed = 1.0f;
    private bool dodged = false;
    private Vector3 resDir = new Vector3();
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (currentPlayer == null)
        {
            currentPlayer = animator.gameObject.GetComponent<PlayerControl>();
        }
        dodged = false;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!dodged && stateInfo.normalizedTime > dodgeStartPer)
        {
            dodged = true;
            resDir = (currentPlayer.targetPoint - currentPlayer.transform.position).normalized * dodgeSpeed;
            currentPlayer.PlayerAttackMove_Plunge();    //重新设置朝向
            currentPlayer.OpenTrigger();
            currentPlayer.CreateEffect();
            currentPlayer.PlayerForceMove(resDir);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
}
