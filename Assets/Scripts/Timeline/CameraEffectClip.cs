using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


[System.Serializable]
public class CameraEffectClip : PlayableAsset
{
    [Header("是否需要ColorAdjusting")] public bool colorAdjusting;
    [Header("起始颜色")]public float startPostExposure;
    [Header("终点颜色")]public float endPostExposure;
    [Header("是否需要边角压暗")]public bool vignette;
    [Header("起始强度")]public float startIntensity;
    [Header("终点强度")]public float endIntensity;

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
