using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : singleton<SceneLoadManager>
{

    public float asyncTime;//异步加载时间长度
    override protected void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
    }
    /// <summary>
    /// 加载指定场景
    /// </summary>
    /// <param name="targetScene"> 目标场景序号 </param>
    public void LoadScene(int targetScene)
    {
        FmodManager.Instance.stopBGM();
        StartCoroutine(IE_LoadScene(targetScene));
    }
    /// <summary>
    /// 异步加载协程
    /// </summary>
    /// <param name="targetScene"> 目标场景序号 </param>
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
            // Debug.Log("正在加载...加载进度：" + (operation.progress * 100).ToString() + "%");
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
    /// 加载主场景
    /// </summary>
    /// <param name="targetScene"></param>
    public void LoadMainScene(int targetScene)
    {
        if(GameManager.Instance.currentPlayer != null)
        {
            Debug.Log("执行");
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
            // Debug.Log("正在加载...加载进度：" + (operation.progress * 100).ToString() + "%");
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

    /// <summary>
    /// 加载指定战斗场景
    /// </summary>
    /// <param name="targetScene"> 目标场景序号 </param>
    public void LoadBattleScene(int targetScene)
    {
        StartCoroutine(IE_LoadBattleScene(targetScene));
    }

    /// <summary>
    /// 异步加载协程
    /// 加载完成调用GameStart方法
    /// </summary>
    /// <param name="targetScene"> 目标场景序号 </param>
    /// <returns></returns>
    IEnumerator IE_LoadBattleScene(int targetScene)
    {
        float time = 0;
        AsyncOperation operation = SceneManager.LoadSceneAsync(targetScene);
        //场景跳转面板相关
        operation.allowSceneActivation = false;
        PanelManager.Instance.Open(new SceneChangePanel());
        SceneChangePanel sceneChangePanel = PanelManager.Instance.GetPanel("SceneChangePanel") as SceneChangePanel;
        
        while (!operation.isDone)
        {
            // Debug.Log("正在加载...加载进度：" + (operation.progress * 100).ToString() + "%");
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
        //打开主UI和新手引导面板
        PanelManager.Instance.Open(new BattleMainPanel());
        PanelManager.Instance.Open(new GuidePanel());
        yield break;
    }
}
