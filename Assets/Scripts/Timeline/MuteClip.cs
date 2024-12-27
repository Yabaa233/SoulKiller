using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class MuteClip : PlayableAsset
{
    [Header("屏蔽玩家移动")] public ExposedReference<GameObject> muteObject;
    [Header("屏蔽玩家的剑")] public ExposedReference<GameObject> swordObject;
    [Header("屏蔽Boss")]public bool isNeedMuteBoss;
    [Header("屏蔽音频")]public bool isNeedMuteSound;
    [Header("激活大门")]public bool activeDoor;
    [Header("激活SkipButton")]public bool activeSkipButton;
    [Header("激活玩家操作")]public bool isNeedActivePlayer;
    [Header("是否需要玩家溶解")]public bool isNeedPlayerDisslove;
    [Header("传入玩家物体")]public ExposedReference<GameObject> player;
    [Header("是否需要屏蔽主UI")]public bool isNeedMuteMainUI;
    [Header("是否需要打开FullPane")]public bool isNeedOpenFullPanel;
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<MuteBehavior>.Create(graph);
        var controlBehaviour = playable.GetBehaviour();

        controlBehaviour.muteObject = muteObject.Resolve(graph.GetResolver());
        controlBehaviour.swordObject = swordObject.Resolve(graph.GetResolver());
        controlBehaviour.isNeedMuteBoss = isNeedMuteBoss;
        controlBehaviour.isNeedMuteSound = isNeedMuteSound;
        controlBehaviour.activeDoor = activeDoor;
        controlBehaviour.activeSkipButton = activeSkipButton;
        controlBehaviour.isNeedActivePlayer = isNeedActivePlayer;
        controlBehaviour.isNeedPlayerDisslove = isNeedPlayerDisslove;
        controlBehaviour.player =  player.Resolve(graph.GetResolver());
        controlBehaviour.isNeedMuteMainUI = isNeedMuteMainUI;
        controlBehaviour.isNeedOpenFullPanel = isNeedOpenFullPanel;

        return playable;
    }
}
