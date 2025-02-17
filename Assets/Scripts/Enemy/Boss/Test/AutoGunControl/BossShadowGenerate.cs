using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShadowGenerate : MonoBehaviour
{
    private Transform shadow, weapon;
    [Header("蓄力时间")]
    public float entryTime;
    [Header("刀子高度")]
    public float knifeY;
    [Header("法术类型")]
    public int type;
    private void Awake()
    {
        shadow = transform.Find("Shadow");
        weapon = transform.Find("Weapon");
    }

    /// <summary>
    /// 生成阴影并开始计时掉落锅铲
    /// </summary>
    /// <param name="position"> 随机到的玩家附近位置 </param>
    public void ShadowGenerate(Vector3 position, BossControl bossControl)
    {
        //设置阴影位置
        shadow.position = position;
        shadow.gameObject.SetActive(true);
        //开始准备掉落锅铲
        StartCoroutine(DownSpatula());
    }

    /// <summary>
    /// 将此物体回收到对象池
    /// </summary>
    public void ShadowRecycle()
    {
        shadow.gameObject.SetActive(false);
        weapon.gameObject.SetActive(false);
        if (type == 1)
        {
            EffectManager.Instance.LetRecycleEffect("Effect_BossSpatula", this.gameObject, 2f);
        }
        else
        {
            EffectManager.Instance.LetRecycleEffect("Effect_BossPiece", this.gameObject, 2f);
        }
    }

    /// <summary>
    /// 掉落锅铲
    /// </summary>
    /// <returns></returns>
    IEnumerator DownSpatula()
    {
        float time = 0;
        //蓄力
        while (time < entryTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        weapon.position = shadow.position + new Vector3(0, knifeY, 0);
        weapon.localRotation = Quaternion.Euler(-90, 0, Random.Range(0, 360));
        weapon.gameObject.SetActive(true);
        yield break;
    }
}
