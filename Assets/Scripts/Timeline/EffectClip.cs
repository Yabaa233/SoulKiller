using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


//パーティクルエフェクトスライス
[System.Serializable]
public class EffectClip : PlayableAsset
{
    [Header("特殊効果のプレハブ")]public ExposedReference<GameObject> effectObject;
    // [Header("目指す目標")]public ExposedReference<Transform> target;
    [Header("特殊効果が生み出す円心")]public ExposedReference<Transform> effectCenter;
    // [Header("エフェクトが生成される半径")]public float radius;
    // [Header("エフェクトの生成数")]public int effectNum;
    // [Header("散逸段階の時間")]public float runTime;
    // [Header("浮遊ステージの時間")]public float floatTime;
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
