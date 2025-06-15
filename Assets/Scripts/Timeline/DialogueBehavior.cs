using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


[System.Serializable]
public class DialogueBehavior : PlayableBehaviour
{
    private PlayableDirector playableDirector;//Timelineオブジェクト上のディレクターコンポーネントを取得します。

    //ダイアログウィンドウに必要ないくつかの属性
    public string characterName;
    [TextArea(8,1)] public string dialogueLine;
    public int dialogueSize;
    public int characterNameSize;
    public Vector2 positionBias;
    public Sprite sprite;
    public bool isCenterPanel;//それは中央パネルですか？
    public bool isDescribePanel;//スキル説明パネルですか？
    public E_BuffKind buffKind;
    public bool isSkipButton;//スキップボタンですか？
    public bool dialogueTestCenter;//ダイアログボックスが中央に表示されます
    public bool characterNameCenter;//名前は中央に表示されます
    public bool requirePause;//ユーザー設定：この会話が終了した後、プレイヤーはスペースキーを押す必要がありますか？
    public bool isGuidePanel;//ガイドパネルですか？
    public bool isNeedWsadMove;//Wsadの画像が必要ですか？
    public bool isNeedSpaceIcon;//スペース画像が必要ですか？
    public bool isNeedQECode;//QE画像が必要ですか？
    public bool isNeedMouse;//マウスの画像が必要ですか？
    private bool isClipPlayed;//このクリップはすでに再生終了しましたか？
    private bool pauseScheduled;


    public override void OnPlayableCreate(Playable playable)
    {
        playableDirector = playable.GetGraph().GetResolver() as PlayableDirector;//解析後に型変換を行う必要があり、それは粒子システムをマウント可能なスクリプトに変えることに少し似ています。
    }

    //MonoBehaviorのUpdateメソッドのように、各フレームで呼び出されます。
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {

        if(isClipPlayed == false && info.weight > 0) // 現在のフラグメントがまだ再生されていない場合、初期化が必要であることを示しています。
        {
            StoryManager.Instance.dialoguePanelScript.ResetAll();//すべての変更を元に戻す
            StoryManager.Instance.dialoguePanelScript.SetText(characterName,dialogueLine);
            StoryManager.Instance.dialoguePanel.SetActive(true);
            StoryManager.Instance.dialoguePanelScript.SetDialogueSize(dialogueSize);
            StoryManager.Instance.dialoguePanelScript.SetCharacterNameSize(characterNameSize);
            if(requirePause)
            {
                StoryManager.Instance.dialoguePanelScript.SetContinueTipsVis(true);//続ける方法を示すテキスト
                pauseScheduled = true;
            }
            if(isCenterPanel)
            {
                StoryManager.Instance.dialoguePanelScript.MovePosition(positionBias);
                StoryManager.Instance.dialoguePanelScript.SetDialogueCenter(dialogueTestCenter);
            }
            else if(isDescribePanel)
            {
                StoryManager.Instance.dialoguePanelScript.SetIconVisible(true);
                foreach (var buffItem in PanelManager.Instance.buffInfoListSO.buffItems)
                {
                    if (buffItem.buffKind == buffKind)
                    {
                        StoryManager.Instance.dialoguePanelScript.SetDesribePanelVisble(true);//見えます
                        StoryManager.Instance.dialoguePanelScript.SetIconImage(buffItem.buffSprite);
                        StoryManager.Instance.dialoguePanelScript.SetDesribePanelText(buffItem.buffStory,buffItem.buffLevelDatas[0].levelDescribe+buffItem.buffDescribe);
                    }
                }
            }
            else if(isGuidePanel)
            {
                StoryManager.Instance.dialoguePanelScript.SetText("","");
                StoryManager.Instance.dialoguePanelScript.SetGuideTextVisble(true);
                StoryManager.Instance.dialoguePanelScript.SetGuideText(dialogueLine);
                if(isNeedWsadMove)
                {
                    StoryManager.Instance.dialoguePanelScript.SetwsadMoveVisible(true);
                }
                else if(isNeedSpaceIcon)
                {
                    StoryManager.Instance.dialoguePanelScript.SetSpaceIconVisible(true);
                }
                else if(isNeedQECode)
                {
                    StoryManager.Instance.dialoguePanelScript.SetQEcodeVisible(true);
                    StoryManager.Instance.dialoguePanelScript.SetQEcodeText(characterName);
                }
                else if(isNeedMouse)
                {
                    StoryManager.Instance.dialoguePanelScript.SetMouseVisible(true);
                    StoryManager.Instance.dialoguePanelScript.SetMouseSprite(sprite);
                }
            }

            StoryManager.Instance.dialoguePanelScript.SetDialogueCenter(dialogueTestCenter);
            // StoryManager.Instance.dialoguePanelScript.SetSkipButtonVisble(true);//TEST

            isClipPlayed = true;
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        isClipPlayed = false;
        // Debug.Log("Clip is Stoooooop");
        
        if(pauseScheduled)
        {
            pauseScheduled = false;
            if(StoryManager.Instance != null)
            {
                StoryManager.Instance.PauseTimeLine(playableDirector);//タイムラインを一時停止
            }
        }
        else
        {
            if(StoryManager.Instance != null)
            {
                StoryManager.Instance.dialoguePanel.SetActive(false);
            }
        }
    }
}
