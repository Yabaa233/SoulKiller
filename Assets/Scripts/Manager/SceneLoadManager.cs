using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : singleton<SceneLoadManager>
{

    public float asyncTime;//非同期ロードの時間長さ
    override protected void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
    }
    ///<summary>

    ////// 指定されたシーンをロードします

    ///</summary>
    /// <param name="targetScene"> 目標シーン番号 </param>
    public void LoadScene(int targetScene)
    {
        FmodManager.Instance.stopBGM();
        StartCoroutine(IE_LoadScene(targetScene));
    }
    ///<summary>

    ////// 非同期ロードコルーチン

    ///</summary>
    /// <param name="targetScene"> 目標シーン番号 </param>
    /// <returns></returns>
    IEnumerator IE_LoadScene(int targetScene)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(targetScene);
        operation.allowSceneActivation = false;
        PanelManager.Instance.Open(new SceneChangePanel());
        SceneChangePanel sceneChangePanel = PanelManager.Instance.GetPanel("SceneChangePanel") as SceneChangePanel;
        while (!operation.isDone)
        {  
            asyncTime += Time.deltaTime;
            // Debug.Log("読み込み中...進行状況：" + (operation.progress * 100).ToString() + "%");
            sceneChangePanel.SetPercent(operation.progress * 100);
            if(sceneChangePanel.IFCanJump())
            {
                operation.allowSceneActivation = true;
            }
            yield return null;
        }
        PanelManager.Instance.Close(sceneChangePanel.UIType);
        yield break;
    }

    /// <summary>
    /// メインシーンをロードする
    /// </summary>
    /// <param name="targetScene"></param>
    public void LoadMainScene(int targetScene)
    {
        if(GameManager.Instance.currentPlayer != null)
        {
            Debug.Log("実行");
            GameManager.Instance.currentPlayer.characterBuffManager.RemoveAllBuff();
            PanelManager.Instance.CloseAllPanel();
        }
        StartCoroutine(IE_LoadMainScene(targetScene));
    }

    IEnumerator IE_LoadMainScene(int targetScene)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(targetScene);
        operation.allowSceneActivation = false;
        PanelManager.Instance.Open(new SceneChangePanel());
        SceneChangePanel sceneChangePanel = PanelManager.Instance.GetPanel("SceneChangePanel") as SceneChangePanel;
        while (!operation.isDone)
        {  
            asyncTime += Time.deltaTime;
            // Debug.Log("読み込み中...進行状況：" + (operation.progress * 100).ToString() + "%");
            sceneChangePanel.SetPercent(operation.progress * 100);
            if(sceneChangePanel.IFCanJump())
            {
                operation.allowSceneActivation = true;
            }
            yield return null;
        }
        PanelManager.Instance.Close(sceneChangePanel.UIType);
        PanelManager.Instance.Open(new StartPanel());
        PanelManager.Instance.SetSkipButton(false);
        yield break;
    }

    ///<summary>


    ////// 指定された戦闘シーンをロードします


    ///</summary>
    /// <param name="targetScene"> 目標シーン番号 </param>
    public void LoadBattleScene(int targetScene)
    {
        StartCoroutine(IE_LoadBattleScene(targetScene));
    }

    ///<summary>


    ////// 非同期ロードコルーチン
    /// ロードが完了した後、GameStartメソッドを呼び出します。


    ///</summary>
    /// <param name="targetScene"> 目標シーン番号 </param>
    /// <returns></returns>
    IEnumerator IE_LoadBattleScene(int targetScene)
    {
        float time = 0;
        AsyncOperation operation = SceneManager.LoadSceneAsync(targetScene);
        //シーン切り替えパネル関連
        operation.allowSceneActivation = false;
        PanelManager.Instance.Open(new SceneChangePanel());
        SceneChangePanel sceneChangePanel = PanelManager.Instance.GetPanel("SceneChangePanel") as SceneChangePanel;
        
        while (!operation.isDone)
        {
            // Debug.Log("読み込み中...進行状況：" + (operation.progress * 100).ToString() + "%");
            asyncTime += Time.deltaTime;
            sceneChangePanel.SetPercent(operation.progress * 100);
            if(sceneChangePanel.IFCanJump())
            {
                operation.allowSceneActivation = true;
            }
            yield return null;
        }
        GameManager.Instance.GameStart();
        PanelManager.Instance.Close(sceneChangePanel.UIType);
        while (time < 1.0f)
        {
            time += Time.deltaTime;
            yield return null;
        }
        time = 0;
        GameManager.Instance.OpenFirstRoomTrigger();
        //メインUIと初心者ガイドパネルを開く
        PanelManager.Instance.Open(new BattleMainPanel());
        PanelManager.Instance.Open(new GuidePanel());
        yield break;
    }
}
