using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


[System.Serializable]
public class EffectBehavior : PlayableBehaviour
{
    private PlayableDirector playableDirector;//Timelineオブジェクト上のディレクターコンポーネントを取得します。

    //特殊効果トラックスライスに必要な属性
    [Header("特殊効果のプレハブ")]public GameObject effectObject;
    // [Header("目指す目標")]public Transform target;
    [Header("特殊効果が生み出す円心")]public Transform effectCenter;
    // [Header("エフェクトが生成される半径")]public float radius;
    // [Header("エフェクトの生成数")]public int effectNum;
    // [Header("散逸段階の時間")]public float runTime;
    // [Header("浮遊ステージの時間")]public float floatTime;

    private bool isClipPlayed;
    private bool effectPlayed;
    private List<GameObject> effectList = new List<GameObject>();
    private List<Vector3> randomList = new List<Vector3>();

    public override void OnPlayableCreate(Playable playable)
    {
        playableDirector = playable.GetGraph().GetResolver() as PlayableDirector;//解析後に型変換を行う必要があり、それは粒子システムをマウント可能なスクリプトに変えることに少し似ています。
        // GenerateEffect();
    }


    //MonoBehaviorのUpdateメソッドのように、各フレームで呼び出されます。
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if(isClipPlayed == false && info.weight > 0)
        {
            GenerateEffect();
            isClipPlayed = true;
            effectPlayed = true;
        }
        // //比率を計算する
        // float x = (float)playable.GetDuration();
        // if(x < 0.001f)
        // {
        //     Debug.LogWarning("これ以上小さい数で割ることはできません。");
        //     x = 1;
        // }
        // float percent = (float)playable.GetTime()/x;

        
        // if(percent < runTime)//前の1/3が各位置に散らばる
        // {
        //     if(effectPlayed)//一度だけ揺れとブルームを再生する
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
        //     //何もせずにただホバーするだけ
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
        //         if(effect == null)//これはすでに破壊されていることを示しています。パフォーマンスを節約するために、Removeを先に行わないでください。
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
