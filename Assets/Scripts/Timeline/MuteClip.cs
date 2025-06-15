using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class MuteClip : PlayableAsset
{
    [Header("プレイヤーの移動をブロックする")] public ExposedReference<GameObject> muteObject;
    [Header("プレイヤーの剣をブロックする")] public ExposedReference<GameObject> swordObject;
    [Header("ボスをブロックする")]public bool isNeedMuteBoss;
    [Header("オーディオをブロックする")]public bool isNeedMuteSound;
    [Header("ゲートをアクティベートする")]public bool activeDoor;
    [Header("SkipButtonをアクティブにする")]public bool activeSkipButton;
    [Header("プレイヤー操作を活性化する")]public bool isNeedActivePlayer;
    [Header("プレイヤーが溶解する必要がありますか？")]public bool isNeedPlayerDisslove;
    [Header("プレイヤーオブジェクトを渡す")]public ExposedReference<GameObject> player;
    [Header("メインUIをブロックする必要がありますか？")]public bool isNeedMuteMainUI;
    [Header("FullPaneを開く必要がありますか？")]public bool isNeedOpenFullPanel;
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
