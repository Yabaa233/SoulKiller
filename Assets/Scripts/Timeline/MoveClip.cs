using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

/// 移动轨道切片
[System.Serializable]
public class MoveClip : PlayableAsset
{
    //公开引用
    [Header("需要移动的物体")]public ExposedReference<GameObject> moveObject;//移动的物体
    [Header("物体开始移动的位置")]public ExposedReference<Transform> startPos;//开始位置
    [Header("物体结束移动的位置")]public ExposedReference<Transform> endPos;//结束位置
    [Header("是否是玩家")] public bool isPlayer;
    [Header("是否是Boss")] public bool isBoss;
    [Header("开启浮动")]public bool startFloat;
    [Header("浮动范围")]public float floatRange;
    [Header("浮动频率")]public float HZ;
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
