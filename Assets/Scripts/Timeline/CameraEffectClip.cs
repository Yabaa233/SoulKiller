using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


[System.Serializable]
public class CameraEffectClip : PlayableAsset
{
    [Header("ColorAdjustingが必要ですか？")] public bool colorAdjusting;
    [Header("開始色")]public float startPostExposure;
    [Header("終点の色")]public float endPostExposure;
    [Header("端の部分を暗くする必要がありますか？")]public bool vignette;
    [Header("開始強度")]public float startIntensity;
    [Header("ゴール強度")]public float endIntensity;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<CameraEffectBehavior>.Create(graph);
        var controlBehaviour = playable.GetBehaviour();

        controlBehaviour.colorAdjusting = colorAdjusting;
        controlBehaviour.startPostExposure = startPostExposure;
        controlBehaviour.endPostExposure = endPostExposure;
        controlBehaviour.vignette = vignette;
        controlBehaviour.startIntensity = startIntensity;
        controlBehaviour.endIntensity = endIntensity;

        return playable;
    }
}
