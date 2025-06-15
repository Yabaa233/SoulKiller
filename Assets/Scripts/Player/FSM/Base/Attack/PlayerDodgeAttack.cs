using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDodgeAttack : StateMachineBehaviour
{
    private PlayerControl currentPlayer;    //現在のキャラクター
    public ComboNode curComboNode;    //現在の攻撃コンボ
    [Header("ステップが開始する時間点")]
    public float dodgeStartPer = 0.4f;
    [Header("ステップの速度")]
    public float dodgeSpeed = 1.0f;
    private bool dodged = false;
    private Vector3 resDir = new Vector3();
    [Header("移動可能なパーセンテージ")] public float movePercent = 0.95f;  //アニメーション終了のパーセンテージ
    [Header("攻撃につながるパーセンテージ")] public float attackPercent = 0.5f;  //アニメーション終了のパーセンテージ

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (currentPlayer == null)
        {
            currentPlayer = animator.gameObject.GetComponent<PlayerControl>();
        }
        currentPlayer.PlayerAttackMove_Plunge();
        currentPlayer.ChangeCombo(curComboNode);    //プレイヤーのComboNodeを切り替えます
        dodged = false;
    }

     override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
     {
        if (!dodged && stateInfo.normalizedTime > dodgeStartPer)
        {
            dodged = true;
            resDir = (currentPlayer.targetPoint - currentPlayer.transform.position).normalized * dodgeSpeed;
            currentPlayer.PlayerAttackMove_Plunge();    //方向を再設定します
            currentPlayer.OpenTrigger();
            currentPlayer.CreateEffect();
            currentPlayer.PlayerForceMove(resDir);
        }
        if (stateInfo.normalizedTime > attackPercent)
        {
            //すべての状態を回復します。
            animator.SetBool("canAttack", true);
        }
        if (stateInfo.normalizedTime > movePercent) //まだ少し硬直時間が残っています
        {
            animator.SetBool("canDodge", true);
            animator.SetBool("canMove", true);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
}
