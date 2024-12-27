using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMove_BackFlash : StateMachineBehaviour
{
    private BossControl boss;
    [Header("抬手动作占比")]
    public float speedUpPer = 0.2f;
    [Header("实际期望冲刺播放速度")]
    public float animSpeed = 1.5f;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("反向闪现");
        if (boss == null)
        {
            boss = animator.GetComponent<BossControl>();
        }
        animator.ResetTrigger("backFlash");
        // animator.SetBool("backFlashing", true);
        animator.SetBool("move", false);
        FMODUnity.RuntimeManager.PlayOneShot("event:/BOSS/shiftOut");
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime > 0.99f)
        {
            animator.ResetTrigger("backFlash");
            animator.SetBool("backFlashing", false);
            boss.bossCD.canBackFlash.flag = false;
        }
    }
}
