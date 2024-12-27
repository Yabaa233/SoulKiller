using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMove_Flash : StateMachineBehaviour
{
    private BossControl boss;
    [Header("抬手动作占比")]
    public float speedUpPer = 0.2f;
    [Header("实际期望冲刺播放速度")]
    public float animSpeed = 1.5f;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("闪现");
        if (boss == null)
        {
            boss = animator.GetComponent<BossControl>();
        }
        animator.ResetTrigger("flash");
        // animator.SetBool("flashing", true);
        animator.SetBool("move", false);
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss.StopMove(3);
        if (stateInfo.normalizedTime > 1)
        {
            animator.ResetTrigger("flash");
            animator.SetBool("flashing", false);
            boss.bossCD.canFlash.flag = false;
        }
    }
}
