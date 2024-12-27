using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

[System.Serializable]
public class GuidePanelBehavior : PlayableBehaviour
{
    private PlayableDirector playableDirector;//获取Timeline对象上的导演组件

    [Header("对应的图片")]public Sprite guideImage;
    [TextArea(8,1)] public string guideLineText;
    
    private GuidePanel guidePanel;
    private bool isClipPlayed;

    public override void OnPlayableCreate(Playable playable)
    {
        playableDirector = playable.GetGraph().GetResolver() as PlayableDirector;//需要解析之后进行类型转换，有点类似让粒子系统可以变成一个可挂载的脚本
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if(isClipPlayed == false && info.weight > 0)
        {
            Debug.Log("正在创建");
            PanelManager.Instance.Open(new GuidePanel());
            isClipPlayed = true;
        }
    }
}
