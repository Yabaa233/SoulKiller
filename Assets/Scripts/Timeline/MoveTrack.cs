using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;


//カスタム移動軌道
[TrackColor(238/255f,255/255f,27/255f)]//軌道の色
[TrackClipType(typeof(MoveClip))]//軌道上に配置できるタイプを選択してください。
public class MoveTrack : TrackAsset
{

}
