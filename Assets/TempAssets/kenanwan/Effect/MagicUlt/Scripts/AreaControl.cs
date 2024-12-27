using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaControl : MonoBehaviour
{
    [Tooltip("所有粒子是否播放")] public bool IsPlaying = true;
    [Tooltip("蓄力阶段粒子的播放时间")] public bool IsBuffed;
    [Tooltip("Buff后蓄力阶段粒子的播放时间")] public float BuffedTime = 1.4f;
    [Tooltip("正常蓄力阶段粒子的播放时间")] public float NormalTime = 2.0f;
    public int StartPS = 1;
    [SerializeField] private ParticleSystem[] particleSystems;
    private float timer;
    private ParticleSystem.MainModule main;
    [Header("法杖的吸引力")]public float attractiveForce = 5.0f;
    void Start()
    {
        particleSystems = GetComponentsInChildren<ParticleSystem>();
    }
    private void OnDisable()
    {
        timer = 0;
        IsPlaying = false;
    }
    private void OnEnable()
    {
        IsPlaying = true;
    }
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (!particleSystems[StartPS].isPlaying)
        {
            timer = 0;
        }
        // Debug.Log(timer);
        //停止播放蓄力阶段粒子
        if (IsBuffed)
        {
            if (timer > BuffedTime)
            {
                for (int i = 0; i < StartPS; i++)
                {
                    LiveChange(i, false);
                }
                LiveChange(4, true);
                LiveChange(5, true);
            }
            else
            {
                LiveChange(4, false);
                LiveChange(5, false);
            }
        }
        else
        {
            if (timer > NormalTime)
            {
                for (int i = 0; i < StartPS; i++)
                {
                    LiveChange(i, false);
                }
                LiveChange(4, true);
                LiveChange(5, true);
            }
            else
            {
                LiveChange(4, false);
                LiveChange(5, false);
            }
        }

        //停止播放所有粒子
        if (!IsPlaying)
        {
            for (int i = 0; i < particleSystems.Length; i++)
            {
                LiveChange(i, false);
            }
        }
        else if (IsPlaying)
        {
            for (int i = 0; i < particleSystems.Length; i++)
            {
                LiveChange(i, true);
            }
        }
    }

    void LiveChange(int index, bool live)
    {
        particleSystems[index].transform.gameObject.SetActive(live);
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "EmyBody")
        {
            other.transform.parent.GetComponent<Rigidbody>().AddForce((transform.position - other.transform.position).normalized * attractiveForce, ForceMode.Force);
        }
    }
}
