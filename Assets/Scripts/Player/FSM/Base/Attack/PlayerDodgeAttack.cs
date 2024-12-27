using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDodgeAttack : StateMachineBehaviour
{
    private PlayerControl currentPlayer;    //当前角色
    public ComboNode curComboNode;    //当前攻击的连招
    [Header("冲刺开始的时间点")]
    public float dodgeStartPer = 0.4f;
    [Header("冲刺速度")]
    public float dodgeSpeed = 1.0f;
    private bool dodged = false;
    private Vector3 resDir = new Vector3();
    [Header("可以移动的百分比")] public float movePercent = 0.95f;  //动画播放结束的百分比
    [Header("可以衔接攻击的百分比")] public float attackPercent = 0.5f;  //动画播放结束的百分比

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (currentPlayer == null)
        {
            currentPlayer = animator.gameObject.GetComponent<PlayerControl>();
        }
        currentPlayer.PlayerAttackMove_Plunge();
        currentPlayer.ChangeCombo(curComboNode);    //切换Player的ComboNode
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
        if (stateInfo.normalizedTime > attackPercent)
        {
            //恢复各状态
            animator.SetBool("canAttack", true);
        }
        if (stateInfo.normalizedTime > movePercent) //留一点硬直时间
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
