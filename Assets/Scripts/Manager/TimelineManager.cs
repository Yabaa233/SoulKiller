using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineManager : singleton<TimelineManager>
{
    //タイムラインで制御する必要があるオブジェクト
    [Header("タイムラインで開いた扉")] public RoomTrigger door;
    public PlayableDirector currentPlayableDirector;
    public List<PlayableDirector> playableDirectors;
    public int index = 0;//自動再生制御

    protected new void Awake() {//すべてのPlayableDirectorがここに登録されます。
       base.Awake();
       Transform mainTimeLine =  GameObject.Find("MainTimeLine").transform;
       PlayableDirector[] playableArrs = mainTimeLine.GetComponentsInChildren<PlayableDirector>();
       playableDirectors = new List<PlayableDirector>(playableArrs);
       if(playableDirectors.Count == 0)
       {
            Debug.LogWarning("タイムラインディレクタークラスを取得できません");
       }
       else
       {
            currentPlayableDirector = playableDirectors[0];
            index = 0;
       }
    }

    /// <summary>
    /// タイムラインをジャンプさせる
    /// </summary>
    /// <param name="index">インデックス</param>
    public void changePlayableTO(int index)
    {
        if(index >= playableDirectors.Count)
        {
            Debug.LogWarning("インデックスが配列の範囲を超えています");
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

    ///<summary>


    ////// 現在のタイムラインを再生し、再生が終了したら自動的に次に切り替わります。


    ///</summary>
    public void PlayCurrentPlayableDirector()
    {
        if(index >= playableDirectors.Count)
        {
            return;
        }
        
        changePlayableTO(index);
        currentPlayableDirector.Play();
        ///今、それを一時停止させるつもりはありません。
        /// 
        index ++ ;
    }

    ///<summary>


    ////// 現在のTimeLIneを一時停止します


    ///</summary>
    public void PauseCurrentPlayableDirector()
    {
        currentPlayableDirector.Pause();
    }

    ///<summary>


    ////// 現在のタイムラインの再生を続けます


    ///</summary>
    public void ResumeCurrentPlayableDirector()
    {
        currentPlayableDirector.Resume();
    }


    ///<summary>



    ////// 現在のタイムラインの再生を停止します



    ///</summary>
    public void StopCurrentPlayableDirector()
    {
        currentPlayableDirector.Stop();
    }
}
