using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

//棋子特效切片
[System.Serializable]
public class PieceClip : PlayableAsset
{
    //公开引用
    [Header("需要操作的棋子")] public ExposedReference<GameObject> gameObject;
    [Header("是否需要反向播放")] public bool isRetryPlay;//是否是反播
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<PieceBehavior>.Create(graph);
        var controlBehaviour = playable.GetBehaviour();

        controlBehaviour.gameObject = gameObject.Resolve(graph.GetResolver());
        controlBehaviour.isRetryPlay = isRetryPlay;
        
        return playable;
    }
}
