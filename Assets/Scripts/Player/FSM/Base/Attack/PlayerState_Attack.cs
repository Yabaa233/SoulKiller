using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤー攻撃状態
/// </summary>
public class PlayerState_Attack : StateMachineBehaviour
{
    private PlayerControl currentPlayer;    //現在のキャラクター
    public ComboNode curComboNode;    //現在の攻撃のコンボ
    private bool nextCombo; //次の攻撃アニメーションに入ったかどうか
    private float forceProgress;  //アニメーション強制再生比率
    [Range(0.0f, 10.0f)]
    [Tooltip("この値が大きいほど制動速度が速い")] public float attackStopLerpValue = 0.005f; //攻撃制動補間用
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (currentPlayer == null)
        {
            currentPlayer = animator.gameObject.GetComponent<PlayerControl>();
        }
        animator.SetBool("changeCombo", false);  //コンボ検出、攻撃ボタンの入力が必要でコンボ状態を維持
        animator.SetBool("canAttack", false);   //攻撃と移動を禁止
        animator.SetBool("attack", false);  //現在の攻撃をキャンセル
        animator.SetBool("canMove", false);
        currentPlayer.ChangeCombo(curComboNode);    //PlayerのComboNodeを切り替え
        currentPlayer.PlayerAttackMove_Plunge();    //自動索敵
        currentPlayer.CloseTrigger();   //攻撃トリガーを閉じる
        forceProgress = curComboNode.forceAnimProgress; //硬直効果
        currentPlayer.SetUseMouseScale(true); //マウス入力で向きを制御、移動時の向き変更を無効化
        currentPlayer.PlayerBaseRotate_Attack();   //攻撃で向きを変更
        currentPlayer.SetUseMouseScale(false); //キーボード入力で向きを制御、攻撃時の向き変更を無効化
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        currentPlayer.PlayerStopMove(attackStopLerpValue);  //制動
        currentPlayer.GetPlayerInput_MouseRotate(); //向きを変更
        if (stateInfo.normalizedTime > forceProgress)
        {
            animator.SetBool("canAttack", true);   //攻撃を許可
            if (animator.GetBool("attack")) //攻撃許可条件下で攻撃入力があれば、コンボを発動
            {
                animator.SetBool("changeCombo", true);  //コンボ検出成功、次のコンボ段階に移行
                nextCombo = true;   //次の攻撃アニメーションに移行予定
            }
        }
        if (!nextCombo && stateInfo.normalizedTime > 0.99f)
        {
            animator.SetBool("changeCombo", false);
            animator.SetBool("canMove", true);  //最後のフレーム終了後しばらくしてから移動可能
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