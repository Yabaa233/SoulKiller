using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


//粒子效果切片
[System.Serializable]
public class EffectClip : PlayableAsset
{
    [Header("特效预制体")]public ExposedReference<GameObject> effectObject;
    // [Header("飞向的目标")]public ExposedReference<Transform> target;
    [Header("特效产生的圆心")]public ExposedReference<Transform> effectCenter;
    // [Header("特效产生的半径")]public float radius;
    // [Header("特效产生的个数")]public int effectNum;
    // [Header("散逸阶段时间")]public float runTime;
    // [Header("悬浮阶段时间")]public float floatTime;
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<EffectBehavior>.Create(graph);
        var controlBehaviour = playable.GetBehaviour();

        controlBehaviour.effectObject = effectObject.Resolve(graph.GetResolver());
        // controlBehaviour.target = target.Resolve(graph.GetResolver());
        controlBehaviour.effectCenter = effectCenter.Resolve(graph.GetResolver());

        // controlBehaviour.radius = radius;
        // controlBehaviour.effectNum = effectNum;
        // controlBehaviour.runTime = runTime;
        // controlBehaviour.floatTime = floatTime;

        return playable;
    }
}
