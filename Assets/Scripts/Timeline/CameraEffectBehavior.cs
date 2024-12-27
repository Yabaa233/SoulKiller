using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class CameraEffectBehavior : PlayableBehaviour
{
    private PlayableDirector playableDirector;//获取Timeline对象上的导演组件
    //相机轨道切片需要的属性
    [Header("是否需要ColorAdjusting")] public bool colorAdjusting;
    [Header("起始颜色")]public float startPostExposure;
    [Header("终点颜色")]public float endPostExposure;
    [Header("是否需要边角压暗")]public bool vignette;
    [Header("起始强度")]public float startIntensity;
    [Header("终点强度")]public float endIntensity;


    private bool isClipPlayed;

    public override void OnPlayableCreate(Playable playable)
    {
        playableDirector = playable.GetGraph().GetResolver() as PlayableDirector;//需要解析之后进行类型转换，有点类似让粒子系统可以变成一个可挂载的脚本
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if(isClipPlayed == false && info.weight > 0)
        {
            isClipPlayed = true;
        }

        float x = (float)playable.GetDuration();
        if(x < 0.001f)
        {
            Debug.LogWarning("不能除这么小的数");
            x = 1;
        }
        float percent = (float)playable.GetTime()/x;

        if(colorAdjusting)
        {
            float colorPercent = startPostExposure + (endPostExposure - startPostExposure)*percent;
            CM_Effect.Instance.SetColorAdjusting(colorPercent);
        }

        if(vignette)
        {
            float intensity = startIntensity + (endIntensity - startIntensity)*percent;
            CM_Effect.Instance.SetVignette(intensity);
        }
    }
}
