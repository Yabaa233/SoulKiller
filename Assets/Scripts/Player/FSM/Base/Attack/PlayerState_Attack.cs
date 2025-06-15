using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーの攻撃状態
/// </summary>
public class PlayerState_Attack : StateMachineBehaviour
{
    private PlayerControl currentPlayer;    //現在のキャラクター
    public ComboNode curComboNode;    //現在の攻撃コンボ
    private bool nextCombo; //次の攻撃アニメーションに入ったかどうか
    private float forceProgress;  //アニメーション強制再生率
    [Range(0.0f, 10.0f)]
    [Tooltip("この値が大きいほど、制動速度は速くなります。")] public float attackStopLerpValue = 0.005f; //攻撃制動補間用
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (currentPlayer == null)
        {
            currentPlayer = animator.gameObject.GetComponent<PlayerControl>();
        }
        animator.SetBool("changeCombo", false);  //コンボの検出、攻撃ボタンの入力が必要でコンボ状態を維持
        animator.SetBool("canAttack", false);   //攻撃と移動を禁止します
        animator.SetBool("attack", false);  //現在の攻撃をキャンセルします
        animator.SetBool("canMove", false);
        currentPlayer.ChangeCombo(curComboNode);    //プレイヤーのComboNodeを切り替えます
        currentPlayer.PlayerAttackMove_Plunge();    //自動探知
        currentPlayer.CloseTrigger();   //攻撃トリガーを閉じる
        forceProgress = curComboNode.forceAnimProgress; //硬直効果
        currentPlayer.SetUseMouseScale(true); //マウス入力で方向を制御し、移動時の方向変更を無効にします。
        currentPlayer.PlayerBaseRotate_Attack();   //攻撃で方向を変える
        currentPlayer.SetUseMouseScale(false); //キーボード入力で方向を制御し、攻撃時の方向変更を無効にします。
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        currentPlayer.PlayerStopMove(attackStopLerpValue);  //ブレーキ
        currentPlayer.GetPlayerInput_MouseRotate(); //方向を変える
        if (stateInfo.normalizedTime > forceProgress)
        {
            animator.SetBool("canAttack", true);   //攻撃を許可する
            if (animator.GetBool("attack")) //攻撃許可条件が満たされている場合、攻撃入力があればコンボを発動します。
            {
                animator.SetBool("changeCombo", true);  //コンボの検出に成功し、次のコンボステージに進みます。
                nextCombo = true;   //次の攻撃アニメーションに移行する予定です。
            }
        }
        if (!nextCombo && stateInfo.normalizedTime > 0.99f)
        {
            animator.SetBool("changeCombo", false);
            animator.SetBool("canMove", true);  //最後のフレームが終了した後しばらくしてから移動が可能になります。
            // currentPlayer.CloseTrigger();   //攻撃トリガーを閉じる
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("changeCombo", false);
        // animator.SetBool("canAttack", true);   //攻撃を許可
        if (!nextCombo)
        {
            currentPlayer.CloseTrigger();   //攻撃トリガーを閉じる
        }
        nextCombo = false;
    }
}