using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeavyAttack : StateMachineBehaviour
{
    private PlayerControl currentPlayer;    //現在のキャラクター
    public ComboNode comboNode; //キャラクターのコンボノード
    [Range(0.0f, 10.0f)]
    [Tooltip("この値が大きいほど、制動速度は速くなります。")] public float attackStopLerpValue = 0.005f; //攻撃制動補間用
    public bool dodged = false;    //重撃ダッシュ状態かどうか
    public float dodgePower = 50f;
    public float stopTime = 1.0f;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (currentPlayer == null)
        {
            currentPlayer = animator.gameObject.GetComponent<PlayerControl>();
        }
        animator.SetBool("canMove", false);
        animator.SetBool("canDodge", false);
        currentPlayer.ChangeCombo(comboNode);    //プレイヤーのComboNodeを切り替えます
        currentPlayer.CloseTrigger();   //攻撃トリガーを閉じる
        currentPlayer.PlayerBaseRotate_Attack();   //攻撃で方向を変える
        currentPlayer.PlayerAttackMove_Plunge();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        currentPlayer.PlayerStopMove(attackStopLerpValue);  //ブレーキ
        currentPlayer.GetPlayerInput_MouseRotate(); //方向を変える
        if (!dodged && Input.GetKey(KeyCode.Space))
        {
            currentPlayer.PlayerBaseMove_Dodge(dodgePower);
            dodged = true;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("canMove", true);
        animator.SetBool("canDodge", true);
        dodged = false;
    }
}
