using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class GuidePanelClip : PlayableAsset
{
    [Header("对应的图片")]public Sprite guideImage;
    [TextArea(8,1)] public string guideLineText;
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<GuidePanelBehavior>.Create(graph);
        var controlBehaviour = playable.GetBehaviour();

        controlBehaviour.guideImage = guideImage;
        controlBehaviour.guideLineText = guideLineText;
        
        return playable;
    }
}
