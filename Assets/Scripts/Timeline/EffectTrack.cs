using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

//カスタムエフェクトトラック
[TrackColor(72/255f,61/255f,139/255f)]//軌道の色
[TrackClipType(typeof(EffectClip))]//軌道上に配置できるタイプを選択してください。
public class EffectTrack : TrackAsset
{
    
}
