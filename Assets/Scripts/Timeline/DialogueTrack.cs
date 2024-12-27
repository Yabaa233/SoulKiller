using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

//自定义对话轨道
// [TrackBindingType()]//可以通过这个绑定到需要放上轨道上的类型
[TrackColor(255/255f,20/255f,147/255f)]//轨道颜色
[TrackClipType(typeof(DialogueClip))]//选择可以放置到轨道上的类型
public class DialogueTrack : TrackAsset
{
    
}
