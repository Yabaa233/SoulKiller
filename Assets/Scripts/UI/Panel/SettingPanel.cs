using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
///<summary>

////// 設定インターフェース
/// xushi

///</summary>
public class SettingPanel : BasePanel
{
    static readonly string path = "UI/Panel/Setting";

    public SettingPanel() : base(new UIType(path)) { }

    //トップタブ
    //画面制御 音声
    GameObject video,control,sound;
    //左に移動 右に移動
    Button moveLeft,moveRight;

    //パネル：画面、制御、音声
    GameObject videoPanel, controlPanel, soundPanel;

    //メインボリューム
    Slider mainSound;
    //メインボリュームミュート
    Toggle mainSoundM;
    //サウンドエフェクトの音量
    Slider effect;
    //サウンドミュート
    Toggle effectM;
    //音楽の音量
    Slider music;
    //音楽をミュートにする
    Toggle musicM;
    //リセットボタン // 戻るボタン
    Button reset, quit;

    List<GameObject> panelList;

    Area_MainBuff area_MainBuff;
    //装飾線
    GameObject line;

    public override void OnInit()
    {
        base.OnInit();
        ifNeedUpdate = true;
    }
    public override void OnShow(params object[] para)
    {
        base.OnShow(para);
        video = UITool.FindChildGameObject("Btn_Video");
        control = UITool.FindChildGameObject("Btn_Control");
        sound = UITool.FindChildGameObject("Btn_Sound");

        videoPanel = UITool.FindChildGameObject("VideoPanel");
        controlPanel = UITool.FindChildGameObject("ControlPanel");
        soundPanel = UITool.FindChildGameObject("SoundPanel");

        moveLeft = UITool.GetOrAddComponentInChildren<Button>("Btn_TurnLeft");
        moveRight = UITool.GetOrAddComponentInChildren<Button>("Btn_TurnRight");
        //reset = UITool.GetOrAddComponentInChildren<Button>("Btn_Reset");
        quit = UITool.GetOrAddComponentInChildren<Button>("Btn_Close");

        mainSound = UITool.GetOrAddComponentInChildren<Slider>("Slider_SoundMain");
        effect = UITool.GetOrAddComponentInChildren<Slider>("Slider_Effect");
        music = UITool.GetOrAddComponentInChildren<Slider>("Slider_Music");

        mainSoundM = mainSound.gameObject.GetComponentInChildren<Toggle>();
        effectM = effect.gameObject.GetComponentInChildren<Toggle>();
        musicM = music.gameObject.GetComponentInChildren<Toggle>();

        line = UITool.FindChildGameObject("Image_LIne");

        //初期化
        panelList = new List<GameObject>();
        panelList.Add(videoPanel);
        panelList.Add(controlPanel);
        panelList.Add(soundPanel);

        //初期の開くインターフェース
        if (para.Length == 1)
        {
            if ((int)para[0] == 1)
                openControl();
            else if((int)para[0] == 2)
                openSound();
        }
        else
        {
            openControl();
        }


        //監視
        video.GetComponent<Button>().onClick.AddListener(openVideo);
        sound.GetComponent<Button>().onClick.AddListener(openSound);
        control.GetComponent<Button>().onClick.AddListener(openControl);

        moveLeft.onClick.AddListener(MoveLeft);
        moveRight.onClick.AddListener(MoveRight);

        quit.onClick.AddListener(Close);

        
        
        //area_MainBuff = PanelManager.Instance.GetPanel("Area_MainBuff") as Area_MainBuff;
        //area_MainBuff.isOpenPause = false;

        //coroutines.Add(MonoHelper.Instance.StartCoroutine(ReListen()));

        PanelManager.Instance.KeyBoardUpdateAction += PanelSwitch;
        PanelManager.Instance.KeyBoardUpdateAction += EscClose;

        Time.timeScale = 0f;

        
    }

    public override void Update()
    {
        base.Update();
        FmodManager.Instance.SetAllVolume(mainSound.value);
        FmodManager.Instance.SetEffectVolume(effect.value);
        FmodManager.Instance.SetMusicVolume(music.value);

    }
    public override void OnClose()
    {
        base.OnClose();
        Time.timeScale = 1f;

        PanelManager.Instance.KeyBoardUpdateAction -= PanelSwitch;

        //FmodManager.Instance.stopBGM();
        
        if(PanelManager.Instance.GetPanel("StartPanel")!=null)
            FmodManager.Instance.PlayBGM(FmodManager.Instance.BGMPathDefinitions[4].ambientAudioType);

        
    }

