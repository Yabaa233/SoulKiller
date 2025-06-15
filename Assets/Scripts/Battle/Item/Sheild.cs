using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// シールド
/// </summary>
public class Sheild : MonoBehaviour
{
    public GameObject ShieldVFX;
    public GameObject RipplesVFX;
    public GameObject HitVFX;
    public GameObject BreakVFX;
    private Material mat;

    public Transform parent;//設置場所
    new SphereCollider collider;
    //データ部分
    public float sheildHealthy;//シールドの耐久性
    public bool IsBreak;

    /// <summary>
    /// シールドの初期化
    /// </summary>
    private void Awake() {
        GameObject shield = Instantiate(ShieldVFX,transform) as GameObject;
        shield.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        collider = shield.transform.GetChild(0).GetComponent<SphereCollider>();
        // shield.transform.position = new Vector3(shield.transform.position.x,shield.transform.position.y + collider.radius/2,shield.transform.position.z);
    }

    /// <summary>
    /// シールドの更新
    /// </summary>
    // ... existing code ...

    /// <summary>
    /// シールドの終了
    /// </summary>
    // ... existing code ...
}
