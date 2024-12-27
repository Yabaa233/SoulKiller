using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShadowGenerate : MonoBehaviour
{
    public bool trapStart;
    public bool trapShadow = true;
    [Header("间隔时间")]
    public float intervalTime;
    [Header("蓄力时间")]
    public float entryTime;
    [Header("铲子停滞时间")]
    public float holdTime;
    [Header("订阅事件组件")]
    public GameObject coreGameObject;
    private float startTime, endTime;
    private GameObject shadow, knife;
    private bool isDown;
    private Vector3 knifePosition;
    private bool isConduct;
    void Start()
    {
        shadow = transform.GetChild(0).gameObject;
        knife = transform.GetChild(1).gameObject;
    }
    /// <summary>
    /// 注册核心机关的事件与关卡进入事件
    /// </summary>
    private void OnEnable()
    {
        endTime = Time.time;
        transform.parent.GetComponent<TrapTrigger>().openTarp += () => trapStart = true;
        coreGameObject.GetComponent<SpitFireControl>().DeathNotice += Close;
    }
    private void OnDisable()
    {
        coreGameObject.GetComponent<SpitFireControl>().DeathNotice -= Close;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if ((!trapStart && !isConduct) || (!trapShadow && !isConduct)) return;//是否在陷阱开启状态与关卡进入状态，都false则陷阱关闭
        if (!isDown)//掉落厨具是否已经开始启动
        {
            if (Time.time > endTime + intervalTime)//判断时间间隔是否足够
            {
                isConduct = true;//陷阱开启状态
                shadow.transform.position = GameManager.Instance.currentPlayer.transform.position+new Vector3(0,1f,0);//指示特效显示
                shadow.SetActive(true);
                isDown = true;
                startTime = Time.time;
            }
        }
        else
        {
            /////攻击指示与厨具同时不存在，说明本次陷阱结束,刷新陷阱开启状态及结束时间
            if (!shadow.activeSelf && !knife.activeSelf)
            {
                isDown = false;
                endTime = Time.time;
                isConduct = false;
            }
            else
            {
                if (Time.time >= startTime + entryTime)
                {
                    if (!knife.activeSelf)//生成厨具
                    {
                        knife.SetActive(true);
                        knife.GetComponent<Rigidbody>().velocity = Vector3.zero;
                        knife.transform.localRotation = Quaternion.Euler(-90, 0, 0);
                        knife.transform.localPosition = shadow.transform.localPosition + new Vector3(0, 15, 0);
                        knifePosition = knife.transform.localPosition;
                    }
                    else
                    {
                        if (!shadow.GetComponent<Trap>().isTouch)//攻击指示已经是否已经被接触
                        {
                            knife.GetComponent<Rigidbody>().velocity += Vector3.down * 10;
                        }
                        else
                        {
                            if(Time.time>= shadow.GetComponent<Trap>().touchTime + holdTime)//大于厨具在地面的时间
                            {
                                knife.GetComponent<Rigidbody>().velocity += Vector3.up * 3;//反向加速
                                if (knife.transform.localPosition.y >= knifePosition[1])//判断坐标高度，还原状态
                                {
                                    knife.GetComponent<Rigidbody>().velocity = Vector3.zero;
                                    shadow.GetComponent<Trap>().isTouch = false;
                                    knife.SetActive(false);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void Close()
    {
        // this.trapStart = false;
        trapShadow = false;
    }
}
