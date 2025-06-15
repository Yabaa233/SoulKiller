using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

/// 移動軌道スライス
[System.Serializable]
public class MoveClip : PlayableAsset
{
    //公開引用
    [Header("移動が必要な物体")]public ExposedReference<GameObject> moveObject;//動く物体
    [Header("物体が移動を開始する位置")]public ExposedReference<Transform> startPos;//開始位置
    [Header("物体が移動を終える位置")]public ExposedReference<Transform> endPos;//終了位置
    [Header("プレイヤーですか？")] public bool isPlayer;
    [Header("ボスですか？")] public bool isBoss;
    [Header("フロートを開始する")]public bool startFloat;
    [Header("浮動範囲")]public float floatRange;
    [Header("フローティングレート")]public float HZ;
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<MoveClipBehavior>.Create(graph);
        var controlBehaviour = playable.GetBehaviour();

        controlBehaviour.moveObject = moveObject.Resolve(graph.GetResolver());
        controlBehaviour.startPos = startPos.Resolve(graph.GetResolver());
        controlBehaviour.endPos = endPos.Resolve(graph.GetResolver());

        controlBehaviour.isPlayer = isPlayer;
        controlBehaviour.startFloat = startFloat;
        controlBehaviour.floatRange = floatRange;
        controlBehaviour.HZ = HZ;
        controlBehaviour.isBoss = isBoss;

        return playable;
    }
}
