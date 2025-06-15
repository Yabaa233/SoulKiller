using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMove_BackFlash : StateMachineBehaviour
{
    private BossControl boss;
    [Header("手を挙げる動作の割合")]
    public float speedUpPer = 0.2f;
    [Header("実際に期待されるスプリントの再生速度")]
    public float animSpeed = 1.5f;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("バックフラッシュ");
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
