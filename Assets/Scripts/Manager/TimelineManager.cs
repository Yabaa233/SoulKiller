using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineManager : singleton<TimelineManager>
{
    //需要通过Timeline控制的物体
    [Header("通过Timeline打开的大门")] public RoomTrigger door;
    public PlayableDirector currentPlayableDirector;
    public List<PlayableDirector> playableDirectors;
    public int index = 0;//自动播放控制

    protected new void Awake() {//所有的PlayableDirector都往这里注册
       base.Awake();
       Transform mainTimeLine =  GameObject.Find("MainTimeLine").transform;
       PlayableDirector[] playableArrs = mainTimeLine.GetComponentsInChildren<PlayableDirector>();
       playableDirectors = new List<PlayableDirector>(playableArrs);
       if(playableDirectors.Count == 0)
       {
            Debug.LogWarning("没有timeline导演类可以获取");
       }
       else
       {
            currentPlayableDirector = playableDirectors[0];
            index = 0;
       }
    }

    /// <summary>
    /// 将Timeline跳转至
    /// </summary>
    /// <param name="index">索引</param>
    public void changePlayableTO(int index)
    {
        if(index >= playableDirectors.Count)
        {
            Debug.LogWarning("索引超出数组限制");
            return;
        }
        if(currentPlayableDirector != playableDirectors[index])
        {
            if(currentPlayableDirector == null)
            {
                currentPlayableDirector = playableDirectors[index];
            }
            else
            {
                currentPlayableDirector.Stop();
                currentPlayableDirector = playableDirectors[index];
            }
        }
    }

    /// <summary>
    /// 播放当前的TimeLine，并且播放结束后会自动切换到下一个
    /// </summary>
    public void PlayCurrentPlayableDirector()
    {
        if(index >= playableDirectors.Count)
        {
            return;
        }
        
        changePlayableTO(index);
        currentPlayableDirector.Play();
        ///现在并不会让它暂停
        /// 
        index ++ ;
    }

    /// <summary>
    /// 暂停当前的TimeLIne
    /// </summary>
    public void PauseCurrentPlayableDirector()
    {
        currentPlayableDirector.Pause();
    }

    /// <summary>
    /// 继续播放当前的Timeline
    /// </summary>
    public void ResumeCurrentPlayableDirector()
    {
        currentPlayableDirector.Resume();
    }


    /// <summary>
    /// 停止当前timeline的播放
    /// </summary>
    public void StopCurrentPlayableDirector()
    {
        currentPlayableDirector.Stop();
    }
}
