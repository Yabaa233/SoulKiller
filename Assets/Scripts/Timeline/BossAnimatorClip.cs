using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class BossAnimatorClip : PlayableAsset
{
    public E_BossClipState bossClipState;
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<BossAnimatorBehavior>.Create(graph);
        var controlBehaviour = playable.GetBehaviour();

        controlBehaviour.bossClipState = bossClipState;

        return playable;
    }
}

//TimeLine控制的Boss状态
public enum E_BossClipState
{
    Boss_Dodge,//Boss冲刺
}
