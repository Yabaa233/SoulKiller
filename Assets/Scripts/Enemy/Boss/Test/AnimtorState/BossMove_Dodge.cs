using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMove_Dodge : StateMachineBehaviour
{
    private BossControl boss;
    [Header("スプリントアタックNode")]
    public ComboNode comboNode;
    [Header("手を挙げる動作の割合")]
    public float speedUpPer = 0.4f;
    [Header("スプリントアニメーションの手を挙げる再生速度を期待しています")]
    public float animSpeed = 1.5f;
    [Header("開始停止パーセンテージ")]
    public float stopPer = 0.75f;
    [Header("スプリント力")]
    public float dodgePower = 2.0f;
    [Header("停止力")]
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
            boss.UpdatePlayerPosition();    //キャラクターの位置座標を更新する
            animator.speed = animSpeed / stateInfo.speed;
            boss.StopMove(stopPower);
        }
        else if (stateInfo.normalizedTime > speedUpPer && stateInfo.normalizedTime < stopPer)  //持続的なスプリント
        {
            animator.speed = stateInfo.speed;
            boss.DodgeToPlayer(dodgePower);
        }
        else if (stateInfo.normalizedTime > stopPer)     //スプリントが終了し、停止します。
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