    //ESCでパネルを閉じる
    public void EscClose(KeyCode keyCode)
    {
        if (keyCode == KeyCode.Escape)
        {
            Close();
        }
    }

    ////エリアがキー入力を再度リッスンします
    //IEnumerator ReListen()
    //{
    //    float firstTime = Time.realtimeSinceStartup;
    //    while (Time.realtimeSinceStartup - firstTime < 0.1f)
    //    {
    //        yield return null;
    //    }
    //    PanelManager.Instance.KeyBoardUpdateAction += area_MainBuff.OpenBuffInfo;
    //}

    //トップバータブでパネルを切り替えます
    public void openControl()
    {
        line.SetActive(true);
        //その後、アニメーション切り替えに変更します。
        videoPanel.SetActive(false);
        soundPanel.SetActive(false);
        controlPanel.SetActive(true);

        line.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.5f);
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/panelOpen");

        control.GetComponent<Image>().color = new Color(1, 1, 1, 0.2f);
        video.GetComponent<Image>().color = new Color(1, 1, 1, 1f);
        sound.GetComponent<Image>().color = new Color(1, 1, 1, 1f);
    }
    public void openVideo()
    {
        line.SetActive(true);
        //その後、アニメーション切り替えに変更します。
        videoPanel.SetActive(true);
        soundPanel.SetActive(false);
        controlPanel.SetActive(false);

        line.transform.DOLocalRotate(new Vector3(0, 0, 60), 0.5f);
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/panelOpen");

        control.GetComponent<Image>().color = new Color(1, 1, 1, 1f);
        video.GetComponent<Image>().color = new Color(1, 1, 1, 0.2f);
        sound.GetComponent<Image>().color = new Color(1, 1, 1, 1f);
    }
    public void openSound()
    {
        line.SetActive(true);
        //その後、アニメーション切り替えに変更します。
        videoPanel.SetActive(false);
        soundPanel.SetActive(true);
        controlPanel.SetActive(false);

        line.transform.DOLocalRotate(new Vector3(0, 0, 90), 0.5f);
        FMODUnity.RuntimeManager.PlayOneShot("event:/UI/panelOpen");

        control.GetComponent<Image>().color = new Color(1, 1, 1, 1f);
        video.GetComponent<Image>().color = new Color(1, 1, 1, 1f);
        sound.GetComponent<Image>().color = new Color(1, 1, 1, 0.2f);
    }
    //左右で画面を切り替える
    public void MoveLeft()
    {
        //Debug.Log("left");
        GameObject curPanel=null;
        for(int i = 0; i < panelList.Count; i++)
        {
            if (panelList[i].activeInHierarchy == true)
            {
                //panelList[i].SetActive(false);
                if (i != 0)
                {
                    curPanel= panelList[i-1];
                    //panelList[i - 1].SetActive(true);
                }
                else
                {
                    curPanel= panelList[panelList.Count-1];
                    //panelList[panelList.Count -1].SetActive(true);
                }
                break;
            }

        }
        if (curPanel == soundPanel)
        {
            openSound();
        }else if (curPanel == controlPanel)
        {
            openControl();
        }else if(curPanel == videoPanel)
        {
            openVideo();
        }
        //line.SetActive(false);
        //FMODUnity.RuntimeManager.PlayOneShot("event:/UI/panelOpen");
    }
    public void MoveRight()
    {
        GameObject curPanel = null;
        for (int i = 0; i < panelList.Count; i++)
        {
            if (panelList[i].activeInHierarchy == true)
            {
                //panelList[i].SetActive(false);
                if (i != panelList.Count-1)
                {

                    curPanel = panelList[i + 1];
                }
                else
                {
                    curPanel= panelList[0];
                }
                break;
            }

        }
        if (curPanel == soundPanel)
        {
            openSound();
        }
        else if (curPanel == controlPanel)
        {
            openControl();
        }
        else if (curPanel == videoPanel)
        {
            openVideo();
        }
        //line.SetActive(false);
        //FMODUnity.RuntimeManager.PlayOneShot("event:/UI/panelOpen");
    }
    //キー検出方法
    public void PanelSwitch(KeyCode keyCode)
    {
        if (keyCode == KeyCode.Q)
            MoveRight();
        if (keyCode == KeyCode.E)
            MoveLeft();
    }

    //ボリュームを調整する

    //ミュート
    //戻る

}
