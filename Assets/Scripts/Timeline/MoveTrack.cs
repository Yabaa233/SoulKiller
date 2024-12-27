using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;


//自定义移动轨道
[TrackColor(238/255f,255/255f,27/255f)]//轨道颜色
[TrackClipType(typeof(MoveClip))]//选择可以放置到轨道上的类型
public class MoveTrack : TrackAsset
{

}
