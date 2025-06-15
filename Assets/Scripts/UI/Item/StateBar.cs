using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


//統一されたステータスバー
public class StateBar : IDisposable
{
    //状態インスタンスを作成し取得する
    private GameObject statePanel;
    //statepanelを保有するオブジェクト
    private GameObject panelKeeper;
    //座標オフセット
    private Vector3 bias;
    //ヘルスバー画像関連
    private Image hpImage;
    private Image hpEffect;
    private bool disposedValue;
    private bool disposedValue1;

    public StateBar(GameObject _statePanel,GameObject _panelKeeper)
    {
        //対応するインスタンスを取得しました
        statePanel = _statePanel;
        panelKeeper = _panelKeeper;

        hpImage = statePanel.transform.Find("Hp").GetComponent<Image>();
        hpEffect = statePanel.transform.Find("HpEffect").GetComponent<Image>();
    }

    ///<summary>


    ////// 更新方法


    ///</summary>
    public void UpdateState(float currentHealth,float maxHealth)
    {
        statePanel.transform.localPosition = PanelManager.Instance.WorldPointToUILocalPoint(panelKeeper.transform.position + bias);

        //ヘルスバーの更新
        hpImage.fillAmount = currentHealth / maxHealth;//ヘルスバーのパーセンテージを設定する
        if (hpEffect.fillAmount > hpImage.fillAmount)
        {
            hpEffect.fillAmount -= 0.001f;
        }
        else
        {
            hpEffect.fillAmount = hpImage.fillAmount;
        }
    }

    /// <summary>
    /// オフセットを設定する
    /// </summary>
    /// <param name="_bias">Vector3オフセット</param>
    public void SetPositionBias(Vector3 _bias)
    {
        bias = _bias;
    }


    /// <summary>
    /// スケールサイズを設定する
    /// </summary>
    /// <param name="_scale">Vector3Scale</param>
    public void SetlocalScale(Vector3 _scale)
    {
        statePanel.GetComponent<RectTransform>().localScale = _scale;
    }

    ///<summary>


    ////// 現在のパネルを破壊する


    ///</summary>
    public void DestroyThis()
    {
        // Debug.Log("It is not being executed.");
        GameObject.Destroy(statePanel);
    }

    /// <summary>
    /// パネルホルダーに戻る
    /// </summary>
    /// <returns></returns>
    public GameObject GetBuffKeeper()
    {
        return panelKeeper;
    }

    /// <summary>
    /// ステータスパネルを戻す
    /// </summary>
    /// <returns></returns>
    public GameObject GetStatePanel()
    {
        return statePanel;
    }

    public void SetStatePanelVisable(bool state)
    {
        statePanel.SetActive(state);
    }

    public void Dispose()
    {
        
    }
}
