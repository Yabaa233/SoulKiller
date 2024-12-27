using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMove_Dodge : StateMachineBehaviour
{
    private BossControl boss;
    [Header("冲刺攻击Node")]
    public ComboNode comboNode;
    [Header("抬手动作占比")]
    public float speedUpPer = 0.4f;
    [Header("期望冲刺动画的抬手播放速度")]
    public float animSpeed = 1.5f;
    [Header("开始停止百分比")]
    public float stopPer = 0.75f;
    [Header("冲刺力度")]
    public float dodgePower = 2.0f;
    [Header("停止力度")]
    public float stopPower = 2.0f;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (boss == null)
        {
            boss = animator.GetComponent<BossControl>();
        }
        boss.comboNode = comboNode;
        animator.ResetTrigger("dodge");
        // animator.SetBool("dodgeing", true);
        animator.SetBool("move", false);
        FMODUnity.RuntimeManager.PlayOneShot("event:/BOSS/dash");
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime < speedUpPer)
        {
            boss.UpdatePlayerPosition();    //更新角色位置坐标
            animator.speed = animSpeed / stateInfo.speed;
            boss.StopMove(stopPower);
        }
        else if (stateInfo.normalizedTime > speedUpPer && stateInfo.normalizedTime < stopPer)  //持续冲刺
        {
            animator.speed = stateInfo.speed;
            boss.DodgeToPlayer(dodgePower);
        }
        else if (stateInfo.normalizedTime > stopPer)     //冲刺结束,停止
        {
            boss.StopMove(stopPower);
        }
        if (stateInfo.normalizedTime > 0.99f)       //
        {
            animator.speed = 1f;
            animator.ResetTrigger("dodge");
            animator.SetBool("dodgeing", false);
            boss.bossCD.canDodge.flag = false;
        }
    }
}
