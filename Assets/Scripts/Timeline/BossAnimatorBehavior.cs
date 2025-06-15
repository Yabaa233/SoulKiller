using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class BossAnimatorBehavior : PlayableBehaviour
{
    private PlayableDirector playableDirector;//Timelineオブジェクト上のディレクターコンポーネントを取得します。
    public E_BossClipState bossClipState;
    private bool isClipPlayed;

    public override void OnPlayableCreate(Playable playable)
    {
        playableDirector = playable.GetGraph().GetResolver() as PlayableDirector;//解析後に型変換を行う必要があり、それは粒子システムをマウント可能なスクリプトに変えることに少し似ています。
    }

    //MonoのUpdateメソッドのように、各フレームで呼び出されます。
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if(isClipPlayed == false && info.weight > 0)
        {
            isClipPlayed = true;

            switch(bossClipState)
            {
                case E_BossClipState.Boss_Dodge:Boss_Dodge();break;
            }
        }
    }



    /////すべての呼び出し方法
    public void Boss_Dodge()
    {

    }
}
