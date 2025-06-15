using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using BehaviorDesigner;

[System.Serializable]
public class MoveClipBehavior : PlayableBehaviour
{   
    private PlayableDirector playableDirector;//Timelineオブジェクト上のディレクターコンポーネントを取得します。

    //移動軌道スライスに必要な属性
    [Header("移動が必要な物体")]public GameObject moveObject;//動く物体
    [Header("物体が移動を開始する位置")]public Transform startPos;//開始位置
    [Header("物体が移動を終える位置")]public Transform endPos;//終了位置
    [Header("プレイヤーですか？")] public bool isPlayer;
    [Header("ボスですか？")] public bool isBoss;
    [Header("フロートを開始する")]public bool startFloat;
    [Header("浮動範囲")]public float floatRange;
    [Header("フロート速度")]public float HZ;

    private bool isClipPlayed;

    private Vector3 startVector;
    private Vector3 endVector;
    private float floatY;//補助浮動小数点

    public override void OnPlayableCreate(Playable playable)
    {
        playableDirector = playable.GetGraph().GetResolver() as PlayableDirector;//解析後に型変換を行う必要があり、それは粒子システムをマウント可能なスクリプトに変えることに少し似ています。
    }

    //MonoBehaviorのUpdateメソッドのように、各フレームで呼び出されます。
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if(isClipPlayed == false && info.weight > 0) //現在のフラグメントがまだ再生されていない場合、初期化が必要であることを示します。TOは他の場所で初期化を行うことが可能だと感じています。
        {
            if(isBoss)
            {
                moveObject = GameManager.Instance.currentBoss.gameObject;
                moveObject.GetComponent<BossControl>().BehaviorTree.DisableBehavior();//ボス自身の行動ツリーをキャンセルする
            }
            else if(isPlayer)
            {
                moveObject = GameManager.Instance.currentPlayer.gameObject;
            }
            moveObject.transform.position = startPos.position;
            startVector = startPos.position;
            endVector = endPos.position;
            floatY = startVector.y;

            isClipPlayed = true;
        }
        float x = (float)playable.GetDuration();
        if(x < 0.001f)
        {
            Debug.LogWarning("これほど小さい数を除算することはできません。");
            x = 1;
        }
        //フローティング
        if(startFloat)//浮遊している間は一時的に移動できません。
        {
            float floatbias= Mathf.Sin(Time.time * Mathf.PI * HZ) * floatRange + floatY;
            moveObject.transform.position = new Vector3(moveObject.transform.position.x,floatbias,moveObject.transform.position.z);
            return;
        }
        //移動
        float percent = (float)playable.GetTime()/x;
        moveObject.transform.position = Vector3.Lerp(startVector,endVector,percent);
    }

    public override void OnPlayableDestroy(Playable playable)
    {
        if(moveObject != null)
        {
            // moveObject.GetComponent<BossControl>().enabled = true;
        }
    }
}
