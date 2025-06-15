using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomTest : MonoBehaviour
{
    public float time = 3;          // A点からBまでの所要時間
    public Transform pointA;        // 点A
    public Transform pointB;        // 点B
    public GameObject shotter;      // Launcher
    public float g = -10;           // 重力加速度
    private bool isBoom = false;    // 初めて地面に触れ、爆発マークの準備をします。
    private Vector3 speed;          // 初期速度ベクトル

    private Rigidbody rb;           // 剛体コンポーネント
    private float firstTouchTime;   // タッチタイミング
    private bool inBlast;
    ///<summary>

    ////// 自分でpointAとpointBを登録して使用します。

    ///</summary>
    /*    private void Awake()
        {
            pointA = transform;
            pointB = GameObject.Find("Player").transform;
        }
*/
    ///<summary>

    ////// 爆発テスト

    ///</summary>
    void Start()
    {
        // rb = GetComponent<Rigidbody>();
        // inBlast = false;
        // //gameObject.GetComponent<Collider>().enabled = false;
        // // 物体をA点に置く
        // transform.position = pointA.position + new Vector3(0,1,0);
 
        // // 式を使って初速度を計算する
        // speed = new Vector3(
        //     (pointB.position.x - pointA.position.x) / time,
        //     (pointB.position.y - pointA.position.y) / time - 0.5f * g * time, 
        //     (pointB.position.z - pointA.position.z) / time);
 
        // // 重力の初速度は0です
        // rb.AddForce(speed, ForceMode.Impulse);
    }

    /// <summary>
    /// テストの初期化
    /// </summary>
    private void OnEnable() 
    {
        rb = GetComponent<Rigidbody>();
        inBlast = false;
        //gameObject.GetComponent<Collider>().enabled = false;
        // 物体をA点に置きます。
        if(pointA != null && pointB != null)
        {
            pointB = GameManager.Instance.currentPlayer.transform;
            transform.position = pointA.position + new Vector3(0,1,0);
    
            // 式を使って初速度を計算する
            speed = new Vector3(
                (pointB.position.x - pointA.position.x) / time,
                (pointB.position.y - pointA.position.y) / time - 0.5f * g * time, 
                (pointB.position.z - pointA.position.z) / time);
    
            // 重力の初速度は0です
            rb.AddForce(speed, ForceMode.Impulse);
        }
    }
    /// <summary>
    /// テストの更新
    /// </summary>
    void FixedUpdate()
    {
        // 重力シミュレーション
        // transform.Translate(speed * Time.deltaTime); // 位置のシミュレーション
        // transform.Translate(Gravity * Time.deltaTime);
        ///<summary>

        ////// 着地時間が3秒を超えた場合、サブオブジェクトのトリガーを起動し、爆発が終了した後、オブジェクトプールに回収します。

        ///</summary>
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
    ///<summary>

    ////// アース検出
/// タイマーと接触マークの起動が開始されます

    ///</summary>
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
    ///<summary>

    ////// 検出をトリガー（複数フレーム）、playbodyにマークを付ける

    ///</summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerJudge("PlayerBody"))
        {
            inBlast = true;
        }
    }
    ///<summary>

    ////// 同期を保証し、プレイヤーが退出する際に現在の状態を更新します。

    ///</summary>
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
    ///<summary>

    ////// 送信者を設定します

    ///</summary>
    /// <param name="gameObject"></param>
    public void SetShotter(GameObject gameObject)
    {
        shotter = gameObject;
    }
    ///<summary>

    ////// 試験の終了

    ///</summary>
}
