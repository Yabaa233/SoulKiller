using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack_SetComboNode : StateMachineBehaviour
{
    public BossControl boss;
    public ComboNode comboNode;
    public bool SetAttackRangeHint = true;
    [Tooltip("是否更新攻击玩家的方向")] public bool SetPlayerPosition = false;
    [Range(0.0f, 1.0f)]
    [Tooltip("如果需要更新 那么在什么时间内进行更新")] public float setPositionTime;
    private bool setedAttackRangeHint  =false;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (boss == null)
        {
            boss = animator.GetComponent<BossControl>();
        }
        boss.comboNode = comboNode;
        setedAttackRangeHint = false;
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (SetPlayerPosition && stateInfo.normalizedTime < setPositionTime)
        {
            boss.UpdatePlayerPosition();
        }
        else if (SetPlayerPosition && !setedAttackRangeHint)
        {
            setedAttackRangeHint = true;
            if (SetAttackRangeHint) boss.SetAttackRangeHint();
        }
    }
}
