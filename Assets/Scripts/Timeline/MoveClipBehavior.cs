using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using BehaviorDesigner;

[System.Serializable]
public class MoveClipBehavior : PlayableBehaviour
{   
    private PlayableDirector playableDirector;//获取Timeline对象上的导演组件

    //移动轨道切片需要的属性
    [Header("需要移动的物体")]public GameObject moveObject;//移动的物体
    [Header("物体开始移动的位置")]public Transform startPos;//开始位置
    [Header("物体结束移动的位置")]public Transform endPos;//结束位置
    [Header("是否是玩家")] public bool isPlayer;
    [Header("是否是Boss")] public bool isBoss;
    [Header("开启浮动")]public bool startFloat;
    [Header("浮动范围")]public float floatRange;
    [Header("浮动速度")]public float HZ;

    private bool isClipPlayed;

    private Vector3 startVector;
    private Vector3 endVector;
    private float floatY;//辅助浮动的点

    public override void OnPlayableCreate(Playable playable)
    {
        playableDirector = playable.GetGraph().GetResolver() as PlayableDirector;//需要解析之后进行类型转换，有点类似让粒子系统可以变成一个可挂载的脚本
    }

    //类似MonoBehavior中的Update方法，每一帧都会进行调用
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if(isClipPlayed == false && info.weight > 0) //如果当前片段还没有播放，说明需要初始化  TO总感觉可以放到其他地方做初始化
        {
            if(isBoss)
            {
                moveObject = GameManager.Instance.currentBoss.gameObject;
                moveObject.GetComponent<BossControl>().BehaviorTree.DisableBehavior();//取消掉Boss自己的行为树
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
            Debug.LogWarning("不能除这么小的数");
            x = 1;
        }
        //浮动
        if(startFloat)//浮动的时候暂时不能移动
        {
            float floatbias= Mathf.Sin(Time.time * Mathf.PI * HZ) * floatRange + floatY;
            moveObject.transform.position = new Vector3(moveObject.transform.position.x,floatbias,moveObject.transform.position.z);
            return;
        }
        //移动
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
