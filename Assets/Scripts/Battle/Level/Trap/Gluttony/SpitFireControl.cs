using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class SpitFireControl : MonoBehaviour
{
    [Header("Interval Time")]
    public float intervalTime;
    [Header("Long Beak Time")]
    public float halfTime;
    [Header("Flame duration")]
    public float attackTime;
    [Header("Flame special effects")]
    public GameObject fire;
    [Header("Left and Right Mechanism")]
    public GameObject left, right;
    [Header("Start rotating angle (Z-axis)")]
    public float startRotation;                                 //開始回転角
    [Header("End Rotation Angle (Z-axis)")]
    public float endRotation;                                   //回転角を終了します
    [Header("Institution Activation Marker")]
    public bool trapStart;
    [Header("Level Completion Notification")]
    public UnityAction DeathNotice;
    private float startTime, endTime, breakTime, shutUpTime;    //火炎放射の準備開始時間、火炎放射の終了時間、機構が破壊される時間、一回の口を閉じる必要な時間。
    private Transform upperJaw;                                 //上顎
    private Vector3 upperStart, upperPoint;                     //口を開ける原始座標、最上限
    private bool inSpitFire,isDead;                                    //火を吹いていますか？
    private RoomTrigger roomTrigger;
    ///<summary>

    ////// 初期化代入

    ///</summary>
    private void Awake()
    {
        upperJaw = gameObject.transform.GetChild(0);
        upperStart = upperJaw.transform.position;
        upperPoint = upperStart + new Vector3(0, 5f, 0);
        breakTime = shutUpTime = 0;
        shutUpTime = halfTime;
    }
    ///<summary>

    ////// 起動時間

    ///</summary>
    private void Start()
    {
        startTime = Time.time;
        inSpitFire = true;
        roomTrigger = transform.parent.parent.GetComponent<RoomTrigger>();
        roomTrigger.clearCheck += () => isDead == true ;  //クリア条件
    }
    ///<summary>

    ////// 起動するたびに、イベントTriggerとバインドする必要があります。

    ///</summary>
    private void OnEnable()
    {
        transform.parent.GetComponent<TrapTrigger>().openTarp += () => trapStart = true;
    }
    private void Update()
    {
        if (!trapStart) return;
        if (left.activeSelf || right.activeSelf)//左右の機関が存在するかどうか
        {
            if (inSpitFire)
            {
                SpitFire();//火を噴出す段階にあるときに火を噴出すを実行します。
            }
            else
            {
                fire.SetActive(false);//終了するときは、炎を消す必要があります。
                if (Time.time >= endTime + intervalTime)//十分な時間を間隔に
                {
                    startTime = Time.time;
                    inSpitFire = true;
                }
            }
        }
        else
        {
            ///
            ///<summary>

            ////// 時間の更新を破壊し、噴火している場合、
            /// それなら、口を閉じるべきで、その後は無駄骨です。

            ///</summary>
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
    ///<summary>

    ////// 時間制御、開始時間+口を開く時間+火を吹く時間+口を閉じる時間が大きいかどうか。
    /// 口を開けて制御してください
    /// 火炎制御
    /// 口を閉じて制御してください。

    ///</summary>
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
    ///<summary>

    ////// 火を消し、口を閉じてください。

    ///</summary>
    void CloseFire()
    {
        if (fire.activeSelf)
        {
            fire.SetActive(false);
        }
        upperJaw.position = Vector3.Lerp(upperJaw.position, upperStart, (Time.time - breakTime) / halfTime);
    }
}
