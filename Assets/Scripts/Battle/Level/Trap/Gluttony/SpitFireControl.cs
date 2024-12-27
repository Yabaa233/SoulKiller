using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class SpitFireControl : MonoBehaviour
{
    [Header("间隔时间")]
    public float intervalTime;
    [Header("长嘴时间")]
    public float halfTime;
    [Header("喷火时长")]
    public float attackTime;
    [Header("火焰特效")]
    public GameObject fire;
    [Header("左右机关")]
    public GameObject left, right;
    [Header("开始旋转角(z轴)")]
    public float startRotation;                                 //起始旋转角
    [Header("结束旋转角(z轴)")]
    public float endRotation;                                   //结束旋转角
    [Header("机关启动标记")]
    public bool trapStart;
    [Header("关卡结束通知")]
    public UnityAction DeathNotice;
    private float startTime, endTime, breakTime, shutUpTime;    //预备喷火的启动时间，喷火结束时间，机关被破坏的时间，每一次闭嘴需要的时间
    private Transform upperJaw;                                 //上颚
    private Vector3 upperStart, upperPoint;                     //张嘴的原始坐标，最上界
    private bool inSpitFire,isDead;                                    //是否在喷火
    private RoomTrigger roomTrigger;
    /// <summary>
    /// 初始化赋值
    /// </summary>
    private void Awake()
    {
        upperJaw = gameObject.transform.GetChild(0);
        upperStart = upperJaw.transform.position;
        upperPoint = upperStart + new Vector3(0, 5f, 0);
        breakTime = shutUpTime = 0;
        shutUpTime = halfTime;
    }
    /// <summary>
    /// 启动时间
    /// </summary>
    private void Start()
    {
        startTime = Time.time;
        inSpitFire = true;
        roomTrigger = transform.parent.parent.GetComponent<RoomTrigger>();
        roomTrigger.clearCheck += () => isDead == true ;  //通关条件
    }
    /// <summary>
    /// 每次启动，要和事件Trigger绑定
    /// </summary>
    private void OnEnable()
    {
        transform.parent.GetComponent<TrapTrigger>().openTarp += () => trapStart = true;
    }
    private void Update()
    {
        if (!trapStart) return;
        if (left.activeSelf || right.activeSelf)//左右机关是否存在
        {
            if (inSpitFire)
            {
                SpitFire();//处在喷火阶段就执行喷火
            }
            else
            {
                fire.SetActive(false);//结束就要关闭火焰
                if (Time.time >= endTime + intervalTime)//间隔足够时间
                {
                    startTime = Time.time;
                    inSpitFire = true;
                }
            }
        }
        else
        {
            ///
            /// <summary>
            /// 破坏时间更新，如果在喷火
            /// 那么就需要将嘴闭上，之后就是空转
            /// </summary>
            ///
            if (breakTime == 0)
            {
                breakTime = Time.time;
            }
            if (inSpitFire && Time.time < breakTime + halfTime)
            {
                CloseFire();
            }
            if (!isDead)
            {
                DeathNotice();
                roomTrigger.TrapClear();
            }
            isDead = true;
        }
    }
    /// <summary>
    /// 时间控制，是否大于开始时间+张牙时间+喷火时间+闭嘴时间
    /// 张嘴控制
    /// 喷火控制
    /// 闭嘴控制
    /// </summary>
    void SpitFire()
    {
        if (Time.time >= startTime + halfTime + attackTime + shutUpTime)
        {
            endTime = Time.time;
            inSpitFire = false;
            return;
        }
        if (Time.time <= startTime + halfTime)
        {
            upperJaw.position = Vector3.Lerp(upperStart, upperPoint, (Time.time - startTime) / halfTime);
        }
        else
        {
            if (Time.time <= startTime + halfTime + attackTime)
            {
                if (!fire.activeSelf)
                {
                    fire.SetActive(true);
                    FMODUnity.RuntimeManager.PlayOneShot("event:/Level/BaoShi/fire");
                }
                float changeRotation = Mathf.Lerp(startRotation, endRotation, (Time.time - (startTime + halfTime)) / attackTime);
                fire.transform.localRotation = Quaternion.Euler(new Vector3(6.5f, 2.5f, changeRotation));
            }
            else
            {
                fire.SetActive(false);
                upperJaw.position = Vector3.Lerp(upperPoint, upperStart, (Time.time - (startTime + halfTime + attackTime) / shutUpTime));
            }
        }
    }
    /// <summary>
    /// 关火，闭嘴
    /// </summary>
    void CloseFire()
    {
        if (fire.activeSelf)
        {
            fire.SetActive(false);
        }
        upperJaw.position = Vector3.Lerp(upperJaw.position, upperStart, (Time.time - breakTime) / halfTime);
    }
}
