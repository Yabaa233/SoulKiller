using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMove_Flash : StateMachineBehaviour
{
    private BossControl boss;
    [Header("手を挙げる動作の割合")]
    public float speedUpPer = 0.2f;
    [Header("実際に期待されるスプリントの再生速度")]
    public float animSpeed = 1.5f;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("フラッシュ");
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
