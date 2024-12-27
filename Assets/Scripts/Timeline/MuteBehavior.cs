using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

[System.Serializable]
public class MuteBehavior : PlayableBehaviour
{
    private PlayableDirector playableDirector;//获取Timeline对象上的导演组件

    //屏蔽轨道需要的属性
    [Header("需要屏蔽的物体")] public GameObject muteObject;
    [Header("屏蔽玩家长剑")] public GameObject swordObject;
    [Header("屏蔽Boss")]public bool isNeedMuteBoss;
    [Header("播放爆炸音频")]public bool isNeedMuteSound;
    [Header("激活大门")]public bool activeDoor;
    [Header("激活SkipButton")]public bool activeSkipButton;
    [Header("激活玩家操作")]public bool isNeedActivePlayer = false;
    [Header("是否需要玩家溶解")]public bool isNeedPlayerDisslove = false;
    [Header("传入玩家物体")]public GameObject player;
    [Header("是否需要屏蔽主UI")]public bool isNeedMuteMainUI = false;
    [Header("是否需要打开FullPane")]public bool isNeedOpenFullPanel = false;
    private PlayerControl playerControl;
    private Material playermaterial;
    private bool isClipPlayed;
    private bool isJump = false;
    public override void OnPlayableCreate(Playable playable)
    {
        playableDirector = playable.GetGraph().GetResolver() as PlayableDirector;//需要解析之后进行类型转换，有点类似让粒子系统可以变成一个可挂载的脚本
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if(isClipPlayed == false && info.weight > 0)
        {
            if(muteObject != null)
            {
                playerControl = muteObject.GetComponent<PlayerControl>();
                swordObject.GetComponent<SpriteRenderer>().enabled = false;
                playerControl.DisableInput();
            }
            if(isNeedMuteBoss)
            {
                GameManager.Instance.currentBoss.gameObject.SetActive(false);
                GameObject.Destroy(GameManager.Instance.currentBoss.gameObject);
            }
            if(isNeedMuteSound)
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/Player/Zhang/fireBallBoomLow");
            }
            if(activeDoor)
            {
                TimelineManager.Instance.door.OpenWallCollider();
            }
            if(activeSkipButton)
            {
                GameObject buttonObject = GameObject.Find("PanelManager").transform.Find("UIRoot").Find("Canvas").Find("BattleUI").Find("SkipButton").gameObject;
                buttonObject.SetActive(true);
                if(PanelManager.Instance.isTip == false)
                {
                    buttonObject.GetComponent<Button>().onClick.AddListener(Skip);
                    PanelManager.Instance.isTip = true;
                }
            }
            if(isNeedActivePlayer)
            {
                playerControl.EnableInput();
            }
            if(isNeedPlayerDisslove)
            {
                playermaterial = player.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().material;
            }
            if(isNeedMuteMainUI)
            {
                // BattleMainPanel battleMainPanel = PanelManager.Instance.GetPanel("BattleMainPanel") as BattleMainPanel;
                // battleMainPanel.UITool.GetUI().SetActive(false);
                // //直接关闭掉好了，避免关卡循环重复创建
                // PanelManager.Instance.Close(battleMainPanel.UIType);
            }
            if(isNeedOpenFullPanel)
            {
                PanelManager.Instance.Open(new FullPanel());
                PanelManager.Instance.SetSkipButton(false);
            }

            isClipPlayed = true;
        }

        float x = (float)playable.GetDuration();
        if(x < 0.001f)
        {
            Debug.LogWarning("不能除这么小的数");
            x = 1;
        }
        float percent = (float)playable.GetTime()/x;

        if(percent > 0.95)
        {
            if(muteObject != null)
            {
                playerControl.EnableInput();
            }
        }

        if(isNeedPlayerDisslove)
        {
            playermaterial.SetFloat("_dissolve",percent);
        }

    }

    public void Skip()
    {
        Debug.Log("触发跳转");
        if(isJump)
        {
            return;
        }
        isJump = true;
        GameObject.Find("PanelManager").transform.Find("UIRoot").Find("Canvas").Find("BattleUI").Find("SkipButton").gameObject.SetActive(false);
        if(SceneManager.GetActiveScene().buildIndex == 1)
        {
            //停止Timeline
            GameObject.Find("Timeline").GetComponent<PlayableDirector>().Stop();
            SceneLoadManager.Instance.LoadBattleScene(2);//只跳转一次
            isJump = false;
            return;
        }
        else if(SceneManager.GetActiveScene().buildIndex == 3)
        {   
            GameObject.Find("Timeline").GetComponent<PlayableDirector>().Stop();
            SceneLoadManager.Instance.LoadMainScene(0);//跳转回到主菜单
            isJump = false;
            return;
        }
    }

}
