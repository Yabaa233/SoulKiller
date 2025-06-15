using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

//チェスピースの特殊効果スライス
[System.Serializable]
public class PieceClip : PlayableAsset
{
    //公開引用
    [Header("操作が必要なチェスの駒")] public ExposedReference<GameObject> gameObject;
    [Header("逆再生が必要ですか？")] public bool isRetryPlay;//逆再生ですか？
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<PieceBehavior>.Create(graph);
        var controlBehaviour = playable.GetBehaviour();

        controlBehaviour.gameObject = gameObject.Resolve(graph.GetResolver());
        controlBehaviour.isRetryPlay = isRetryPlay;
        
        return playable;
    }
}
