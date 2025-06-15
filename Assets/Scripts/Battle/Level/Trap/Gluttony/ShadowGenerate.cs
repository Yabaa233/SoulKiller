using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShadowGenerate : MonoBehaviour
{
    public bool trapStart;
    public bool trapShadow = true;
    [Header("Interval Time")]
    public float intervalTime;
    [Header("Charging time")]
    public float entryTime;
    [Header("Shovel Downtime")]
    public float holdTime;
    [Header("Subscription Event Component")]
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
    /// コア機関の登録イベントとステージ入場イベント
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
        if ((!trapStart && !isConduct) || (!trapShadow && !isConduct)) return;//トラップが開いている状態とステージに入る状態、どちらもfalseであればトラップは閉じます。
        if (!isDown)//落とした調理器具の起動はもう始まりましたか？
        {
            if (Time.time > endTime + intervalTime)//時間間隔が十分かどうかを判断します
            {
                isConduct = true;//トラップが開いている状態
                shadow.transform.position = GameManager.Instance.currentPlayer.transform.position+new Vector3(0,1f,0);//特殊効果の表示を指示する
                shadow.SetActive(true);
                isDown = true;
                startTime = Time.time;
            }
        }
        else
        {
            /////攻撃指示と調理器具が同時に存在しないことは、この罠が終了したことを示しています。罠の開始状態と終了時間を更新してください。
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
                    if (!knife.activeSelf)//調理器具を生成する
                    {
                        knife.SetActive(true);
                        knife.GetComponent<Rigidbody>().velocity = Vector3.zero;
                        knife.transform.localRotation = Quaternion.Euler(-90, 0, 0);
                        knife.transform.localPosition = shadow.transform.localPosition + new Vector3(0, 15, 0);
                        knifePosition = knife.transform.localPosition;
                    }
                    else
                    {
                        if (!shadow.GetComponent<Trap>().isTouch)//攻撃指示はすでに接触されましたか？
                        {
                            knife.GetComponent<Rigidbody>().velocity += Vector3.down * 10;
                        }
                        else
                        {
                            if(Time.time>= shadow.GetComponent<Trap>().touchTime + holdTime)//台所用具が地面にある時間よりも長い
                            {
                                knife.GetComponent<Rigidbody>().velocity += Vector3.up * 3;//逆加速
                                if (knife.transform.localPosition.y >= knifePosition[1])//座標の高さを判断し、状態を復元します。
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
