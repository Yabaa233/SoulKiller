using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤー回避状態
/// </summary>
public class PlayerState_Dodge : StateMachineBehaviour
{
    private PlayerControl currentPlayer; //現在のキャラクター
    [Header("攻撃に繋げられるパーセンテージ")] public float attackPercent = 0.5f;  //アニメーション終了のパーセンテージ
    [Header("移動可能なパーセンテージ")] public float movePercent = 0.95f;  //アニメーション終了のパーセンテージ
    [Tooltip("ダッシュ力")] public float dodgePower = 80f;    //ダッシュ力
    [Range(0.0f, 10f)]
    [Tooltip("この値が大きいほど制動速度が速い")] public float dodgeStopLerpValue = 0.0075f; //ダッシュ制動補間用
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (currentPlayer == null)
        {
            currentPlayer = animator.gameObject.GetComponent<PlayerControl>();
        }
        //ダッシュ開始
        currentPlayer.PlayerBaseMove_Dodge(dodgePower);
        currentPlayer.CloseTrigger();   //攻撃トリガーを閉じる
        //ダッシュ中は移動禁止
        animator.SetBool("canMove", false);
        //ダッシュ中は攻撃禁止
        animator.SetBool("canAttack", false);
        //ダッシュ中は再度ダッシュ禁止
        animator.SetBool("canDodge", false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        currentPlayer.PlayerStopMove(dodgeStopLerpValue);   //ダッシュ制動
        currentPlayer.GetPlayerInput_MouseRotate();
        // if ( && Input.GetKeyDown(KeyCode.Space))
        // {
        //     Debug.Log("DodgeAgain");
        //     currentPlayer.dodgeCount --;
        //     currentPlayer.dodgeCD.curTime = 0;
        //     currentPlayer.dodgeCD.flag = false;
        //     animator.SetBool("canDodge", true);
        //     animator.SetTrigger("dodge");
        // }
        if (stateInfo.normalizedTime > attackPercent)
        {
            //各状態を回復
            animator.SetBool("canAttack", true);
        }
        if (stateInfo.normalizedTime > movePercent) //少し硬直時間を残す
        {
            animator.SetBool("canDodge", true);
            animator.SetBool("canMove", true);
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("canDodge", true);
    }
}
