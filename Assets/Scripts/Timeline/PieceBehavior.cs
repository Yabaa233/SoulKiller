using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
public class PieceBehavior : PlayableBehaviour
{
    private PlayableDirector playableDirector;//获取Timeline对象上的导演组件


    //棋子轨道切片需要的属性
    [Header("需要操作的棋子")] public GameObject gameObject;

    private Material pieceMaterial;
    public bool isRetryPlay;//是否是反播
    private bool isClipPlayed;
    private float startFloat;
    private float endFloat;

    public override void OnPlayableCreate(Playable playable)
    {
       playableDirector = playable.GetGraph().GetResolver() as PlayableDirector;//需要解析之后进行类型转换，有点类似让粒子系统可以变成一个可挂载的脚本
    }

    //类似MonoBehavior中的Update方法，每一帧都会进行调用
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if(isClipPlayed == false && info.weight > 0) //如果当前片段还没有播放，说明需要初始化  TO总感觉可以放到其他地方做初始化
        {
            pieceMaterial = gameObject.GetComponent<Renderer>().material;
            if(pieceMaterial == null)
            {
                Debug.Log("没有东西");
            }
            startFloat = 0;
            endFloat = 60;
            if(isRetryPlay)
            {
                pieceMaterial.SetFloat("_AbsorbRadius",endFloat);
                gameObject.SetActive(true);
            }
            isClipPlayed = true;
        }
        float x = (float)playable.GetDuration();
        if(x < 0.001f)
        {
            Debug.LogWarning("不能除这么小的数");
            x = 1;
        }
        float percent = (float)playable.GetTime()/x;
        if(isRetryPlay)
        {
            percent = 1 - percent;
        }
        float curfloat = (endFloat - startFloat) * percent;
        pieceMaterial.SetFloat("_AbsorbRadius",curfloat);
    }
}
