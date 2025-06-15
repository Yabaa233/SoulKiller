using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using System;

public class StoryManager : singleton<StoryManager>
{
    private List<Action> actions;//ストーリーの各ステップを関数またはクラスとして保存し、インデックスに対応するものを呼び出します。
    private int _currentStep;//現在のストーリー段階

    //現在のストーリー再生状況
    public enum StoryMode
    {
        GamePlay,
        DialogueMoment,
    }
    public StoryMode storyMode;
    private PlayableDirector currentPlayableDirector;

    /// バッファエリア
    public GameObject dialoguePanel;//ダイアログパネルオブジェクトをキャッシュする
    public DialoguePanel dialoguePanelScript;//スクリプトのキャッシュパネルオブジェクト

    protected override void Awake() {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);  
    }

    /// <summary>
    /// ストーリーを巻き戻す
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
            Debug.LogWarning("ストーリーのステップ数が制限を超えていることに注意してください。");
            _currentStep = actions.Count - 1;
        }
    }
    
    private void Start() {
        dialoguePanel = PanelManager.Instance.CreateDialoguePanel();
        dialoguePanelScript = dialoguePanel.GetComponent<DialoguePanel>();
        dialoguePanelScript.SetContinueTipsVis(false);
        dialoguePanel.SetActive(false);

        //ストーリーの登録
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
        currentPlayableDirector.playableGraph.GetRootPlayable(0).SetSpeed(0d);//一時停止を設定する
        // currentPlayableDirector.Pause();
    }

    public void ResumeTimeLine()
    {
        storyMode = StoryMode.GamePlay;
        currentPlayableDirector.playableGraph.GetRootPlayable(0).SetSpeed(1d);//再生に応答します
        // currentPlayableDirector.Resume();

        dialoguePanelScript.SetContinueTipsVis(false);
        dialoguePanel.SetActive(true);
    }



///
//すべての脚本
/// 
    ///<summary>
 
    ////// 第一幕
 
    ///</summary>
    public void StartStory()
    {

    }
}
