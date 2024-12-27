using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 移动状态
/// </summary>
public class PlayerState_Move : StateMachineBehaviour
{
    private PlayerControl currentPlayer;    //当前角色
    [Range(0.0f, 10.0f)]
    [Tooltip("此值越大速度提升越快")] public float velocityLerpValue = 0.02f; //用于速度插值
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (currentPlayer == null)
        {
            currentPlayer = animator.gameObject.GetComponent<PlayerControl>();
        }
        animator.SetBool("canDodge", true);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        currentPlayer.PlayerBaseMove(velocityLerpValue); //移动
        currentPlayer.PlayerBaseRotate_Move();
        currentPlayer.GetPlayerInput_MouseRotate();    //光圈旋转
    }
}
