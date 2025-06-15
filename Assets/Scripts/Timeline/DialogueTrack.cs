using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

//カスタムダイアログトラック
// [TrackBindingType()]//このタイプをトラックにバインドすることができます
[TrackColor(255/255f,20/255f,147/255f)]//軌道の色
[TrackClipType(typeof(DialogueClip))]//軌道上に配置できるタイプを選択してください。
public class DialogueTrack : TrackAsset
{
    
}
