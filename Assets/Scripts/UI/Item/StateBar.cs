using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


//统一的状态栏
public class StateBar : IDisposable
{
    //创建并且获得状态实例
    private GameObject statePanel;
    //持有statepanel的物体
    private GameObject panelKeeper;
    //坐标偏移
    private Vector3 bias;
    //血条图片相关
    private Image hpImage;
    private Image hpEffect;
    private bool disposedValue;
    private bool disposedValue1;

    public StateBar(GameObject _statePanel,GameObject _panelKeeper)
    {
        //获取到对应的实例
        statePanel = _statePanel;
        panelKeeper = _panelKeeper;

        hpImage = statePanel.transform.Find("Hp").GetComponent<Image>();
        hpEffect = statePanel.transform.Find("HpEffect").GetComponent<Image>();
    }

    /// <summary>
    /// 更新方法
    /// </summary>
    public void UpdateState(float currentHealth,float maxHealth)
    {
        statePanel.transform.localPosition = PanelManager.Instance.WorldPointToUILocalPoint(panelKeeper.transform.position + bias);

        //血条更新
        hpImage.fillAmount = currentHealth / maxHealth;//设置血条的百分比
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
    /// 设置偏移
    /// </summary>
    /// <param name="_bias">Vector3偏移量</param>
    public void SetPositionBias(Vector3 _bias)
    {
        bias = _bias;
    }


    /// <summary>
    /// 设置Scale大小
    /// </summary>
    /// <param name="_scale">Vector3Scale</param>
    public void SetlocalScale(Vector3 _scale)
    {
        statePanel.GetComponent<RectTransform>().localScale = _scale;
    }

    /// <summary>
    /// 销毁当前面板
    /// </summary>
    public void DestroyThis()
    {
        // Debug.Log("没有执行");
        GameObject.Destroy(statePanel);
    }

    /// <summary>
    /// 返回面板持有者
    /// </summary>
    /// <returns></returns>
    public GameObject GetBuffKeeper()
    {
        return panelKeeper;
    }

    /// <summary>
    /// 返回状态面板
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
