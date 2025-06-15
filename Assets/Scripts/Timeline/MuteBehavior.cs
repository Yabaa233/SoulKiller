using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

[System.Serializable]
public class MuteBehavior : PlayableBehaviour
{
    private PlayableDirector playableDirector;//Timelineオブジェクト上のディレクターコンポーネントを取得します。

    //トラックをブロックするために必要な属性
    [Header("ブロックする必要があるオブジェクト")] public GameObject muteObject;
    [Header("プレイヤーのロングソードをブロックする")] public GameObject swordObject;
    [Header("ボスをブロックする")]public bool isNeedMuteBoss;
    [Header("爆発音声を再生する")]public bool isNeedMuteSound;
    [Header("ゲートをアクティベートする")]public bool activeDoor;
    [Header("SkipButtonをアクティブにする")]public bool activeSkipButton;
    [Header("プレイヤー操作を活性化する")]public bool isNeedActivePlayer = false;
    [Header("プレイヤーが溶解する必要がありますか？")]public bool isNeedPlayerDisslove = false;
    [Header("プレイヤーオブジェクトを渡す")]public GameObject player;
    [Header("メインUIをブロックする必要がありますか？")]public bool isNeedMuteMainUI = false;
    [Header("FullPaneを開く必要がありますか？")]public bool isNeedOpenFullPanel = false;
    private PlayerControl playerControl;
    private Material playermaterial;
    private bool isClipPlayed;
    private bool isJump = false;
    public override void OnPlayableCreate(Playable playable)
    {
        playableDirector = playable.GetGraph().GetResolver() as PlayableDirector;//解析後に型変換を行う必要があり、それは粒子システムをマウント可能なスクリプトに変えることに少し似ています。
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
                // //直接閉じてしまいましょう。これにより、ステージが繰り返し作成されるのを防ぎます。
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
            Debug.LogWarning("これほど小さい数を除算することはできません。");
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
        Debug.Log("ジャンプをトリガーする");
        if(isJump)
        {
            return;
        }
        isJump = true;
        GameObject.Find("PanelManager").transform.Find("UIRoot").Find("Canvas").Find("BattleUI").Find("SkipButton").gameObject.SetActive(false);
        if(SceneManager.GetActiveScene().buildIndex == 1)
        {
            //タイムラインを停止
            GameObject.Find("Timeline").GetComponent<PlayableDirector>().Stop();
            SceneLoadManager.Instance.LoadBattleScene(2);//一回だけジャンプします
            isJump = false;
            return;
        }
        else if(SceneManager.GetActiveScene().buildIndex == 3)
        {   
            GameObject.Find("Timeline").GetComponent<PlayableDirector>().Stop();
            SceneLoadManager.Instance.LoadMainScene(0);//メインメニューに戻る
            isJump = false;
            return;
        }
    }

}
