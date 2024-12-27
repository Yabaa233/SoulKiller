using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Idle : StateMachineBehaviour
{
    public PlayerControl currentPlayer;    //当前角色
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (currentPlayer == null)
        {
            currentPlayer = animator.gameObject.GetComponent<PlayerControl>();
        }
        Debug.Log("OnStateEnter Idle");
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("OnExit Idle");
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Debug.Log("OnUpDate Idle");
        currentPlayer.GetPlayerInput_MouseRotate();    //光圈旋转
    }
}
