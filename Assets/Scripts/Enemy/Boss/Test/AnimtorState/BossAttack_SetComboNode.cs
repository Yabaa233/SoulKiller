using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack_SetComboNode : StateMachineBehaviour
{
    public BossControl boss;
    public ComboNode comboNode;
    public bool SetAttackRangeHint = true;
    [Tooltip("攻撃するプレイヤーの方向を更新しますか？")] public bool SetPlayerPosition = false;
    [Range(0.0f, 1.0f)]
    [Tooltip("更新が必要な場合、いつ更新すべきですか？")] public float setPositionTime;
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
