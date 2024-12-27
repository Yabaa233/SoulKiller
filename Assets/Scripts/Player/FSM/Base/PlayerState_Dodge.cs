using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Dodge : StateMachineBehaviour
{
    private PlayerControl currentPlayer; //当前角色
    [Header("可以衔接攻击的百分比")] public float attackPercent = 0.5f;  //动画播放结束的百分比
    [Header("可以移动的百分比")] public float movePercent = 0.95f;  //动画播放结束的百分比
    [Tooltip("冲刺力度")] public float dodgePower = 80f;    //冲刺力度
    [Range(0.0f, 10f)]
    [Tooltip("此值越大制动速度越快")] public float dodgeStopLerpValue = 0.0075f; //用于冲刺制动插值
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (currentPlayer == null)
        {
            currentPlayer = animator.gameObject.GetComponent<PlayerControl>();
        }
        //开冲
        currentPlayer.PlayerBaseMove_Dodge(dodgePower);
        currentPlayer.CloseTrigger();   //关闭攻击触发器
        //闪避时禁止运动
        animator.SetBool("canMove", false);
        //闪避时禁止攻击
        animator.SetBool("canAttack", false);
        //闪避时禁止再次闪避
        animator.SetBool("canDodge", false);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        currentPlayer.PlayerStopMove(dodgeStopLerpValue);   //闪避制动
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
            //恢复各状态
            animator.SetBool("canAttack", true);
        }
        if (stateInfo.normalizedTime > movePercent) //留一点硬直时间
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
