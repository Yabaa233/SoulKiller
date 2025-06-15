using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーの移動状態
/// </summary>
public class PlayerState_Move : StateMachineBehaviour
{
    private PlayerControl currentPlayer;    //現在のキャラクター
    [Range(0.0f, 10.0f)]
    [Tooltip("この値が大きいほど、速度の上昇が早くなります。")] public float velocityLerpValue = 0.02f; //速度補間用
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
        currentPlayer.PlayerBaseMove(velocityLerpValue); //移動
        currentPlayer.PlayerBaseRotate_Move();
        currentPlayer.GetPlayerInput_MouseRotate();    //オーラの回転
    }
}
