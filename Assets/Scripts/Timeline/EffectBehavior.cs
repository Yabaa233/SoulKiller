using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


[System.Serializable]
public class EffectBehavior : PlayableBehaviour
{
    private PlayableDirector playableDirector;//获取Timeline对象上的导演组件

    //特效轨道切片需要的属性
    [Header("特效预制体")]public GameObject effectObject;
    // [Header("飞向的目标")]public Transform target;
    [Header("特效产生的圆心")]public Transform effectCenter;
    // [Header("特效产生的半径")]public float radius;
    // [Header("特效产生的个数")]public int effectNum;
    // [Header("散逸阶段时间")]public float runTime;
    // [Header("悬浮阶段时间")]public float floatTime;

    private bool isClipPlayed;
    private bool effectPlayed;
    private List<GameObject> effectList = new List<GameObject>();
    private List<Vector3> randomList = new List<Vector3>();

    public override void OnPlayableCreate(Playable playable)
    {
        playableDirector = playable.GetGraph().GetResolver() as PlayableDirector;//需要解析之后进行类型转换，有点类似让粒子系统可以变成一个可挂载的脚本
        // GenerateEffect();
    }


    //类似MonoBehavior中的Update方法，每一帧都会进行调用
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if(isClipPlayed == false && info.weight > 0)
        {
            GenerateEffect();
            isClipPlayed = true;
            effectPlayed = true;
        }
        // //计算比例
        // float x = (float)playable.GetDuration();
        // if(x < 0.001f)
        // {
        //     Debug.LogWarning("不能除这么小的数");
        //     x = 1;
        // }
        // float percent = (float)playable.GetTime()/x;

        
        // if(percent < runTime)//前1/3散开到各个位置
        // {
        //     if(effectPlayed)//只播放一次抖动和Bloom
        //     {
        //         CM_Effect.Instance.CM_do_shake(shake_type.simple,0.5f,0.5f,0.5f);
        //         CM_Effect.Instance.SetBloomIntensity(2,true,1f);
        //         effectPlayed = false;
        //     }
        //     float realPercent = (float)playable.GetTime()/runTime;
        //     for(int i=0;i<effectList.Count;i++)
        //     {
        //         if(effectList[i] == null)
        //         {
        //             continue;
        //         }
        //         effectList[i].transform.position = Vector3.Lerp(effectCenter.position,randomList[i],realPercent);
        //     }
        // }
        // else if(runTime<percent && percent<floatTime)
        // {
        //     float realPecent = (percent - runTime)/(1 - runTime);
        //     //什么都不做 只是悬停
        //     for(int i=0;i<effectList.Count;i++)
        //     {
        //         if(effectList[i] == null)
        //         {
        //             continue;
        //         }
        //         else
        //         {
        //             effectList[i].transform.position = randomList[i];
        //         }
        //     }
        // }
        // else
        // {
        //     float realPecent = (percent - floatTime) / (1-floatTime);
        //     foreach(var effect in effectList)
        //     {
        //         if(effect == null)//说明这个已经被销毁了，为了节省性能先不进行Remove
        //         {
        //             continue;
        //         }
        //         effect.transform.position = Vector3.Lerp(effect.transform.position,target.position,realPecent);
        //         // Debug.Log(effect.transform.position);
        //         if(Vector3.Distance(effect.transform.position,target.transform.position) < 1f)
        //         {
        //             GameObject.Destroy(effect);
        //         }
        //     }
        // }
    }

    public void GenerateEffect()
    {
        // for(int i=0;i<effectNum;i++)
        // {
        //     Vector3 randomPoint = effectCenter.position + Random.insideUnitSphere *radius;
        //     randomPoint.y = 5f;
        CM_Effect.Instance.CM_do_shake(shake_type.simple,1f,1f,0.5f);
        CM_Effect.Instance.SetBloomIntensity(2,true,1f);
        GameObject effect = GameObject.Instantiate(effectObject,effectCenter.position,effectObject.transform.rotation,effectCenter);
        //     effectList.Add(effect);
        //     randomList.Add(randomPoint);
        // }
    }

}
