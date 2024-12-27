using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Attack : StateMachineBehaviour
{
    private PlayerControl currentPlayer;    //当前角色
    public ComboNode curComboNode;    //当前攻击的连招
    private bool nextCombo; //是否进入了下一段攻击动画
    private float forceProgress;  //动画强制播放比例
    [Range(0.0f, 10.0f)]
    [Tooltip("此值越大制动速度越快")] public float attackStopLerpValue = 0.005f; //用于攻击制动插值
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (currentPlayer == null)
        {
            currentPlayer = animator.gameObject.GetComponent<PlayerControl>();
        }
        animator.SetBool("changeCombo", false);  //连招检测，需要输入攻击按键才能避免退出连招状态
        animator.SetBool("canAttack", false);   //禁止攻击和移动
        animator.SetBool("attack", false);  //取消当前攻击
        animator.SetBool("canMove", false);
        currentPlayer.ChangeCombo(curComboNode);    //切换Player的ComboNode
        currentPlayer.PlayerAttackMove_Plunge();    //自动索敌
        currentPlayer.CloseTrigger();   //关闭攻击触发器
        forceProgress = curComboNode.forceAnimProgress; //硬直效果
        currentPlayer.SetUseMouseScale(true); //使用鼠标输入朝向，关闭移动时的朝向修改
        currentPlayer.PlayerBaseRotate_Attack();   //攻击改变朝向
        currentPlayer.SetUseMouseScale(false); //使用键盘输入朝向，关闭攻击时的朝向修改
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        currentPlayer.PlayerStopMove(attackStopLerpValue);  //制动
        currentPlayer.GetPlayerInput_MouseRotate(); //转向
        if (stateInfo.normalizedTime > forceProgress)
        {
            animator.SetBool("canAttack", true);   //允许攻击
            if (animator.GetBool("attack")) //如果在允许攻击条件下输入攻击，那么触发连击
            {
                animator.SetBool("changeCombo", true);  //连招检测成功，进入下一连招阶段
                nextCombo = true;   //即将进入下一段攻击动画
            }
        }
        if (!nextCombo && stateInfo.normalizedTime > 0.99f)
        {
            animator.SetBool("changeCombo", false);
            animator.SetBool("canMove", true);  //最后一帧结束之后一段时间才可以进行移动
            // currentPlayer.CloseTrigger();   //关闭攻击触发器
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("changeCombo", false);
        // animator.SetBool("canAttack", true);   //允许攻击
        if (!nextCombo)
        {
            currentPlayer.CloseTrigger();   //关闭攻击触发器
        }
        nextCombo = false;
    }
}