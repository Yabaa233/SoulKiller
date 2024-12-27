using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShadowDestroy : MonoBehaviour
{
    public float force;   //击退力度
    public float downSpeed = 5.0f; //下落速度
    public float damage = 10.0f;    //伤害值
    private Rigidbody rb;
    private void Awake()
    {
        //组件获取
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        //位置更新
        rb.velocity += Vector3.down * downSpeed;
    }
    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log(other.name);
        if (other.gameObject == GameManager.Instance.currentPlayer.gameObject)
        {
            GameManager.Instance.currentBoss.bossData.currentComboAttack = damage;
            GameManager.Instance.BossAttack();
            Vector3 forceVector = (other.gameObject.transform.position - gameObject.transform.position).normalized;
            forceVector[1] = 0;
            other.gameObject.GetComponent<Rigidbody>().AddForce(forceVector * force, ForceMode.Impulse);
            //回收
            transform.parent.GetComponent<BossShadowGenerate>().ShadowRecycle();
        }
        if (other.name == "Shadow" && other.transform.parent.gameObject == transform.parent.gameObject)
        {
            transform.parent.GetComponent<BossShadowGenerate>().ShadowRecycle();
        }
        FMODUnity.RuntimeManager.PlayOneShot("event:/Level/BaoShi/changziGround");

    }
}
