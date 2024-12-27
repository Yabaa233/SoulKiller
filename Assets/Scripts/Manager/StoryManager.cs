using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using System;

public class StoryManager : singleton<StoryManager>
{
    private List<Action> actions;//将剧情里面的每一步都存成一个函数或者类，通过索引对应的去调用
    private int _currentStep;//当前剧情阶段

    //当前剧情播放的状态
    public enum StoryMode
    {
        GamePlay,
        DialogueMoment,
    }
    public StoryMode storyMode;
    private PlayableDirector currentPlayableDirector;

    /// 缓存区
    public GameObject dialoguePanel;//缓存对话面板对象
    public DialoguePanel dialoguePanelScript;//缓存面板对象的脚本

    protected override void Awake() {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);  
    }

    /// <summary>
    /// 将剧情回退至
    /// </summary>
    /// <param name="_step"></param>
    public void SetStoryTo(int _step)
    {
        _currentStep = _step;
        if(_currentStep < 0)
        {
            _currentStep = 0;
        }
        else if(_currentStep > actions.Count - 1)
        {
            Debug.LogWarning("注意剧情步数超过限制");
            _currentStep = actions.Count - 1;
        }
    }
    
    private void Start() {
        dialoguePanel = PanelManager.Instance.CreateDialoguePanel();
        dialoguePanelScript = dialoguePanel.GetComponent<DialoguePanel>();
        dialoguePanelScript.SetContinueTipsVis(false);
        dialoguePanel.SetActive(false);

        //注册剧情
        // actions.Add(StartStory);
    }

    private void Update() {
        if(storyMode == StoryMode.DialogueMoment)
        {
            if(Input.GetKeyDown(KeyCode.C))
            {
                ResumeTimeLine();
            }
        }
    }

    public void PauseTimeLine(PlayableDirector _playableDirector)
    {
        currentPlayableDirector = _playableDirector;
        storyMode = StoryMode.DialogueMoment;
        currentPlayableDirector.playableGraph.GetRootPlayable(0).SetSpeed(0d);//设置暂停
        // currentPlayableDirector.Pause();
    }

    public void ResumeTimeLine()
    {
        storyMode = StoryMode.GamePlay;
        currentPlayableDirector.playableGraph.GetRootPlayable(0).SetSpeed(1d);//回复播放
        // currentPlayableDirector.Resume();

        dialoguePanelScript.SetContinueTipsVis(false);
        dialoguePanel.SetActive(true);
    }



///
/// ///////////////所有剧本
/// 
    /// <summary>
    /// 第一幕
    /// </summary>
    public void StartStory()
    {

    }
}
