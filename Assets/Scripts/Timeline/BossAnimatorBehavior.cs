using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class BossAnimatorBehavior : PlayableBehaviour
{
    private PlayableDirector playableDirector;//获取Timeline对象上的导演组件
    public E_BossClipState bossClipState;
    private bool isClipPlayed;

    public override void OnPlayableCreate(Playable playable)
    {
        playableDirector = playable.GetGraph().GetResolver() as PlayableDirector;//需要解析之后进行类型转换，有点类似让粒子系统可以变成一个可挂载的脚本
    }

    //类似Mono中的Update方法，每一帧都会进行调用
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



    /////所有的调用方法
    public void Boss_Dodge()
    {

    }
}
