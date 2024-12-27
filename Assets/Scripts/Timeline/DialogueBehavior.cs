using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


[System.Serializable]
public class DialogueBehavior : PlayableBehaviour
{
    private PlayableDirector playableDirector;//获取Timeline对象上的导演组件

    //对话窗口需要的一些属性
    public string characterName;
    [TextArea(8,1)] public string dialogueLine;
    public int dialogueSize;
    public int characterNameSize;
    public Vector2 positionBias;
    public Sprite sprite;
    public bool isCenterPanel;//是否是中央面板
    public bool isDescribePanel;//是否是技能描述面板
    public E_BuffKind buffKind;
    public bool isSkipButton;//是否是跳过按钮
    public bool dialogueTestCenter;//对话框居中显示
    public bool characterNameCenter;//姓名居中显示
    public bool requirePause;//用户设置：这个对话完成之后，是否需要玩家按下空格
    public bool isGuidePanel;//是否是引导面板
    public bool isNeedWsadMove;//是否需要Wsad图片
    public bool isNeedSpaceIcon;//是否需要空格图片
    public bool isNeedQECode;//是否需要QE图片
    public bool isNeedMouse;//是否需要鼠标图片
    private bool isClipPlayed;//是否这个Clip片段已经播放结束了
    private bool pauseScheduled;


    public override void OnPlayableCreate(Playable playable)
    {
        playableDirector = playable.GetGraph().GetResolver() as PlayableDirector;//需要解析之后进行类型转换，有点类似让粒子系统可以变成一个可挂载的脚本
    }

    //类似MonoBehavior中的Update方法，每一帧都会进行调用
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {

        if(isClipPlayed == false && info.weight > 0) // 如果当前片段还没有播放，说明需要初始化
        {
            StoryManager.Instance.dialoguePanelScript.ResetAll();//还原所有更改
            StoryManager.Instance.dialoguePanelScript.SetText(characterName,dialogueLine);
            StoryManager.Instance.dialoguePanel.SetActive(true);
            StoryManager.Instance.dialoguePanelScript.SetDialogueSize(dialogueSize);
            StoryManager.Instance.dialoguePanelScript.SetCharacterNameSize(characterNameSize);
            if(requirePause)
            {
                StoryManager.Instance.dialoguePanelScript.SetContinueTipsVis(true);//显示如何继续的文字
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
                        StoryManager.Instance.dialoguePanelScript.SetDesribePanelVisble(true);//可见
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
                StoryManager.Instance.PauseTimeLine(playableDirector);//暂停timeline
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
