using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

[System.Serializable]
public class GuidePanelBehavior : PlayableBehaviour
{
    private PlayableDirector playableDirector;//Timelineオブジェクト上のディレクターコンポーネントを取得します。

    [Header("対応する画像")]public Sprite guideImage;
    [TextArea(8,1)] public string guideLineText;
    
    private GuidePanel guidePanel;
    private bool isClipPlayed;

    public override void OnPlayableCreate(Playable playable)
    {
        playableDirector = playable.GetGraph().GetResolver() as PlayableDirector;//解析後に型変換を行う必要があり、それは粒子システムをマウント可能なスクリプトに変えることに少し似ています。
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if(isClipPlayed == false && info.weight > 0)
        {
            Debug.Log("作成中です");
            PanelManager.Instance.Open(new GuidePanel());
            isClipPlayed = true;
        }
    }
}
