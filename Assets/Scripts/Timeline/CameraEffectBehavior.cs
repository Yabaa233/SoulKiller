using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class CameraEffectBehavior : PlayableBehaviour
{
    private PlayableDirector playableDirector;//Timelineオブジェクト上のディレクターコンポーネントを取得します。
    //カメラトラックスライスに必要な属性
    [Header("ColorAdjustingが必要ですか？")] public bool colorAdjusting;
    [Header("開始色")]public float startPostExposure;
    [Header("終点の色")]public float endPostExposure;
    [Header("端の部分を暗くする必要がありますか？")]public bool vignette;
    [Header("開始強度")]public float startIntensity;
    [Header("ゴール強度")]public float endIntensity;


    private bool isClipPlayed;

    public override void OnPlayableCreate(Playable playable)
    {
        playableDirector = playable.GetGraph().GetResolver() as PlayableDirector;//解析後に型変換を行う必要があり、それは粒子システムをマウント可能なスクリプトに変えることに少し似ています。
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
            Debug.LogWarning("これほど小さい数を除算することはできません。");
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
