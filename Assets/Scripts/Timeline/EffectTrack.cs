using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

//自定义效果轨道
[TrackColor(72/255f,61/255f,139/255f)]//轨道颜色
[TrackClipType(typeof(EffectClip))]//选择可以放置到轨道上的类型
public class EffectTrack : TrackAsset
{
    
}
