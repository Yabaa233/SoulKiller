using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
public class PieceBehavior : PlayableBehaviour
{
    private PlayableDirector playableDirector;//Timelineオブジェクト上のディレクターコンポーネントを取得します。


    //チェスピースの軌道スライスに必要な属性
    [Header("操作が必要なチェスの駒")] public GameObject gameObject;

    private Material pieceMaterial;
    public bool isRetryPlay;//逆再生ですか？
    private bool isClipPlayed;
    private float startFloat;
    private float endFloat;

    public override void OnPlayableCreate(Playable playable)
    {
       playableDirector = playable.GetGraph().GetResolver() as PlayableDirector;//解析後に型変換を行う必要があり、それは粒子システムをマウント可能なスクリプトに変えることに少し似ています。
    }

    //MonoBehaviorのUpdateメソッドのように、各フレームで呼び出されます。
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if(isClipPlayed == false && info.weight > 0) //現在のフラグメントがまだ再生されていない場合、初期化が必要であることを示します。TOは他の場所で初期化を行うことが可能だと感じています。
        {
            pieceMaterial = gameObject.GetComponent<Renderer>().material;
            if(pieceMaterial == null)
            {
                Debug.Log("何もない");
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
            Debug.LogWarning("これほど小さい数を除算することはできません。");
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
