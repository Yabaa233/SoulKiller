using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheild : MonoBehaviour
{
    public GameObject ShieldVFX;
    public GameObject RipplesVFX;
    public GameObject HitVFX;
    public GameObject BreakVFX;
    private Material mat;

    public Transform parent;//存放位置
    new SphereCollider collider;
    //数据部分
    public float sheildHealthy;//护盾生命值
    public bool IsBreak;

    private void Awake() {
        GameObject shield = Instantiate(ShieldVFX,transform) as GameObject;
        shield.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        collider = shield.transform.GetChild(0).GetComponent<SphereCollider>();
        // shield.transform.position = new Vector3(shield.transform.position.x,shield.transform.position.y + collider.radius/2,shield.transform.position.z);
    }


}
