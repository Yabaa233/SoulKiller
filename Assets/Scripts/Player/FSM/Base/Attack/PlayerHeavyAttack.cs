using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeavyAttack : StateMachineBehaviour
{
    private PlayerControl currentPlayer;    //当前角色
    public ComboNode comboNode; //角色连招节点
    [Range(0.0f, 10.0f)]
    [Tooltip("此值越大制动速度越快")] public float attackStopLerpValue = 0.005f; //用于攻击制动插值
    public bool dodged = false;    //是否处于重击冲刺状态
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
        currentPlayer.ChangeCombo(comboNode);    //切换Player的ComboNode
        currentPlayer.CloseTrigger();   //关闭攻击触发器
        currentPlayer.PlayerBaseRotate_Attack();   //攻击改变朝向
        currentPlayer.PlayerAttackMove_Plunge();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        currentPlayer.PlayerStopMove(attackStopLerpValue);  //制动
        currentPlayer.GetPlayerInput_MouseRotate(); //转向
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
