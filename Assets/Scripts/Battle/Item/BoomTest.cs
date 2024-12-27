using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomTest : MonoBehaviour
{
    public float time = 3;          // 代表从A点出发到B经过的时长
    public Transform pointA;        // 点A
    public Transform pointB;        // 点B
    public GameObject shotter;      // 发射人
    public float g = -10;           // 重力加速度
    private bool isBoom = false;    // 第一次接触地面，准备爆炸标记
    private Vector3 speed;          // 初速度向量
/*    private Vector3 Gravity;        // 重力向量*/
    private Rigidbody rb;           // 刚体组件
    private float firstTouchTime;   // 触碰计时
    private bool inBlast;
    /// <summary>
    /// 使用自行注册pointA和pointB
    /// </summary>
    /*    private void Awake()
        {
            pointA = transform;
            pointB = GameObject.Find("Player").transform;
        }*/
    void Start()
    {
        // rb = GetComponent<Rigidbody>();
        // inBlast = false;
        // //gameObject.GetComponent<Collider>().enabled = false;
        // // 将物体置于A点
        // transform.position = pointA.position + new Vector3(0,1,0);
 
        // // 通过一个式子计算初速度
        // speed = new Vector3(
        //     (pointB.position.x - pointA.position.x) / time,
        //     (pointB.position.y - pointA.position.y) / time - 0.5f * g * time, 
        //     (pointB.position.z - pointA.position.z) / time);
 
        // // 重力初始速度为0
        // rb.AddForce(speed, ForceMode.Impulse);
    }

    private void OnEnable() 
    {
        rb = GetComponent<Rigidbody>();
        inBlast = false;
        //gameObject.GetComponent<Collider>().enabled = false;
        // 将物体置于A点
        if(pointA != null && pointB != null)
        {
            pointB = GameManager.Instance.currentPlayer.transform;
            transform.position = pointA.position + new Vector3(0,1,0);
    
            // 通过一个式子计算初速度
            speed = new Vector3(
                (pointB.position.x - pointA.position.x) / time,
                (pointB.position.y - pointA.position.y) / time - 0.5f * g * time, 
                (pointB.position.z - pointA.position.z) / time);
    
            // 重力初始速度为0
            rb.AddForce(speed, ForceMode.Impulse);
        }
    }
    void FixedUpdate()
    {
        // 重力模拟
        // transform.Translate(speed * Time.deltaTime); // 模拟位移
        // transform.Translate(Gravity * Time.deltaTime);
        ///<summary>
        ///如果落地时间超过3秒，就启动子物体的trigger，并且在爆炸结束之后回收至对象池
        /// </summary>
        if (isBoom)
        {
            if (Time.time > firstTouchTime + 3f)
            {
                if (inBlast)
                {
                    if(shotter != null)
                    {
                        GameManager.Instance.EnemyAttack(shotter.GetComponent<BaseEnemyControl>());
                    }
                }
                GameObject boomOver = ObjectPool.Instance.GetObject("Enemy_BoomOverEff", EffectManager.Instance.transform, true, true);
                boomOver.transform.position = transform.position;
                EffectManager.Instance.LetRecycleEffect("Enemy_BoomOverEff", boomOver, 1.5f);
                ObjectPool.Instance.RecycleObj("Boom", gameObject);
                isBoom = false;
            }
        }
    }
    /// <summary>
    /// 接触地面的检测
    /// 会启动计时与接触标记
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerJudge("Ground"))
        {
            /*Debug.Log("Enter Ground");*/
            if (!isBoom)
            {
                isBoom = true;
                gameObject.transform.GetChild(0).gameObject.SetActive(true);
                firstTouchTime = Time.time;
            }
        }
        else
        {
            if (other.gameObject.layer == LayerJudge("Obstacle"))
            {
                ObjectPool.Instance.RecycleObj("Boom", gameObject);
            }
        }
    }
    /// <summary>
    /// 触发检测(多帧)，对playbody进行标记
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerJudge("PlayerBody"))
        {
            inBlast = true;
        }
    }
    /// <summary>
    /// 保证同步，退出时更新玩家所处状态
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerJudge("PlayerBody"))
        {
            inBlast = false;
        }
    }
    LayerMask LayerJudge(string LayerName)
    {
        return LayerMask.NameToLayer(LayerName);
    }
    /// <summary>
    /// 设置发射者
    /// </summary>
    /// <param name="gameObject"></param>
    public void SetShotter(GameObject gameObject)
    {
        shotter = gameObject;
    }
}
